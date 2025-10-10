namespace DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos
{
	public record ConfigurationSettingUpdateRequestDto(
			string Id,
			string Name,
			string Type,
			string Value,
			bool IsActive,
			long TimeStamp
		);
}
