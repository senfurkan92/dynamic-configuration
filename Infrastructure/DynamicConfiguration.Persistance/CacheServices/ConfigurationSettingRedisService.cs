using DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos;
using DynamicConfiguration.Application.Interfaces;
using DynamicConfiguration.Domain.Entities;
using DynamicConfiguration.Persistance.Repositories;
using Mapster;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace DynamicConfiguration.Persistance.CacheServices
{
	public class ConfigurationSettingRedisService : IConfigurationSettingRedisService
	{
		private readonly IConfigurationSettingMongoRepository _repository;
		private readonly IDatabase _redis;

		public ConfigurationSettingRedisService(string mongoCstr, string mongoDatabaseName, string redisCstr)
		{
			_repository = new ConfigurationSettingMongoRepository(mongoCstr, mongoDatabaseName);

			var connectionMultiplexer = ConnectionMultiplexer.Connect(redisCstr);
			_redis = connectionMultiplexer.GetDatabase(0);
		}

		public async Task<ConfigurationSettingGetByNameResponseDto?> Get(string application, string name, CancellationToken cancellationToken)
		{
			var key = GetKey(application, name);

			var cached = await _redis.StringGetAsync(key);

			if (cached.HasValue)
				return JsonConvert.DeserializeObject<ConfigurationSettingGetByNameResponseDto>(cached.ToString());

			var document = await _repository.Get(x => x.ApplicationName == application 
					&& x.Name == name 
					&& x.IsActive, 
				cancellationToken);

			var dto = document?.Adapt<ConfigurationSettingGetByNameResponseDto>();

			if (dto != null)
				await _redis.StringSetAsync(key, JsonConvert.SerializeObject(dto));

			return dto;
		}

		public async Task Remove(string application, string name, CancellationToken cancellationToken)
		{
			var key = GetKey(application, name);

			await _redis.KeyDeleteAsync(key);
		}

		public async Task<List<ConfigurationSettingListByApplicationResponseDto>> ListByApplication(string application, CancellationToken cancellationToken)
		{
			var key = ListByApplicationKey(application);

			var cached = await _redis.StringGetAsync(key);

			if (cached.HasValue)
				return JsonConvert.DeserializeObject<List<ConfigurationSettingListByApplicationResponseDto>>(cached.ToString()) ?? new();

			var document = await _repository.List(x => x.ApplicationName == application && x.IsActive, cancellationToken);

			var dto = document?.Adapt<List<ConfigurationSettingListByApplicationResponseDto>>();

			if (dto != null)
				await _redis.StringSetAsync(key, JsonConvert.SerializeObject(dto));

			return dto ?? new();
		}

		public async Task RemoveListByApplication(string application, CancellationToken cancellationToken)
		{
			var key = ListByApplicationKey(application);

			await _redis.KeyDeleteAsync(key);
		}

		private string GetKey(string application, string name) => $"{nameof(ConfigurationSetting)}.{application}.{name}";

		private string ListByApplicationKey(string application) => $"{nameof(ConfigurationSetting)}.list:{application}";
	}
}
