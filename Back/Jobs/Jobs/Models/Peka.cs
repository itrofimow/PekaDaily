using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Jobs.Models
{
    [MongoEntity("pekas")]
    public class Peka
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        public int Counter { get; set; }

        public string Url { get; set; }
    }
}