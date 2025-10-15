using DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos;
using DynamicConfiguration.Application.Interfaces;
using DynamicConfiguration.Persistance.Services;
using System.Globalization;

namespace DynamicConfiguration.Lib
{
    public class ConfigurationReader
    {
        private readonly IConfigurationSettingService _service;
        private readonly string _applicationName;
        private List<ConfigurationSettingListByApplicationResponseDto> _config = new();
        private readonly System.Timers.Timer? _timer;

        public ConfigurationReader(string applicationName, string connectionString, string databaseName, int refreshIntervalMs)
        {
            _service = new ConfigurationSettingService(connectionString, databaseName);

            _applicationName = applicationName;

            LoadConfiguration();

            if (refreshIntervalMs > 0) { 
                _timer = new System.Timers.Timer(refreshIntervalMs);
                _timer.Elapsed += async (sender, e) => await RefreshConfiguration();
                _timer.AutoReset = true;
                _timer.Start();     
            }
        }

        public object? GetValue(string key, CancellationToken cancellationToken = default)
        {
            var stringValue = _config.FirstOrDefault(x => x.Name == key);

            if (stringValue == null)
                return default;

            var targetType = GetTypeFromString(stringValue.Type);

            return ConvertToType(stringValue.Value, targetType);
        }

        public T? GetValue<T>(string key, CancellationToken cancellationToken = default)
        {
            var stringValue = _config.FirstOrDefault(x => x.Name == key);

            if (stringValue == null)
                return default(T?);

            return (T)ConvertToType(stringValue.Value, typeof(T));
        }

        private void LoadConfiguration()
        {
            _config = _service.ListByApplication(new ConfigurationSettingListByApplicationRequestDto(_applicationName), default).Result ?? new();
        }

        private async Task RefreshConfiguration()
        {
            await _service.RefreshListByApplication(new ConfigurationSettingRefreshListByApplicationRequestDto(_applicationName), default);
            LoadConfiguration();
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

        private Type GetTypeFromString(string typeName) => typeName switch
        {
            "String" => typeof(string),
            "Boolean" => typeof(bool),
            "Int32" => typeof(int),
            "Double" => typeof(double),
            "Decimal" => typeof(decimal),
            _ => throw new NotSupportedException($"Type '{typeName}' is not supported")
        };
    }
}
