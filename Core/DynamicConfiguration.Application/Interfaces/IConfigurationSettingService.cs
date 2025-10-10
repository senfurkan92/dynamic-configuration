using DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos;

namespace DynamicConfiguration.Application.Interfaces
{
	public interface IConfigurationSettingService
	{
		/// <summary>
		/// get document from db
		/// </summary>
		/// <param name="dto"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<ConfigurationSettingGetResponseDto?> Get(ConfigurationSettingGetRequestDto dto, CancellationToken cancellationToken);

		/// <summary>
		/// get document from cache
		/// </summary>
		/// <param name="dto"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<ConfigurationSettingGetByNameResponseDto?> GetByName(ConfigurationSettingGetByNameRequestDto dto, CancellationToken cancellationToken);

		/// <summary>
		/// get application names from db
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<ConfigurationSettingListApplicationResponseDto>> ListApplications(CancellationToken cancellationToken);

		/// <summary>
		/// get documents from db
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<ConfigurationSettingListResponseDto>> List(CancellationToken cancellationToken);

		/// <summary>
		/// get configs from redis by application
		/// </summary>
		/// <param name="dto"></param>
		/// <param name="cancellationToken"></>
		/// <returns></returns>param
		Task<List<ConfigurationSettingListByApplicationResponseDto>> ListByApplication(ConfigurationSettingListByApplicationRequestDto dto, CancellationToken cancellationToken);

		/// <summary>
		/// refresh configs on redis by application
		/// </summary>
		/// <param name="dto"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task RefreshListByApplication(ConfigurationSettingRefreshListByApplicationRequestDto dto, CancellationToken cancellationToken);

		/// <summary>
		/// create on db
		/// </summary>
		/// <param name="dto"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<ConfigurationSettingCreateResponseDto> Create(ConfigurationSettingCreateRequestDto dto, CancellationToken cancellationToken);

		/// <summary>
		/// update on db
		/// remove from redis
		/// </summary>
		/// <param name="dto"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<ConfigurationSettingUpdateResponseDto> Update(ConfigurationSettingUpdateRequestDto dto, CancellationToken cancellationToken);

		/// <summary>
		/// delete from db
		/// remove from redis
		/// </summary>
		/// <param name="dto"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<ConfigurationSettingDeleteResponseDto> Delete(ConfigurationSettingDeleteRequestDto dto, CancellationToken cancellationToken);
	}
}
