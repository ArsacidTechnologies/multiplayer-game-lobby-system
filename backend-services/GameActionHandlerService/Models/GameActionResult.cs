namespace game_action_handler.Models
{
    public class GameActionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public PlayerState UpdatedState { get; set; }
    }
}
