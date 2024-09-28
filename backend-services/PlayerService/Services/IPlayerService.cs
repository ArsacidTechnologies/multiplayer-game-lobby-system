using player_service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace player_service.Services
{
    public interface IPlayerService
    {
        Task<Player> CreatePlayerAsync(string playerName = null);
        Task<IEnumerable<Player>> GetPlayersAsync();
    }
}