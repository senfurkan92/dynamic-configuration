namespace DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos
{
	public record ConfigurationSettingListByApplicationResponseDto(
			string Id,
			string Name,
			string Type,
			string Value,
			bool IsActive,
			string ApplicationName,
			long TimeStamp
		);
}
