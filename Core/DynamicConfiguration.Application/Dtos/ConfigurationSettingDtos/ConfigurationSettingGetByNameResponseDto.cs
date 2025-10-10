namespace DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos
{
	public record ConfigurationSettingGetByNameResponseDto(
			string Id,
			string Name,
			string Type,
			string Value,
			bool IsActive,
			string ApplicationName,
			long TimeStamp
		);
}
