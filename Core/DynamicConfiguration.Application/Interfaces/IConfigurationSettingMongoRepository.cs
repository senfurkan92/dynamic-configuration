using DynamicConfiguration.Domain.Entities;

namespace DynamicConfiguration.Application.Interfaces
{
	public interface IConfigurationSettingMongoRepository : IMongoRepository<ConfigurationSetting>
	{
		/// <summary>
		/// list distinct application names
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<string>> ListApplications(CancellationToken cancellationToken);
	}
}
