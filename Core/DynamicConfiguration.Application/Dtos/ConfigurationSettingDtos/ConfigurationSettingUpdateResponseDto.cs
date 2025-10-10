namespace DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos
{
	public record ConfigurationSettingUpdateResponseDto(
			string Id,
			string Name,
			string Type,
			string Value,
			bool IsActive,
			string ApplicationName
		);
}
