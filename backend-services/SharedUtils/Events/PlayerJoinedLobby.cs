namespace SharedUtils.Events
{
    public class PlayerJoinedLobbyEvent
    {
        public string PlayerId { get; set; }
        public string LobbyId { get; set; }
        public string LobbyName { get; set; }
        public DateTime JoinedAt { get; set; }

        public PlayerJoinedLobbyEvent(string playerId, string lobbyId, string lobbyName)
        {
            PlayerId = playerId;
            LobbyId = lobbyId;
            LobbyName = lobbyName;
            JoinedAt = DateTime.UtcNow;
        }
    }
}