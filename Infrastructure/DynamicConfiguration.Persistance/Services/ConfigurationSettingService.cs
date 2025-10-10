using DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos;
using DynamicConfiguration.Application.Interfaces;
using DynamicConfiguration.Domain.Entities;
using DynamicConfiguration.Persistance.CacheServices;
using DynamicConfiguration.Persistance.RabbitMqService;
using DynamicConfiguration.Persistance.Repositories;
using Mapster;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System.Globalization;

namespace DynamicConfiguration.Persistance.Services
{
	public class ConfigurationSettingService: IConfigurationSettingService
	{
		private readonly IConfigurationSettingMongoRepository _repository;
		private readonly IConfigurationSettingRedisService _redisService;
		private readonly IConfigurationSettingRabbitMqService _rabbitMqService;
		private readonly string _mongoCstr;
		private readonly string _mongoDatabaseName;
		private readonly int? _refreshIntervalMs;

		public ConfigurationSettingService(string mongoCstr, string mongoDatabaseName, int? refreshIntervalMs = default)
		{
			var redisCstr = "localhost:6379";
			var rabbitMqCstr = "amqp://guest:guest@localhost:5672/";

			_mongoCstr = mongoCstr;
			_mongoDatabaseName = mongoDatabaseName;
			_repository = new ConfigurationSettingMongoRepository(mongoCstr, mongoDatabaseName);
			_redisService = new ConfigurationSettingRedisService(mongoCstr, mongoDatabaseName, redisCstr);
			_rabbitMqService = new ConfigurationSettingRabbitMqService(mongoCstr, mongoDatabaseName, rabbitMqCstr);
			_refreshIntervalMs = refreshIntervalMs;
		}

		public async Task<ConfigurationSettingCreateResponseDto> Create(ConfigurationSettingCreateRequestDto dto, CancellationToken cancellationToken)
		{
			ValidateValueType(dto.Value, dto.Type);

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

			return document.Adapt<ConfigurationSettingCreateResponseDto>();
		}

		public async Task<ConfigurationSettingDeleteResponseDto> Delete(ConfigurationSettingDeleteRequestDto dto, CancellationToken cancellationToken)
		{
			var document = await _repository.Get(x => x.Id == dto.Id, cancellationToken);

			if (document == null) 
				throw new InvalidOperationException("Configuration not found.");

			await _repository.Delete(x => x.Id == dto.Id, cancellationToken);

			await _redisService.Remove(_mongoCstr, _mongoDatabaseName, document.ApplicationName, document.Id, cancellationToken);

			await _rabbitMqService.Delete(dto.Id, cancellationToken);

			return new ConfigurationSettingDeleteResponseDto(dto.Id);
		}

		public async Task<ConfigurationSettingGetResponseDto?> Get(ConfigurationSettingGetRequestDto dto, CancellationToken cancellationToken)
		{
			var document = await _repository.Get(x => x.Id == dto.Id, cancellationToken);

			return document.Adapt<ConfigurationSettingGetResponseDto>();
		}

		public async Task<ConfigurationSettingGetByNameResponseDto?> GetByName(ConfigurationSettingGetByNameRequestDto dto, CancellationToken cancellationToken)
		{
			var document = await _redisService.Get(_mongoCstr, _mongoDatabaseName, dto.Application, dto.Name, cancellationToken);

			return document;
		}

		public async Task<List<ConfigurationSettingListApplicationResponseDto>> ListApplications(CancellationToken cancellationToken)
		{
			var data = await _repository.ListApplications(cancellationToken);

			return data.Select(x => new ConfigurationSettingListApplicationResponseDto(x)).ToList();
		}

		public async Task<List<ConfigurationSettingListResponseDto>> List(CancellationToken cancellationToken)
		{
			var documents = await _repository.List(x => true, cancellationToken);

			return documents?.ToList().Adapt<List<ConfigurationSettingListResponseDto>>() ?? new();
		}

		public async Task<List<ConfigurationSettingListByApplicationResponseDto>> ListByApplication(ConfigurationSettingListByApplicationRequestDto dto, CancellationToken cancellationToken)
		{
			var documents = await _repository.List(x => dto.ApplicationName == null || x.ApplicationName == dto.ApplicationName, cancellationToken);

			return documents.Adapt<List<ConfigurationSettingListByApplicationResponseDto>>()
				?? new();
		}

		public async Task<ConfigurationSettingUpdateResponseDto> Update(ConfigurationSettingUpdateRequestDto dto, CancellationToken cancellationToken)
		{
			ValidateValueType(dto.Value, dto.Type);

			var document = await _repository.Get(x => x.Id == dto.Id, cancellationToken);

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
			
			await _redisService.Remove(_mongoCstr, _mongoDatabaseName, document.ApplicationName, document.Id, cancellationToken);

			await _rabbitMqService.Update(dto.Id, cancellationToken);

			return document.Adapt<ConfigurationSettingUpdateResponseDto>();
		}

		private void ValidateValueType(string value, string typeName)
		{
			var targetType = typeName switch
			{
				"String" => typeof(string),
				"Boolean" => typeof(bool),
				"Int32" => typeof(int),
				"Double" => typeof(double),
				"Decimal" => typeof(decimal),
				_ => throw new NotSupportedException($"Type '{typeName}' is not supported")
			};

			try
			{
				_ = ConvertToType(value, targetType);
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException($"Value '{value}' is not compatible with type '{typeName}'");
			}
		}

		private object ConvertToType(string value, Type targetType)
		{
			try
			{
				if (targetType == typeof(string))
					return value;
				if (targetType == typeof(int))
					return int.Parse(value);
				if (targetType == typeof(bool))
					return bool.Parse(value);
				if (targetType == typeof(double))
					return double.Parse(value, CultureInfo.InvariantCulture);
				if (targetType == typeof(decimal))
					return decimal.Parse(value, CultureInfo.InvariantCulture);

				return Convert.ChangeType(value, targetType);
			}
			catch
			{
				throw new InvalidCastException($"Cannot convert '{value}' to {targetType.Name}");
			}
		}
	}
}
