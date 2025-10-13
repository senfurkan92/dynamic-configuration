namespace DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos
{
    public record ConfigurationSettingGetResponseDto(
            string Id,
            string Name,
            string Type,
            string Value,
            bool IsActive,
            string ApplicationName,
            long TimeStamp
        );
}
