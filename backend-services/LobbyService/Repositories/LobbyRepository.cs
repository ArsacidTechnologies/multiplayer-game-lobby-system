using lobby_service.Models;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SharedUtils.Utils;

namespace lobby_service.Repositories
{
    public class LobbyRepository : ILobbyRepository
    {
        private readonly IDatabase _db;

        public LobbyRepository(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task AddLobbyAsync(Lobby lobby)
        {
            var serializedLobby = JsonSerializer.Serialize(lobby);
            await _db.HashSetAsync(RedisKeyHelper.GetLobbyKey(lobby.LobbyId), lobby.LobbyId, serializedLobby);
        }

        public async Task<Lobby> GetLobbyAsync(string lobbyId)
        {
            var lobbyData = await _db.HashGetAsync(RedisKeyHelper.GetLobbyKey(lobbyId), lobbyId);
            if (string.IsNullOrEmpty(lobbyData))
            {
                return null;
            }

            return JsonSerializer.Deserialize<Lobby>(lobbyData);
        }

        public async Task<IEnumerable<Lobby>> GetLobbiesAsync()
        {
            var lobbies = new List<Lobby>();
            var allLobbyEntries = await _db.HashGetAllAsync("lobbies");

            foreach (var lobbyEntry in allLobbyEntries)
            {
                var lobby = JsonSerializer.Deserialize<Lobby>(lobbyEntry.Value);
                if (lobby != null)
                {
                    lobbies.Add(lobby);
                }
            }

            return lobbies;
        }

        public async Task UpdateLobbyAsync(Lobby lobby)
        {
            var serializedLobby = JsonSerializer.Serialize(lobby);
            await _db.HashSetAsync(RedisKeyHelper.GetLobbyKey(lobby.LobbyId), lobby.LobbyId, serializedLobby);
        }
    }
}
