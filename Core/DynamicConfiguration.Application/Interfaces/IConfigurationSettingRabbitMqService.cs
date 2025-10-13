namespace DynamicConfiguration.Application.Interfaces
{
    public interface IConfigurationSettingRabbitMqService
    {
        /// <summary>
        /// publishing message on configuration deleted
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Delete(string id, CancellationToken cancellationToken);

        /// <summary>
        /// publishing message on configuration updating
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task Update(string id, CancellationToken cancellationToken);
    }
}
