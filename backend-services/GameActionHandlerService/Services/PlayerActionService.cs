using game_action_handler.Models;
using System;
using System.Threading.Tasks;

namespace game_action_handler.Services
{
    public class PlayerActionService : IPlayerActionService
    {
        public async Task<PlayerAction> RollDiceAsync(string playerId)
        {
            var random = new Random();
            int diceRoll = random.Next(1, 7);  // Simulate a dice roll (1-6)

            var action = new PlayerAction
            {
                PlayerId = playerId,
                DiceRoll = diceRoll,
                Action = "roll",
                ActionTime = DateTime.UtcNow
            };

            // Perform further logic such as storing the action, etc.
            return await Task.FromResult(action);
        }

        public async Task<bool> MoveBoardNutAsync(string playerId, int nutId, int diceRoll, BoardNut boardNut, GameBoard gameBoard)
        {
            if (!boardNut.IsInPlay)
            {
                throw new InvalidOperationException("The board-nut is not in play.");
            }

            // Calculate the new position based on the dice roll
            int newPosition = boardNut.Position + diceRoll;

            // Check for special board rules (like ladders or penalties)
            if (gameBoard.SpecialPositions.ContainsKey(newPosition))
            {
                newPosition = gameBoard.SpecialPositions[newPosition];  // Apply the special position rule
            }

            // Validate if the new position is within the board size
            if (newPosition > gameBoard.BoardSize)
            {
                throw new InvalidOperationException("Invalid move. The board-nut cannot move beyond the board.");
            }

            // Update the board-nut position
            boardNut.Position = newPosition;

            return await Task.FromResult(true);  // Return success
        }
    }
}
