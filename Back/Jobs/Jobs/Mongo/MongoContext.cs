using System;
using System.Collections.Concurrent;
using Jobs.Models;
using MongoDB.Driver;

namespace Jobs.Mongo
{
    public class MongoContext
    {
        private readonly MongoClient _mongoClient;
        private readonly MongoUrl _mongoUrl;

        private readonly ConcurrentDictionary<Type, string> _collectionNameCache =
            new ConcurrentDictionary<Type, string>();

        public MongoContext(string uri)
        {
            _mongoUrl = new MongoUrl(uri);
            var settings = MongoClientSettings.FromUrl(_mongoUrl);

            _mongoClient = new MongoClient(settings);
        }

        public IMongoCollection<T> For<T>() where T : class
        {
            var collectionName = _collectionNameCache.GetOrAdd(typeof(T), GetCollectionName);
            return _mongoClient.GetDatabase(_mongoUrl.DatabaseName).GetCollection<T>(collectionName);
        }

        public string GetCollectionName(Type entity)
        {
            var attribute = (MongoEntity) Attribute.GetCustomAttribute(entity, typeof(MongoEntity));
            if (attribute == null)
                throw new Exception($"Mark {entity} with {typeof(MongoEntity)}");

            return attribute.CollectionName;
        }
    }
}