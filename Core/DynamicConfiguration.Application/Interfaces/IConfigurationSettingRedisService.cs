using DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos;
using MongoDB.Bson.IO;

namespace DynamicConfiguration.Application.Interfaces
{
	public interface IConfigurationSettingRedisService
	{
		/// <summary>
		/// get cached data
		/// </summary>
		/// <param name="cstr"></param>
		/// <param name="dbName"></param>
		/// <param name="application"></param>
		/// <param name="name"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<ConfigurationSettingGetByNameResponseDto?> Get(string cstr, string dbName, string application, string name, CancellationToken cancellationToken);

		/// <summary>
		/// remove cached data on update/delete
		/// </summary>
		/// <param name="cstr"></param>
		/// <param name="dbName"></param>
		/// <param name="application"></param>
		/// <param name="name"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task Remove(string cstr, string dbName, string application, string name, CancellationToken cancellationToken);

		/// <summary>
		/// list configs by application name
		/// </summary>
		/// <param name="cstr"></param>
		/// <param name="dbName"></param>
		/// <param name="application"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<ConfigurationSettingListByApplicationResponseDto>> ListByApplication(string cstr, string dbName, string application, CancellationToken cancellationToken);

		/// <summary>
		/// refresh configs by application name
		/// </summary>
		/// <param name="cstr"></param>
		/// <param name="dbName"></param>
		/// <param name="application"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task RefreshListByApplication(string cstr, string dbName, string application, CancellationToken cancellationToken);
	}
}
