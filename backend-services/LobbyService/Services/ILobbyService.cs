using lobby_service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lobby_service.Services
{
    public interface ILobbyService
    {
        Task<Lobby> CreateLobbyAsync(string lobbyName);
        Task RemovePlayerFromLobbyAsync(string lobbyId, string playerId);
        Task<Lobby> GetLobbieAsync(string lobbyId);
        Task<Lobby> JoinLobbyAsync(string lobbyId, string playerId);
        Task<IEnumerable<Lobby>> GetLobbiesAsync();
    }
}