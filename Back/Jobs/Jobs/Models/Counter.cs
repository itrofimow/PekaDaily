using MongoDB.Bson.Serialization.Attributes;

namespace Jobs.Models
{
    [MongoEntity("counter")]
    public class Counter
    {
        [BsonId]
        public int Id { get; set; }

        public int Value { get; set; }
    }
}