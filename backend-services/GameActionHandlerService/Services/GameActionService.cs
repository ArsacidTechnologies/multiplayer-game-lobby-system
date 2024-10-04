using game_action_handler.Models;
using game_action_handler.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace game_action_handler.Services
{
    public class GameActionService : IGameActionService
    {
        private readonly IPlayerStateRepository _stateRepository;

        public GameActionService(IPlayerStateRepository stateRepository)
        {
            _stateRepository = stateRepository;
        }

        /// <summary>
        /// Process the player action of rolling dice and moving the piece.
        /// </summary>
        /// <param name="action">The player's action, including the dice roll and piece to move.</param>
        /// <returns>A result indicating whether the action was successful and the updated state.</returns>
        public async Task<GameActionResult> ProcessPlayerActionAsync(PlayerAction action)
        {
            var playerState = await _stateRepository.GetPlayerStateAsync(action.PlayerId);
            if (playerState == null)
            {
                return new GameActionResult { Success = false, Message = "Player not found." };
            }

            // Get the current position of the piece
            if (!playerState.BoardPiecesPositions.ContainsKey(action.BoardPieceId))
            {
                return new GameActionResult { Success = false, Message = "Invalid board piece." };
            }

            var currentPosition = playerState.BoardPiecesPositions[action.BoardPieceId];

            // Calculate the new position
            var newPosition = currentPosition + action.DiceRoll;

            // Apply any special rules (e.g., ladders or penalties)
            newPosition = await ApplySpecialRulesAsync(newPosition);

            // Update the piece's position
            playerState.BoardPiecesPositions[action.BoardPieceId] = newPosition;

            // Save the updated player state
            await _stateRepository.SavePlayerStateAsync(playerState);

            return new GameActionResult
            {
                Success = true,
                Message = $"Piece moved to position {newPosition}.",
                UpdatedState = playerState
            };
        }

        /// <summary>
        /// Checks if a specific board position has any special rules (e.g., ladder or penalty).
        /// </summary>
        /// <param name="position">The position to check for special rules.</param>
        /// <returns>The updated position after applying special rules (if any).</returns>
        public async Task<int> ApplySpecialRulesAsync(int position)
        {
            // This is where you can apply special game rules
            // For example, assume position 5 is a ladder and moves you to 15
            if (position == 5)
            {
                return 15;  // Ladder rule
            }
            else if (position == 12)
            {
                return 6;   // Penalty rule
            }

            // Otherwise, return the same position
            return position;
        }
    }
}
