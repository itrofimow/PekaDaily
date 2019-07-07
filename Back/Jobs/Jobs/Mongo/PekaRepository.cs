using System.Collections.Generic;
using System.Threading.Tasks;
using Jobs.Models;
using MongoDB.Driver;

namespace Jobs.Mongo
{
    public interface IPekaRepository
    {
        Task<bool> CheckCounter(int counter);

        Task<Peka> GetPeka(int counter);

        Task AddPekas(List<Peka> pekas);
    }
    
    public class PekaRepository : IPekaRepository
    {
        private readonly MongoContext _context;

        public PekaRepository(MongoContext context)
        {
            _context = context;
        }
        
        public Task<bool> CheckCounter(int counter)
        {
            return _context.For<Peka>()
                .Find(x => x.Counter == counter)
                .AnyAsync();
        }

        public Task<Peka> GetPeka(int counter)
        {
            return _context.For<Peka>()
                .Find(x => x.Counter == counter)
                .SingleOrDefaultAsync();
        }

        public Task AddPekas(List<Peka> pekas)
        {
            return _context.For<Peka>()
                .InsertManyAsync(pekas);
        }
    }
}