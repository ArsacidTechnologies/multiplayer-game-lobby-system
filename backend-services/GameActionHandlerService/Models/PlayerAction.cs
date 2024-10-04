namespace game_action_handler.Models
{
    public class PlayerAction
    {
        public string PlayerId { get; set; }
        public int DiceRoll { get; set; }
        public int BoardPieceId { get; set; }
    }
}
