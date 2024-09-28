using StackExchange.Redis;
using player_service.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace player_service.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _db;

        public PlayerRepository(IConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
            _db = _redisConnection.GetDatabase();
        }

        public async Task AddPlayerAsync(Player player)
        {
            // Serialize player to JSON and store it in Redis with a unique key (player ID)
            var playerData = JsonSerializer.Serialize(player);
            await _db.HashSetAsync("players", player.Id, playerData);
        }

        public async Task<IEnumerable<Player>> GetPlayersAsync()
        {
            // Retrieve all players from Redis
            var players = new List<Player>();
            var playerEntries = await _db.HashGetAllAsync("players");

            foreach (var entry in playerEntries)
            {
                var player = JsonSerializer.Deserialize<Player>(entry.Value);
                if (player != null)
                {
                    players.Add(player);
                }
            }

            return players;
        }
    }
}