namespace DynamicConfiguration.WebUI.Models
{
	public record ConfigurationSettingCreateRequest(
			string MongoCstr, 
			string MongoDatabaseName,
			string Name,
			string Type,
			string Value,
			bool IsActive,
			string ApplicationName
		);
}
