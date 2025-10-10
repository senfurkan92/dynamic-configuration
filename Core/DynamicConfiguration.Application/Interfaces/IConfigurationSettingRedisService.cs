using DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos;
using MongoDB.Bson.IO;

namespace DynamicConfiguration.Application.Interfaces
{
	public interface IConfigurationSettingRedisService
	{
		Task<ConfigurationSettingGetByNameResponseDto?> Get(string cstr, string dbName, string application, string name, CancellationToken cancellationToken);

		Task Update(string cstr, string dbName, string application, string name, CancellationToken cancellationToken);

		Task Remove(string cstr, string dbName, string application, string name, CancellationToken cancellationToken);

		Task<List<ConfigurationSettingListByApplicationResponseDto>> ListByApplication(string cstr, string dbName, string application, CancellationToken cancellationToken);

		Task RefreshListByApplication(string cstr, string dbName, string application, CancellationToken cancellationToken);
	}
}
