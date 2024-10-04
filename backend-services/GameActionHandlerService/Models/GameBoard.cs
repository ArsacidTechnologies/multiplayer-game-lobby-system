namespace game_action_handler.Models
{
    public class GameBoard
    {
        public int BoardSize { get; set; }  // The total number of positions on the board
        public Dictionary<int, int> SpecialPositions { get; set; }  // E.g., ladders or penalties
    }
}
