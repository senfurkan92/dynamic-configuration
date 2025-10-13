namespace DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos
{
    public record ConfigurationSettingCreateRequestDto(
            string Name,
            string Type,
            string Value,
            bool IsActive,
            string ApplicationName
        );
}
