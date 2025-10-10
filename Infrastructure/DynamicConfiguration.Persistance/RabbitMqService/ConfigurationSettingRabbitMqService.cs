using DynamicConfiguration.Application.Interfaces;
using DynamicConfiguration.Persistance.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace DynamicConfiguration.Persistance.RabbitMqService
{
	public class ConfigurationSettingRabbitMqService : IConfigurationSettingRabbitMqService
	{
		private readonly IConfigurationSettingMongoRepository _repository;
		private readonly ConnectionFactory _factory;

		public ConfigurationSettingRabbitMqService(string mongoCstr, string mongoDatabaseName, string rabbitMqCstr)
		{
			_repository = new ConfigurationSettingMongoRepository(mongoCstr, mongoDatabaseName);

			_factory = new ConnectionFactory
			{
				Uri = new Uri(rabbitMqCstr)
			};
		}

		public async Task Update(string id, CancellationToken cancellationToken)
		{
			var document = await _repository.Get(x => x.Id == id, cancellationToken);

			if (document == null) return;

			var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(document));
			var exhange = "configuration.exchange";
			var routingKey = "configuration.update";

			await Publish(exhange, routingKey, body, cancellationToken);
		}

		public async Task Delete(string id, CancellationToken cancellationToken)
		{
			var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new { id }));
			var exhange = "configuration.exchange";
			var routingKey = "configuration.update";

			await Publish(exhange, routingKey, body, cancellationToken);
		}

		private async Task Publish(string exhange, string routingKey, byte[] body, CancellationToken cancellationToken)
		{
			using var conn = await _factory.CreateConnectionAsync(cancellationToken);

			using var channel = await conn.CreateChannelAsync();

			await channel.ExchangeDeclareAsync(exchange: exhange, type: ExchangeType.Topic, durable: true);

			var props = new BasicProperties
			{
				ContentType = "application/json"
			};

			await channel.BasicPublishAsync(
				exhange,
				routingKey,
				false,
				props,
				body,
				cancellationToken
			);
		}
	}
}
