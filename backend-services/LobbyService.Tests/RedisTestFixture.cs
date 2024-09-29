using StackExchange.Redis;
using Xunit;
namespace test_fixtures
{
    public class RedisTestFixture : IAsyncLifetime
    {
        private readonly IConnectionMultiplexer _redisConnection;
        public IDatabase Database { get; private set; }

        public RedisTestFixture()
        {
            // Connect to Redis (adjust the connection string if necessary)
            _redisConnection = ConnectionMultiplexer.Connect("localhost:6379");
            Database = _redisConnection.GetDatabase();
        }

        public async Task InitializeAsync()
        {
            // Clear Redis cache before any tests run
            await Database.ExecuteAsync("FLUSHALL");
        }

        public Task DisposeAsync()
        {
            // Clean up after all tests have run
            _redisConnection.Dispose();
            return Task.CompletedTask;
        }
    }
}