using lobby_service.Models;
using lobby_service.Repositories;
using RedLockNet.SERedis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SharedUtils.Utils;
using RedLockNet;
using MassTransit;
using SharedUtils.Events;

namespace lobby_service.Services
{
    public class LobbyService : ILobbyService
    {
        private readonly ILobbyRepository _lobbyRepository;
        private readonly IDatabase _db;
        private readonly IDistributedLockFactory _lockFactory;
        private readonly IBus _bus;
        public LobbyService(ILobbyRepository lobbyRepository, IConnectionMultiplexer redis, IDistributedLockFactory lockFactory, IBus bus)
        {
            _lobbyRepository = lobbyRepository;
            _db = redis.GetDatabase();
            _lockFactory = lockFactory;
            _bus = bus;

        }

        public async Task<Lobby> CreateLobbyAsync(string lobbyName)
        {
            // Check if a lobby with the same name already exists
            var existingLobby = await _lobbyRepository.GetLobbyByNameAsync(lobbyName);
            if (existingLobby != null)
            {
                throw new InvalidOperationException("Lobby name already exists.");
            }

            // If the lobby name is unique, proceed to create a new lobby
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
            // Define the lock key for this lobby
            var lockKey = RedisKeyHelper.GetLobbyKey(lobbyId);

            // Use Redlock to acquire a distributed lock for this lobby
            using (var redLock = await _lockFactory.CreateLockAsync(
                resource: lockKey,
                expiryTime: TimeSpan.FromSeconds(5),  // lock expiry
                waitTime: TimeSpan.FromSeconds(10),   // Time to wait for the lock
                retryTime: TimeSpan.FromSeconds(1)))  // Time between retries
            {
                if (redLock.IsAcquired)
                {
                    // Use Redis transactions for atomic updates
                    var transaction = _db.CreateTransaction();

                    // Check if player exists
                    if (!await _db.KeyExistsAsync(RedisKeyHelper.GetPlayerKey(playerId)))
                    {
                        throw new ArgumentException("Player does not exist.");
                    }

                    // Check if the player is already in a lobby
                    var existingLobbyId = await _db.StringGetAsync(RedisKeyHelper.GetUserLobbyKey(playerId));
                    if (!string.IsNullOrEmpty(existingLobbyId))
                    {
                        // If the player is already in a lobby, remove them from the old lobby
                        var oldLobbyKey = RedisKeyHelper.GetLobbyPlayersKey(existingLobbyId);
                        _ = transaction.SetRemoveAsync(oldLobbyKey, playerId);
                    }

                    // Get the new lobby
                    var lobby = await _lobbyRepository.GetLobbyAsync(lobbyId);
                    if (lobby == null)
                    {
                        throw new ArgumentException("Lobby not found.");
                    }

                    // Check if the new lobby has enough capacity
                    var lobbyPlayerCount = await _db.SetLengthAsync(RedisKeyHelper.GetLobbyPlayersKey(lobbyId));
                    if (lobbyPlayerCount >= lobby.Capacity)
                    {
                        throw new InvalidOperationException("Lobby is full.");
                    }

                    // Add the player to the new lobby in a transaction
                    _ = transaction.SetAddAsync(RedisKeyHelper.GetLobbyPlayersKey(lobbyId), playerId);

                    // Track player's membership in Redis in a transaction (set the player's new lobby)
                    _ = transaction.StringSetAsync(RedisKeyHelper.GetUserLobbyKey(playerId), lobbyId);

                    // Execute the transaction
                    bool success = await transaction.ExecuteAsync();
                    if (!success)
                    {
                        throw new InvalidOperationException("Failed to join lobby. Please try again.");
                    }

                    // Optionally update the lobby players in the repository
                    lobby.Players.Add(playerId);
                    await _lobbyRepository.UpdateLobbyAsync(lobby);

                    // Publish the PlayerJoinedLobbyEvent using MassTransit
                    var playerJoinedEvent = new PlayerJoinedLobbyEvent(playerId, lobbyId, lobby.LobbyName);
                    await _bus.Publish(playerJoinedEvent);

                    return lobby;
                }
                else
                {
                    // Lock not acquired, retry logic has failed
                    throw new InvalidOperationException("Could not acquire lock to join the lobby.");
                }
            }
        }
        public async Task RemovePlayerFromLobbyAsync(string lobbyId, string playerId)
        {
            // Define the lock key for this lobby
            var lockKey = RedisKeyHelper.GetLobbyKey(lobbyId);

            using (var redLock = await _lockFactory.CreateLockAsync(
                resource: lockKey,
                expiryTime: TimeSpan.FromSeconds(5),
                waitTime: TimeSpan.FromSeconds(10),
                retryTime: TimeSpan.FromSeconds(1)))
            {
                if (redLock.IsAcquired)
                {
                    // Use Redis transactions for atomic updates
                    var transaction = _db.CreateTransaction();

                    // Get the lobby
                    var lobby = await _lobbyRepository.GetLobbyAsync(lobbyId);
                    if (lobby == null)
                    {
                        throw new ArgumentException("Lobby not found.");
                    }

                    // Remove player from the Redis set in a transaction
                    _ = transaction.SetRemoveAsync(RedisKeyHelper.GetLobbyPlayersKey(lobbyId), playerId);

                    // Remove player's lobby association in a transaction
                    _ = transaction.KeyDeleteAsync(RedisKeyHelper.GetUserLobbyKey(playerId));

                    // Execute the transaction
                    bool success = await transaction.ExecuteAsync();
                    if (!success)
                    {
                        throw new InvalidOperationException("Failed to remove player from the lobby.");
                    }

                    // Optionally update the lobby in the repository
                    lobby.Players.Remove(playerId);
                    await _lobbyRepository.UpdateLobbyAsync(lobby);
                }
                else
                {
                    throw new InvalidOperationException("Could not acquire lock to remove the player from the lobby.");
                }
            }
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
