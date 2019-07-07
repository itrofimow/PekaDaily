using MongoDB.Bson.Serialization.Attributes;

namespace Jobs.Models
{
    public class Counter
    {
        [BsonId]
        public int Id { get; set; }

        public int Value { get; set; }
    }
}