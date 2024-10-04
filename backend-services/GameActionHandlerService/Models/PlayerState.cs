namespace game_action_handler.Models
{
    public class PlayerState
    {
        public string PlayerId { get; set; }
        public Dictionary<int, int> BoardPiecesPositions { get; set; } // Key: BoardPieceId, Value: Position
    }
}