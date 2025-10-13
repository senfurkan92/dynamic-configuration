namespace DynamicConfiguration.WebUI.Models
{
    public record ConfigurationSettingGetValueRequest(
            string MongoCstr,
            string MongoDatabaseName,
            string ApplicationName,
            string Name
        );
}
