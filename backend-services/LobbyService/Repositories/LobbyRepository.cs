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

            // Save the individual lobby under its own key (if needed)
            await _db.HashSetAsync(RedisKeyHelper.GetLobbyKey(lobby.LobbyId), lobby.LobbyId, serializedLobby);

            // Also, store the lobby in the "all lobbies" hash for collective access
            await _db.HashSetAsync(RedisKeyHelper.GetAllLobbiesKey(), lobby.LobbyId, serializedLobby);
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

            // Retrieve all lobbies from the collective Redis key
            var allLobbyEntries = await _db.HashGetAllAsync(RedisKeyHelper.GetAllLobbiesKey());

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

        public async Task<Lobby> GetLobbyByNameAsync(string lobbyName)
        {
            var allLobbies = await GetLobbiesAsync();

            // Check if any lobby has the same name
            foreach (var lobby in allLobbies)
            {
                if (lobby.LobbyName == lobbyName)
                {
                    return lobby;  // Return the found lobby
                }
            }

            return null;  // Return null if no lobby with the same name is found
        }
    }
}
