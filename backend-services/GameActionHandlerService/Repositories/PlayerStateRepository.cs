using game_action_handler.Models;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using game_action_handler.Repositories;
namespace game_action_handler.Repositories
{
    public class PlayerStateRepository : IPlayerStateRepository
    {
        // In-memory store (for simplicity). In real scenarios, this could be Redis or a database.
        private static ConcurrentDictionary<string, PlayerState> _playerStates = new ConcurrentDictionary<string, PlayerState>();

        public Task<PlayerState> GetPlayerStateAsync(string playerId)
        {
            _playerStates.TryGetValue(playerId, out var state);
            return Task.FromResult(state);
        }

        public Task SavePlayerStateAsync(PlayerState playerState)
        {
            _playerStates[playerState.PlayerId] = playerState;
            return Task.CompletedTask;
        }
    }
}
