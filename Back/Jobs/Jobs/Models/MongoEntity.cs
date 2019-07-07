using System;

namespace Jobs.Models
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MongoEntity : Attribute
    {
        public string CollectionName { get; }

        public MongoEntity(string collectionName)
        {
            CollectionName = collectionName;
        }
    }
}