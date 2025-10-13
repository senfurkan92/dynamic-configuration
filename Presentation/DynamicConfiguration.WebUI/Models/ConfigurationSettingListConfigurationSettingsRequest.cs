namespace DynamicConfiguration.WebUI.Models
{
    public record ConfigurationSettingListConfigurationSettingsRequest(
            string MongoCstr,
            string MongoDatabaseName
        );
}
