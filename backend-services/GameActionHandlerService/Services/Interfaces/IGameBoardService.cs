using game_action_handler.Models;
using System.Threading.Tasks;

namespace game_action_handler.Services
{
    public interface IGameBoardService
    {
        /// <summary>
        /// Retrieves the game board for a specific game.
        /// </summary>
        /// <param name="gameId">The ID of the game.</param>
        /// <returns>The game board object containing board details and rules.</returns>
        Task<GameBoard> GetGameBoardAsync(int gameId);

        /// <summary>
        /// Checks if a specific board position has any special rules (e.g., ladder or penalty).
        /// </summary>
        /// <param name="position">The position to check for special rules.</param>
        /// <returns>The updated position after applying special rules (if any).</returns>
        Task<int> ApplySpecialPositionRulesAsync(int position);
    }
}
