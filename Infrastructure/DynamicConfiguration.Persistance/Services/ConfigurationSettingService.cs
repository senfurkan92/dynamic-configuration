using DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos;
using DynamicConfiguration.Application.Interfaces;
using DynamicConfiguration.Domain.Entities;
using DynamicConfiguration.Persistance.CacheServices;
using DynamicConfiguration.Persistance.Repositories;
using Mapster;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;

namespace DynamicConfiguration.Persistance.Services
{
	public class ConfigurationSettingService: IConfigurationSettingService
	{
		private readonly IConfigurationSettingMongoRepository _repository;
		private readonly IConfigurationSettingRedisService _redisService;
		private readonly int? _refreshIntervalMs;

		public ConfigurationSettingService(string mongoCstr, string mongoDatabaseName, int? refreshIntervalMs = default)
		{
			var redisCstr = "localhost:6379";
			var rabbitMqCstr = "amqp://guest:guest@rabbitmq:5672/";

			_repository = new ConfigurationSettingMongoRepository(mongoCstr, mongoDatabaseName);
			_redisService = new ConfigurationSettingRedisService(mongoCstr, mongoDatabaseName, redisCstr);
			_refreshIntervalMs = refreshIntervalMs;
		}

		public async Task<ConfigurationSettingCreateResponseDto> Create(ConfigurationSettingCreateRequestDto dto, CancellationToken cancellationToken)
		{
			var document = new ConfigurationSetting
			{
				ApplicationName = dto.ApplicationName,
				IsActive = dto.IsActive,
				Name = dto.Name,
				TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
				Type = dto.Type,
				Value = dto.Value
			};

			await _repository.Create(document, cancellationToken);

			await _redisService.RemoveListByApplication(dto.ApplicationName, cancellationToken);

			return document.Adapt<ConfigurationSettingCreateResponseDto>();
		}

		public async Task<ConfigurationSettingDeleteResponseDto> Delete(ConfigurationSettingDeleteRequestDto dto, CancellationToken cancellationToken)
		{
			var document = await _repository.Get(x => x.Id == dto.Id, cancellationToken);

			if (document == null) 
				throw new InvalidOperationException("Configuration not found.");

			await _repository.Delete(x => x.Id == dto.Id, cancellationToken);

			await _redisService.Remove(document.ApplicationName, document.Id, cancellationToken);

			await _redisService.RemoveListByApplication(document.ApplicationName, cancellationToken);

			return new ConfigurationSettingDeleteResponseDto(dto.Id);
		}

		public async Task<ConfigurationSettingGetResponseDto?> Get(ConfigurationSettingGetRequestDto dto, CancellationToken cancellationToken)
		{
			var document = await _repository.Get(x => x.Id == dto.Id && x.IsActive, cancellationToken);

			return document.Adapt<ConfigurationSettingGetResponseDto>();
		}

		public async Task<ConfigurationSettingGetByNameResponseDto?> GetByName(ConfigurationSettingGetByNameRequestDto dto, CancellationToken cancellationToken)
		{
			var document = await _redisService.Get(dto.Application, dto.Name, cancellationToken);

			return document;
		}

		public async Task<List<ConfigurationSettingListApplicationResponseDto>> ListApplications(CancellationToken cancellationToken)
		{
			var data = await _repository.ListApplications(cancellationToken);

			return data.Select(x => new ConfigurationSettingListApplicationResponseDto(x)).ToList();
		}

		public async Task<List<ConfigurationSettingListResponseDto>> List(CancellationToken cancellationToken)
		{
			var documents = await _repository.List(x => x.IsActive, cancellationToken);

			return documents?.ToList().Adapt<List<ConfigurationSettingListResponseDto>>() ?? new();
		}

		public async Task<List<ConfigurationSettingListByApplicationResponseDto>> ListByApplication(ConfigurationSettingListByApplicationRequestDto dto, CancellationToken cancellationToken)
		{
			var documents = await _repository.List(x => x.ApplicationName == dto.ApplicationName, cancellationToken);

			return documents?.ToList().Adapt<List<ConfigurationSettingListByApplicationResponseDto>>()
				?? new();
		}

		public async Task<ConfigurationSettingUpdateResponseDto> Update(ConfigurationSettingUpdateRequestDto dto, CancellationToken cancellationToken)
		{
			var document = await _repository.Get(x => x.Id == dto.Id && x.IsActive, cancellationToken);

			if (document == null) 
				throw new InvalidOperationException("Configuration not found.");

			if (dto.TimeStamp != document.TimeStamp)
				throw new InvalidOperationException("The configuration has been modified by another process.");

			document.IsActive = dto.IsActive;
			document.Name = dto.Name;
			document.Type = dto.Type;
			document.Value = dto.Value;
			document.TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

			await _repository.Update(x => x.Id == document.Id, document, cancellationToken);
			
			await _redisService.Remove(document.ApplicationName, document.Id, cancellationToken);

			await _redisService.RemoveListByApplication(document.ApplicationName, cancellationToken);

			return document.Adapt<ConfigurationSettingUpdateResponseDto>();
		}
	}
}
