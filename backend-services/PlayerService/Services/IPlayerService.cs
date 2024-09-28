using player_service.Models;
using System.Collections.Generic;

namespace player_service.Services
{
    public interface IPlayerService
    {
        Player CreatePlayer(string playerName = null);
        IEnumerable<Player> GetPlayers();
    }
}