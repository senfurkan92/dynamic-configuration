namespace DynamicConfiguration.WebUI.Models
{
    public record ConfigurationSettingDeleteRequest(
            string MongoCstr,
            string MongoDatabaseName,
            string Id
        );
}
