using System.Linq.Expressions;

namespace DynamicConfiguration.Application.Interfaces
{
	public interface IMongoRepository<T> where T : class
	{
		Task<T> Create(T document, CancellationToken cancellationToken);
		Task Delete(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);
		Task<T?> Get(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);
		Task<IEnumerable<T>> List(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);
		Task<T?> Update(Expression<Func<T, bool>> filter, T document, CancellationToken cancellationToken);
	}
}
