namespace DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos
{
	public record ConfigurationSettingCreateResponseDto(
			string Id,
			string Name,
			string Type,
			string Value,
			bool IsActive,
			string ApplicationName
		);
}
