namespace DynamicConfiguration.WebUI.Models
{
	public record ConfigurationSettingUpdateRequest(
			string MongoCstr, 
			string MongoDatabaseName,
			string Id,
			string Name,
			string Type,
			string Value,
			bool IsActive,
			long TimeStamp
		);
}
