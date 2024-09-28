using lobby_service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lobby_service.Repositories
{
    public interface ILobbyRepository
    {
        Task AddLobbyAsync(Lobby lobby);
        Task<Lobby> GetLobbyAsync(string lobbyId);
        Task<IEnumerable<Lobby>> GetLobbiesAsync();
        Task UpdateLobbyAsync(Lobby lobby);
    }
}