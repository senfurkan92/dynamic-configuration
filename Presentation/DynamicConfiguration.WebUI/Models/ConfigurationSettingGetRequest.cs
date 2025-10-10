namespace DynamicConfiguration.WebUI.Models
{
	public record ConfigurationSettingGetRequest(
			string MongoCstr, 
			string MongoDatabaseName,
			string Id
		);
}
