using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using _7454E74E_B5DE_4630_A0FE_2DD6994282CD.Model;

namespace _7454E74E_B5DE_4630_A0FE_2DD6994282CD
{
    public class Repository<T> : IRepository<T> 
        where  T : IEntity
    {
        private readonly ILogger<Repository<T>> _logger;
        private readonly IMongoCollection<T> _collection;
        private readonly int _responseLimit;

        public Repository(
            IOptions<RepositorySetting> settings,
            ILogger<Repository<T>> logger)
        {
            if (settings.Value == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            _logger = logger;

            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.Database);
            _responseLimit = settings.Value.ResponseLimit;
            _collection  = database.GetCollection<T>(typeof(T).Name);
        }

        public Task<T> GetById(string id)
        {
            return _collection.Find(new BsonDocument {{"_id", new ObjectId(id)}}).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> GetAll(int? skip, int? limit)
        {
            return _collection.Find(_ => true).Skip(skip ?? 0).Limit(limit ?? _responseLimit).ToList();
        }

        public async Task<IEnumerable<T>> Get(string jsonFilter, int? skip, int? limit)
        {
            var filter = new BsonDocument(BsonSerializer.Deserialize<BsonDocument>(jsonFilter));
            return _collection.Find(filter).Skip(skip ?? 0).Limit(limit ?? _responseLimit).ToList();
        }
        
        public async Task<T> Add(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<bool> Update(T entity)
        {
            ReplaceOneResult actionResult 
                = await _collection
                    .ReplaceOneAsync(
                        new BsonDocument {{"_id", entity.Id}},
                        entity,
                        new UpdateOptions { IsUpsert = true });
            
            return actionResult.IsAcknowledged
                && actionResult.ModifiedCount > 0;
        }

        public async Task<bool> Delete(string id)
        {
            var actionResult = await _collection.DeleteOneAsync(new BsonDocument {{"_id", new ObjectId(id)}});
            return actionResult.IsAcknowledged;
        }

        public async Task<bool> Delete(T entity)
        {
            var actionResult = await _collection.DeleteOneAsync(new BsonDocument {{"_id", entity.Id}});
            return actionResult.IsAcknowledged;
        }
    }
}