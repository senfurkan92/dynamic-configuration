using DynamicConfiguration.Application.Interfaces;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace DynamicConfiguration.Persistance.Repositories
{
    public class MongoRepository<T> : IMongoRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> _collection;

        public MongoRepository(string connectionString, string databaseName, string collectionName)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _collection = database.GetCollection<T>(collectionName, null);
        }

        /// <summary>
        /// get from db
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<T?> Get(Expression<Func<T, bool>> filter, CancellationToken cancellationToken)
        {
            var documents = await List(filter, cancellationToken);

            return documents.FirstOrDefault();
        }

        /// <summary>
        /// list from db
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<List<T>> List(Expression<Func<T, bool>> filter, CancellationToken cancellationToken)
        {
            var documents = await _collection.FindAsync(filter, null, cancellationToken);

            var list = await documents.ToListAsync();

            return list;
        }

        /// <summary>
        /// create on db
        /// </summary>
        /// <param name="document"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<T> Create(T document, CancellationToken cancellationToken)
        {
            await _collection.InsertOneAsync(document, null, cancellationToken);

            return document;
        }

        /// <summary>
        /// update on db
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="document"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<T?> Update(Expression<Func<T, bool>> filter, T document, CancellationToken cancellationToken)
        {
            var current = await Get(filter, cancellationToken);

            if (current == null)
                throw new InvalidOperationException("Document not found.");

            var updated = await _collection.FindOneAndReplaceAsync(filter, document, null, cancellationToken);

            return updated;
        }

        /// <summary>
        /// delete from db
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task Delete(Expression<Func<T, bool>> filter, CancellationToken cancellationToken)
        {
            var result = await _collection.DeleteOneAsync(filter, cancellationToken);

            if (result.DeletedCount < 1)
                throw new InvalidOperationException("Document not deleted.");
        }
    }
}
