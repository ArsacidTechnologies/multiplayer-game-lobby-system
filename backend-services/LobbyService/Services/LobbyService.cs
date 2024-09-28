// Services/LobbyService.cs
using lobby_service.Models;
using lobby_service.Repositories;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedUtils.Utils;

namespace lobby_service.Services
{
    public class LobbyService : ILobbyService
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly IDatabase _db;

        public LobbyService(ILobbyRepository lobbyRepository, IConnectionMultiplexer redis)
        {
            _lobbyRepository = lobbyRepository;
            _db = redis.GetDatabase();
        }

        public async Task<Lobby> CreateLobbyAsync(string lobbyName)
        {
            var lobby = new Lobby
            {
                LobbyId = Guid.NewGuid().ToString(),
                LobbyName = lobbyName
            };

            await _lobbyRepository.AddLobbyAsync(lobby);
            return lobby;
        }

        public async Task<Lobby> JoinLobbyAsync(string lobbyId, string playerId)
        {
            // Use RedisKeyHelper to generate the correct Redis key for the player
            var playerKey = RedisKeyHelper.GetPlayerKey(playerId);

            // Check if player exists
            if (!await _db.KeyExistsAsync(playerKey))
            {
                throw new ArgumentException("Player does not exist.");
            }

            // Check if player is already in a lobby
            var existingLobbyId = await _db.StringGetAsync(RedisKeyHelper.GetUserLobbyKey(playerId));
            if (!string.IsNullOrEmpty(existingLobbyId))
            {
                throw new InvalidOperationException("User is already in another lobby.");
            }

            // Get the lobby
            var lobby = await _lobbyRepository.GetLobbyAsync(lobbyId);
            if (lobby == null)
            {
                throw new ArgumentException("Lobby not found.");
            }

            // Check if lobby has enough capacity
            var lobbyPlayerCount = await _db.SetLengthAsync(RedisKeyHelper.GetLobbyPlayersKey(lobbyId));
            if (lobbyPlayerCount >= lobby.Capacity)
            {
                throw new InvalidOperationException("Lobby is full.");
            }

            // Add player to the Redis set representing the lobby
            await _db.SetAddAsync(RedisKeyHelper.GetLobbyPlayersKey(lobbyId), playerId);

            // Track player's membership in Redis
            await _db.StringSetAsync(RedisKeyHelper.GetUserLobbyKey(playerId), lobbyId);

            // Optionally update the lobby players in the repository
            lobby.Players.Add(playerId);
            await _lobbyRepository.UpdateLobbyAsync(lobby);

            return lobby;
        }

        public async Task RemovePlayerFromLobbyAsync(string lobbyId, string playerId)
        {
            // Get the lobby
            var lobby = await _lobbyRepository.GetLobbyAsync(lobbyId);
            if (lobby == null)
            {
                throw new ArgumentException("Lobby not found.");
            }

            // Remove player from the Redis set
            await _db.SetRemoveAsync(RedisKeyHelper.GetLobbyPlayersKey(lobbyId), playerId);

            // Remove player's lobby association
            await _db.KeyDeleteAsync(RedisKeyHelper.GetUserLobbyKey(playerId));

            // Optionally update the lobby in the repository
            lobby.Players.Remove(playerId);
            await _lobbyRepository.UpdateLobbyAsync(lobby);
        }

        public async Task<Lobby> GetLobbieAsync(string lobbyId)
        {
            var lobby = await _lobbyRepository.GetLobbyAsync(lobbyId);
            if (lobby == null)
            {
                throw new ArgumentException("Lobby not found.");
            }

            // Sync Redis players data with repository
            var lobbyPlayers = await _db.SetMembersAsync(RedisKeyHelper.GetLobbyPlayersKey(lobbyId));
            lobby.Players = lobbyPlayers.ToStringArray().ToList();

            return lobby;
        }

        public Task<IEnumerable<Lobby>> GetLobbiesAsync()
        {
            return _lobbyRepository.GetLobbiesAsync();
        }
    }
}
