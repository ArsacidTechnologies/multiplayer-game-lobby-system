using player_service.Models;
using SharedUtils.Utils;  // Import RedisKeyHelper
using StackExchange.Redis;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace player_service.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly IDatabase _db;

        public PlayerRepository(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task AddPlayerAsync(Player player)
        {
            var playerKey = RedisKeyHelper.GetPlayerKey(player.Id);
            var playerData = JsonSerializer.Serialize(player);

            // Save player data to Redis using the generated key
            await _db.StringSetAsync(playerKey, playerData);

            // Also store the player's ID in a Redis Set to keep track of all players
            await _db.SetAddAsync("PlayerService:allPlayers", playerKey);
        }

        public async Task<IEnumerable<Player>> GetPlayersAsync()
        {
            var players = new List<Player>();

            // Retrieve all player keys from Redis Set
            var playerKeys = await _db.SetMembersAsync("PlayerService:allPlayers");

            // Iterate over each player key and retrieve the player data
            foreach (var playerKey in playerKeys)
            {
                var playerData = await _db.StringGetAsync(playerKey.ToString());
                if (!string.IsNullOrEmpty(playerData))
                {
                    players.Add(JsonSerializer.Deserialize<Player>(playerData));
                }
            }

            return players;
        }
    }
}
