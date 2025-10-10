using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConfiguration.Application.Interfaces
{
	public interface IConfigurationSettingRabbitMqService
	{
		Task Delete(string id, CancellationToken cancellationToken);
		Task Update(string id, CancellationToken cancellationToken);
	}
}
