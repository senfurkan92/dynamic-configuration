using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DynamicConfiguration.Domain.Entities
{
    public class ConfigurationSetting
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("name")]
        public string Name { get; set; } = null!;

        [BsonElement("type")]
        public string Type { get; set; } = null!;

        [BsonElement("value")]
        public string Value { get; set; } = null!;

        [BsonElement("isActive")]
        public bool IsActive { get; set; }

        [BsonElement("applicationName")]
        public string ApplicationName { get; set; } = null!;

        /// <summary>
        /// concurrency check
        /// </summary>
        [BsonElement("timeStamp")]
        public long TimeStamp { get; set; }
    }
}
