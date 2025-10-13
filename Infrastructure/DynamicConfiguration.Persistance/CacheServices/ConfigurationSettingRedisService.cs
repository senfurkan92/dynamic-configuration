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

        /// <summary>
        /// get cached data
        /// </summary>
        /// <param name="cstr"></param>
        /// <param name="dbName"></param>
        /// <param name="application"></param>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// remove cached data on update/delete
        /// </summary>
        /// <param name="cstr"></param>
        /// <param name="dbName"></param>
        /// <param name="application"></param>
        /// <param name="name"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Remove(string cstr, string dbName, string application, string name, CancellationToken cancellationToken)
        {
            var key = GetKey(cstr, dbName, application, name);

            await _redis.KeyDeleteAsync(key);
        }

        /// <summary>
        /// list configs by application name
        /// </summary>
        /// <param name="cstr"></param>
        /// <param name="dbName"></param>
        /// <param name="application"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<ConfigurationSettingListByApplicationResponseDto>> ListByApplication(string cstr, string dbName, string application, CancellationToken cancellationToken)
        {
            var key = GetListByApplication(cstr, dbName, application);

            var cached = await _redis.StringGetAsync(key);

            if (cached.HasValue)
                return JsonConvert.DeserializeObject<List<ConfigurationSettingListByApplicationResponseDto>>(cached.ToString()) ?? new();

            var documents = await _repository.List(x => x.ApplicationName == application && x.IsActive, cancellationToken);

            var dto = documents.Adapt<List<ConfigurationSettingListByApplicationResponseDto>>() ?? new();

            await _redis.StringSetAsync(key, JsonConvert.SerializeObject(dto));

            return dto;
        }

        /// <summary>
        /// refresh configs by application name
        /// </summary>
        /// <param name="cstr"></param>
        /// <param name="dbName"></param>
        /// <param name="application"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task RefreshListByApplication(string cstr, string dbName, string application, CancellationToken cancellationToken)
        {
            var key = GetListByApplication(cstr, dbName, application);

            var documents = await _repository.List(x => x.ApplicationName == application && x.IsActive, cancellationToken);

            var dto = documents.Adapt<List<ConfigurationSettingListByApplicationResponseDto>>() ?? new();

            await _redis.StringSetAsync(key, JsonConvert.SerializeObject(dto));
        }

        /// <summary>
        /// get key
        /// </summary>
        /// <param name="cstr"></param>
        /// <param name="dbName"></param>
        /// <param name="application"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private string GetKey(string cstr, string dbName, string application, string name) => $"{nameof(ConfigurationSetting)}.{ComputeHash(cstr, dbName)}.{application}.{name}";

        /// <summary>
        /// get key
        /// </summary>
        /// <param name="cstr"></param>
        /// <param name="dbName"></param>
        /// <param name="application"></param>
        /// <returns></returns>
        private string GetListByApplication(string cstr, string dbName, string application) => $"{nameof(ConfigurationSetting)}.{ComputeHash(cstr, dbName)}.{application}";

        /// <summary>
        /// get hashed for key
        /// </summary>
        /// <param name="cstr"></param>
        /// <param name="dbName"></param>
        /// <returns></returns>
        private string ComputeHash(string cstr, string dbName)
        {
            using var sha = SHA256.Create();
            var inputBytes = Encoding.UTF8.GetBytes($"{cstr}|{dbName}");
            var hashBytes = sha.ComputeHash(inputBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }
    }
}
