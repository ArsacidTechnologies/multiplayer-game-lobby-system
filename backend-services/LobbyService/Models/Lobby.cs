using System.Collections.Generic;

namespace lobby_service.Models
{
    public class Lobby
    {
        public string LobbyId { get; set; }
        public string LobbyName { get; set; }
        public int Capacity { get; set; } = 64;  // Default lobby capacity
        public List<string> Players { get; set; } = new List<string>(); // List of player IDs
    }
}