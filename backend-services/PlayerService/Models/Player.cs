namespace player_service.Models
{
    public class Player
    {
        public string Id { get; set; } // Random guest session number
        public string Name { get; set; } //Player's display name

        public Player()
        {
            // Generate random guest session number
            Id = Guid.NewGuid().ToString();
        }
    }
}