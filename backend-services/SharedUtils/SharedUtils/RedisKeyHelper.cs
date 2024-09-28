using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedUtils.Utils
{
    public static class RedisKeyHelper
    {
        private const string PlayerPrefix = "PlayerService:players:";
        private const string LobbyPrefix = "LobbyService:lobbies:";
        private const string UserLobbyPrefix = "user:";
        private const string LobbyPlayersSuffix = ":players";

        // Generate the Redis key for a player
        public static string GetPlayerKey(string playerId)
        {
            return $"{PlayerPrefix}{playerId}";
        }

        // Generate the Redis key for a lobby
        public static string GetLobbyKey(string lobbyId)
        {
            return $"{LobbyPrefix}{lobbyId}";
        }

        // Generate the Redis key for players in a specific lobby
        public static string GetLobbyPlayersKey(string lobbyId)
        {
            return $"{LobbyPrefix}{lobbyId}{LobbyPlayersSuffix}";
        }

        // Generate the Redis key for storing user's current lobby association
        public static string GetUserLobbyKey(string playerId)
        {
            return $"{UserLobbyPrefix}{playerId}:lobby";
        }

        // Generate the Redis key for all lobbies
        public static string GetAllLobbiesKey()
        {
            return $"{LobbyPrefix}all";
        }
    }
}