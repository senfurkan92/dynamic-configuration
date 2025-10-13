namespace DynamicConfiguration.WebUI.Models
{
    public record ConfigurationSettingListApplicationsRequest(
            string MongoCstr,
            string MongoDatabaseName
        );
}
