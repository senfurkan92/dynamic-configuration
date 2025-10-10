using DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos;
using DynamicConfiguration.Application.Interfaces;
using DynamicConfiguration.Domain.Entities;
using DynamicConfiguration.Persistance.Repositories;
using Mapster;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;

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

		public async Task<ConfigurationSettingGetByNameResponseDto?> Get(string cstr, string dbName, string application, string name, CancellationToken cancellationToken)
		{
			var key = GetKey(cstr, dbName, application, name);

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

		public async Task Update(string cstr, string dbName, string application, string name, CancellationToken cancellationToken)
		{
			var key = GetKey(cstr, dbName, application, name);

			var document = await _repository.Get(x => x.ApplicationName == application
					&& x.Name == name
					&& x.IsActive,
				cancellationToken);

			if (document == null) return;

			var dto = document?.Adapt<ConfigurationSettingGetByNameResponseDto>();

			if (dto != null)
				await _redis.StringSetAsync(key, JsonConvert.SerializeObject(dto));
		}

		public async Task Remove(string cstr, string dbName, string application, string name, CancellationToken cancellationToken)
		{
			var key = GetKey(cstr, dbName, application, name);

			await _redis.KeyDeleteAsync(key);
		}

		public async Task<List<ConfigurationSettingListByApplicationResponseDto>> ListByApplication(string cstr, string dbName, string application, CancellationToken cancellationToken)
		{
			var key = GetListByApplication(cstr, dbName, application);

			var cached = await _redis.StringGetAsync(key);

			if (cached.HasValue)
				return JsonConvert.DeserializeObject<List<ConfigurationSettingListByApplicationResponseDto>>(cached.ToString()) ?? new ();

			var documents = await _repository.List(x => x.ApplicationName == application && x.IsActive, cancellationToken);

			var dto = documents.Adapt<List<ConfigurationSettingListByApplicationResponseDto>>() ?? new ();

			await _redis.StringSetAsync(key, JsonConvert.SerializeObject(dto));

			return dto;
		}

		public async Task RefreshListByApplication(string cstr, string dbName, string application, CancellationToken cancellationToken)
		{
			var key = GetListByApplication(cstr, dbName, application);

			var documents = await _repository.List(x => x.ApplicationName == application && x.IsActive, cancellationToken);

			var dto = documents.Adapt<List<ConfigurationSettingListByApplicationResponseDto>>() ?? new();

			await _redis.StringSetAsync(key, JsonConvert.SerializeObject(dto));
		}

		private string GetKey(string cstr, string dbName, string application, string name) => $"{nameof(ConfigurationSetting)}.{ComputeHash(cstr, dbName)}.{application}.{name}";

		private string GetListByApplication(string cstr, string dbName, string application) => $"{nameof(ConfigurationSetting)}.{ComputeHash(cstr, dbName)}.{application}";

		private string ComputeHash(string cstr, string dbName)
		{
			using var sha = SHA256.Create();
			var inputBytes = Encoding.UTF8.GetBytes($"{cstr}|{dbName}");
			var hashBytes = sha.ComputeHash(inputBytes);

			return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
		}
	}
}
