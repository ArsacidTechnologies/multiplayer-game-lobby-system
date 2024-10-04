namespace game_action_handler.Models
{
    public class BoardNut
    {
        public int NutId { get; set; }  // The unique ID of the board-nut
        public string PlayerId { get; set; }  // The player who owns this nut
        public int Position { get; set; }  // The current position of the nut on the board
        public bool IsInPlay { get; set; }  // Whether the nut is currently in play or not
    }
}
