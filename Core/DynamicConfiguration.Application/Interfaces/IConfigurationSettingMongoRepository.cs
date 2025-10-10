using DynamicConfiguration.Domain.Entities;

namespace DynamicConfiguration.Application.Interfaces
{
	public interface IConfigurationSettingMongoRepository : IMongoRepository<ConfigurationSetting>
	{
		Task<List<string>> ListApplications(CancellationToken cancellationToken);
	}
}
