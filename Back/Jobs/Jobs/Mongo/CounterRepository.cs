using System.Threading.Tasks;
using Jobs.Models;
using MongoDB.Driver;

namespace Jobs.Mongo
{
    public interface ICounterRepository
    {
        Task<int> GetCurrent();

        Task IncrementCurrent();
    }
    
    public class CounterRepository : ICounterRepository
    {
        private readonly MongoContext _context;

        public CounterRepository(MongoContext context)
        {
            _context = context;
        }
        
        public Task<int> GetCurrent()
        {
            return _context.For<Counter>()
                .Find(x => x.Id == 1)
                .Project(x => x.Value)
                .SingleOrDefaultAsync();
        }

        public Task IncrementCurrent()
        {
            var update = Builders<Counter>.Update.Inc(x => x.Value, 1);

            return _context.For<Counter>()
                .UpdateOneAsync(x => x.Id == 1, update);
        }
    }
}