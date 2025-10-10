using DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos;

namespace DynamicConfiguration.Application.Interfaces
{
	public interface IConfigurationSettingRedisService
	{
		Task<ConfigurationSettingGetByNameResponseDto?> Get(string application, string name, CancellationToken cancellationToken);

		Task Remove(string application, string name, CancellationToken cancellationToken);

		Task<List<ConfigurationSettingListByApplicationResponseDto>> ListByApplication(string application, CancellationToken cancellationToken);

		Task RemoveListByApplication(string application, CancellationToken cancellationToken);
	}
}
