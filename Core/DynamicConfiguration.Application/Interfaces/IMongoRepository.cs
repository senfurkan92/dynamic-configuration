using System.Linq.Expressions;

namespace DynamicConfiguration.Application.Interfaces
{
	public interface IMongoRepository<T> where T : class
	{
		/// <summary>
		/// create on db
		/// </summary>
		/// <param name="document"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<T> Create(T document, CancellationToken cancellationToken);

		/// <summary>
		/// delete from db
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task Delete(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);

		/// <summary>
		/// get from db
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<T?> Get(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);

		/// <summary>
		/// list from db
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<List<T>> List(Expression<Func<T, bool>> filter, CancellationToken cancellationToken);

		/// <summary>
		/// update on db
		/// </summary>
		/// <param name="filter"></param>
		/// <param name="document"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		Task<T?> Update(Expression<Func<T, bool>> filter, T document, CancellationToken cancellationToken);
	}
}
