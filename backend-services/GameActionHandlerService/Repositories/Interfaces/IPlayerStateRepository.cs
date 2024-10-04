using game_action_handler.Models;
using System.Threading.Tasks;

namespace game_action_handler.Repositories
{
    public interface IPlayerStateRepository
    {
        Task<PlayerState> GetPlayerStateAsync(string playerId);
        Task SavePlayerStateAsync(PlayerState playerState);
    }
}
