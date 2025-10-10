using DynamicConfiguration.Application.Interfaces;
using DynamicConfiguration.Domain.Entities;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace DynamicConfiguration.Persistance.Repositories
{
	public class ConfigurationSettingMongoRepository(string connectionString, string databaseName) : MongoRepository<ConfigurationSetting>(connectionString, databaseName, "configurationSettings"), IConfigurationSettingMongoRepository
	{
		/// <summary>
		/// list distinct application names
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		public async Task<List<string>> ListApplications(CancellationToken cancellationToken)
		{
			var applications = await _collection
				.AsQueryable()
				.Where(x => x.IsActive)
				.Select(x => x.ApplicationName)
				.Distinct()
				.ToListAsync(cancellationToken);

			return applications;
		}
	}
}
