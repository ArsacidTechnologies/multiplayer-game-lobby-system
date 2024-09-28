using player_service.Models;
using System.Collections.Generic;

namespace player_service.Repositories
{
    public class PlayerRepository : IPlayerRepository
    {
        private readonly List<Player> _players = new List<Player>();

        public void AddPlayer(Player player)
        {
            _players.Add(player);
        }

        public IEnumerable<Player> GetPlayers()
        {
            return _players;
        }
    }
}