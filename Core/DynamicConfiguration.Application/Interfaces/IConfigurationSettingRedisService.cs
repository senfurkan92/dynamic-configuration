using DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos;

namespace DynamicConfiguration.Application.Interfaces
{
	public interface IConfigurationSettingRedisService
	{
		Task<ConfigurationSettingGetByNameResponseDto?> Get(string cstr, string dbName, string application, string name, CancellationToken cancellationToken);

		Task Update(string cstr, string dbName, string application, string name, CancellationToken cancellationToken);

		Task Remove(string cstr, string dbName, string application, string name, CancellationToken cancellationToken);
	}
}
