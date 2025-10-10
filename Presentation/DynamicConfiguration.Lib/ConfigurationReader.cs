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

		public ConfigurationReader(string applicationName, string connectionString, string databaseName, int refreshIntervalMs)
		{
			_service = new ConfigurationSettingService(connectionString, databaseName, refreshIntervalMs);

			_applicationName = applicationName;
		}

		public async Task<object?> GetValue(string key, CancellationToken cancellationToken = default)
		{
			var stringValue = await _service.GetByName(new ConfigurationSettingGetByNameRequestDto(_applicationName, key), cancellationToken);

			if (stringValue == null)
				return default;

			var targetType = GetTypeFromString(stringValue.Type);

			return ConvertToType(stringValue.Value, targetType);
		}

		public async Task<T?> GetValue<T>(string key, CancellationToken cancellationToken = default)
		{
			var stringValue = await _service.GetByName(new ConfigurationSettingGetByNameRequestDto(_applicationName, key), cancellationToken);

			if (stringValue == null)
				return default(T?);

			return (T)ConvertToType(stringValue.Value, typeof(T));
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
