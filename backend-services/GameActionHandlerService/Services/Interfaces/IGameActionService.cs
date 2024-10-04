using game_action_handler.Models;
using System.Threading.Tasks;

namespace game_action_handler.Services
{
    public interface IGameActionService
    {
        Task<GameActionResult> ProcessPlayerActionAsync(PlayerAction action);
        Task<int> ApplySpecialRulesAsync(int position);
    }
}
