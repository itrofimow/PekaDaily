using System.Threading.Tasks;
using Npgsql;

namespace Jobs.Cockroach
{
    public interface ICockroachPekaRepository
    {
        Task SetPeka(string url);
    }
    
    public class CockroachPekaRepository : ICockroachPekaRepository
    {
        private const string ConnectionString = "host=localhost;port=26257;user=root";
        
        public async Task SetPeka(string url)
        {
            using (var conn = new NpgsqlConnection(ConnectionString))
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "UPDATE peka SET peka = (@p) WHERE ID = 1";
                    cmd.Parameters.AddWithValue("p", url);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
    }
}