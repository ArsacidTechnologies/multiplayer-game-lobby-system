using player_service.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace player_service.Repositories
{
    public interface IPlayerRepository
    {
        Task AddPlayerAsync(Player player);
        Task<IEnumerable<Player>> GetPlayersAsync();
    }
}