using DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos;

namespace DynamicConfiguration.Application.Interfaces
{
	public interface IConfigurationSettingService
	{
		Task<ConfigurationSettingGetResponseDto?> Get(ConfigurationSettingGetRequestDto dto, CancellationToken cancellationToken);

		Task<ConfigurationSettingGetByNameResponseDto?> GetByName(ConfigurationSettingGetByNameRequestDto dto, CancellationToken cancellationToken);

		Task<List<ConfigurationSettingListApplicationResponseDto>> ListApplications(CancellationToken cancellationToken);

		Task<List<ConfigurationSettingListResponseDto>> List(CancellationToken cancellationToken);

		Task<List<ConfigurationSettingListByApplicationResponseDto>> ListByApplication(ConfigurationSettingListByApplicationRequestDto dto, CancellationToken cancellationToken);

		Task RefreshListByApplication(ConfigurationSettingRefreshListByApplicationRequestDto dto, CancellationToken cancellationToken);

		Task<ConfigurationSettingCreateResponseDto> Create(ConfigurationSettingCreateRequestDto dto, CancellationToken cancellationToken);

		Task<ConfigurationSettingUpdateResponseDto> Update(ConfigurationSettingUpdateRequestDto dto, CancellationToken cancellationToken);

		Task<ConfigurationSettingDeleteResponseDto> Delete(ConfigurationSettingDeleteRequestDto dto, CancellationToken cancellationToken);
	}
}
