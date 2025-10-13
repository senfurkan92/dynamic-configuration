using DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos;
using DynamicConfiguration.Application.Interfaces;
using DynamicConfiguration.Domain.Entities;
using DynamicConfiguration.Persistance.CacheServices;
using DynamicConfiguration.Persistance.RabbitMqService;
using DynamicConfiguration.Persistance.Repositories;
using Mapster;
using System.Globalization;

namespace DynamicConfiguration.Persistance.Services
{
    public class ConfigurationSettingService : IConfigurationSettingService
    {
        private readonly IConfigurationSettingMongoRepository _repository;
        private readonly IConfigurationSettingRedisService _redisService;
        private readonly IConfigurationSettingRabbitMqService _rabbitMqService;
        private readonly string _mongoCstr;
        private readonly string _mongoDatabaseName;

        public ConfigurationSettingService(string mongoCstr, string mongoDatabaseName)
        {
            var redisCstr = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING") ?? "localhost:6381";
            var rabbitMqCstr = Environment.GetEnvironmentVariable("RABBITMQ_CONNECTION_STRING") ?? "amqp://guest:guest@localhost:5674/";

            _mongoCstr = mongoCstr;
            _mongoDatabaseName = mongoDatabaseName;
            _repository = new ConfigurationSettingMongoRepository(mongoCstr, mongoDatabaseName);
            _redisService = new ConfigurationSettingRedisService(mongoCstr, mongoDatabaseName, redisCstr);
            _rabbitMqService = new ConfigurationSettingRabbitMqService(mongoCstr, mongoDatabaseName, rabbitMqCstr);
        }

        /// <summary>
        /// create on db
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// delete from db
        /// remove from redis
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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

        /// <summary>
        /// get document from db
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ConfigurationSettingGetResponseDto?> Get(ConfigurationSettingGetRequestDto dto, CancellationToken cancellationToken)
        {
            var document = await _repository.Get(x => x.Id == dto.Id, cancellationToken);

            return document.Adapt<ConfigurationSettingGetResponseDto>();
        }

        /// <summary>
        /// get document from cache
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ConfigurationSettingGetByNameResponseDto?> GetByName(ConfigurationSettingGetByNameRequestDto dto, CancellationToken cancellationToken)
        {
            var document = await _redisService.Get(_mongoCstr, _mongoDatabaseName, dto.Application, dto.Name, cancellationToken);

            return document;
        }

        /// <summary>
        /// get application names from db
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<ConfigurationSettingListApplicationResponseDto>> ListApplications(CancellationToken cancellationToken)
        {
            var data = await _repository.ListApplications(cancellationToken);

            return data.Select(x => new ConfigurationSettingListApplicationResponseDto(x)).ToList();
        }

        /// <summary>
        /// get documents from db
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<ConfigurationSettingListResponseDto>> List(CancellationToken cancellationToken)
        {
            var documents = await _repository.List(x => true, cancellationToken);

            return documents?.ToList().Adapt<List<ConfigurationSettingListResponseDto>>() ?? new();
        }

        /// <summary>
        /// get configs from redis by application
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></>
        /// <returns></returns>param
        public async Task<List<ConfigurationSettingListByApplicationResponseDto>> ListByApplication(ConfigurationSettingListByApplicationRequestDto dto, CancellationToken cancellationToken)
        {
            var documents = await _redisService.ListByApplication(_mongoCstr, _mongoDatabaseName, dto.ApplicationName, cancellationToken);

            return documents;
        }

        /// <summary>
        /// refresh configs on redis by application
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task RefreshListByApplication(ConfigurationSettingRefreshListByApplicationRequestDto dto, CancellationToken cancellationToken)
        {
            await _redisService.RefreshListByApplication(_mongoCstr, _mongoDatabaseName, dto.ApplicationName, cancellationToken);
        }

        /// <summary>
        /// update on db
        /// remove from redis
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
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
