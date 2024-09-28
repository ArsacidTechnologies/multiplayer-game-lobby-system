using player_service.Models;
using System.Collections.Generic;

namespace player_service.Repositories
{
    public interface IPlayerRepository
    {
        void AddPlayer(Player player);
        IEnumerable<Player> GetPlayers();
    }
}