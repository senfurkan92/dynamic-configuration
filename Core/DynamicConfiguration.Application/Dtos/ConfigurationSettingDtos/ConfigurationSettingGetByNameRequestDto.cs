namespace DynamicConfiguration.Application.Dtos.ConfigurationSettingDtos
{
    public record ConfigurationSettingGetByNameRequestDto(
            string Application,
            string Name
        );
}
