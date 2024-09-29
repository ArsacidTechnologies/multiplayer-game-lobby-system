using lobby_service.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xunit;
using test_fixtures;


namespace lobby_service.IntegrationTests
{
    public class LobbyControllerIntegrationTests : IClassFixture<LobbyServiceFixture>, IClassFixture<PlayerServiceFixture>, IClassFixture<RedisTestFixture>
    {
        private readonly HttpClient _lobbyClient;
        private readonly HttpClient _playerClient;
        private readonly RedisTestFixture _redisFixture;

        public LobbyControllerIntegrationTests(LobbyServiceFixture lobbyFixture, PlayerServiceFixture playerFixture, RedisTestFixture redisFixture)
        {
            _redisFixture = redisFixture;  // Redis fixture injected here
            _lobbyClient = lobbyFixture.CreateClient();  // Client for LobbyService
            _playerClient = playerFixture.CreateClient();  // Client for PlayerService
        }

        // Helper method to create a player in PlayerService
        private async Task<string> CreatePlayerAsync(string playerName)
        {
            var playerContent = new StringContent(JsonSerializer.Serialize(new { PlayerName = playerName }), Encoding.UTF8, "application/json");
            var playerResponse = await _playerClient.PostAsync("/api/player", playerContent);
            playerResponse.EnsureSuccessStatusCode();

            var playerResponseString = await playerResponse.Content.ReadAsStringAsync();
            var createdPlayer = JsonSerializer.Deserialize<Dictionary<string, string>>(playerResponseString);
            return createdPlayer["id"];  // Assuming player ID is returned as "id"
        }

        // Helper method to create a lobby in LobbyService
        private async Task<Lobby> CreateLobbyAsync(string lobbyName)
        {
            var response = await _lobbyClient.PostAsync($"/api/lobby?lobbyName={lobbyName}", null);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Lobby>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        // Test to ensure a lobby is successfully created
        [Fact]
        public async Task CreateLobby_ReturnsOk_WithCreatedLobby()
        {
            var lobbyName = $"TestLobby_{Guid.NewGuid()}";

            var createdLobby = await CreateLobbyAsync(lobbyName);

            // Assert
            Assert.NotNull(createdLobby.LobbyId);
            Assert.Equal(lobbyName, createdLobby.LobbyName);
            Assert.Equal(64, createdLobby.Capacity);
            Assert.Empty(createdLobby.Players);
        }

        // Test to ensure a player can join a lobby
        [Fact]
        public async Task JoinLobby_ReturnsOk_WhenPlayerIsCreatedInPlayerService()
        {
            // Step 1: Create a unique player using the PlayerService
            var playerId = await CreatePlayerAsync($"Player_{Guid.NewGuid()}");

            // Step 2: Create a unique lobby using the LobbyService
            var createdLobby = await CreateLobbyAsync($"TestLobby_{Guid.NewGuid()}");
            var lobbyId = createdLobby.LobbyId;

            // Step 3: Player joins the lobby
            var joinResponse = await _lobbyClient.PostAsync($"/api/lobby/{lobbyId}/join?playerId={playerId}", null);
            joinResponse.EnsureSuccessStatusCode();

            var joinResponseString = await joinResponse.Content.ReadAsStringAsync();
            var updatedLobby = JsonSerializer.Deserialize<Lobby>(joinResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            Assert.NotNull(updatedLobby);
            Assert.Contains(playerId, updatedLobby.Players);
        }

        // Test to check if joining a non-existent lobby returns NotFound
        [Fact]
        public async Task JoinLobby_ReturnsNotFound_WhenLobbyDoesNotExist()
        {
            // Step 1: Create a player in PlayerService
            var playerId = await CreatePlayerAsync($"Player_{Guid.NewGuid()}");

            // Step 2: Try to join a non-existent lobby
            var nonExistentLobbyId = "non-existent-lobby";
            var joinResponse = await _lobbyClient.PostAsync($"/api/lobby/{nonExistentLobbyId}/join?playerId={playerId}", null);

            // Assert: The response should be 404 Not Found
            Assert.Equal(System.Net.HttpStatusCode.NotFound, joinResponse.StatusCode);

            // Check the error message in the response body
            var joinResponseString = await joinResponse.Content.ReadAsStringAsync();
            var errorResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(joinResponseString);
            Assert.Equal("Lobby not found.", errorResponse["message"]);
        }

        // Test to remove a player from the lobby
        [Fact]
        public async Task RemovePlayerFromLobby_ReturnsOk_WhenPlayerIsRemovedSuccessfully()
        {
            // Step 1: Create a player
            var playerId = await CreatePlayerAsync($"Player_{Guid.NewGuid()}");

            // Step 2: Create a lobby
            var createdLobby = await CreateLobbyAsync($"Lobby_{Guid.NewGuid()}");
            var lobbyId = createdLobby.LobbyId;

            // Step 3: Join the player to the lobby
            await _lobbyClient.PostAsync($"/api/lobby/{lobbyId}/join?playerId={playerId}", null);

            // Step 4: Remove the player from the lobby
            var removeResponse = await _lobbyClient.PostAsync($"/api/lobby/{lobbyId}/remove?playerId={playerId}", null);
            removeResponse.EnsureSuccessStatusCode();

            // Step 5: Assert the player has been removed from the lobby
            var lobbyResponse = await _lobbyClient.GetAsync($"/api/lobby/{lobbyId}");
            lobbyResponse.EnsureSuccessStatusCode();

            var lobbyResponseString = await lobbyResponse.Content.ReadAsStringAsync();
            var updatedLobby = JsonSerializer.Deserialize<Lobby>(lobbyResponseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.DoesNotContain(playerId, updatedLobby.Players);
        }

        // Test to handle case when player does not exist
        [Fact]
        public async Task JoinLobby_ReturnsNotFound_WhenPlayerDoesNotExist()
        {
            // Step 1: Create a lobby
            var createdLobby = await CreateLobbyAsync($"Lobby_{Guid.NewGuid()}");
            var lobbyId = createdLobby.LobbyId;

            // Step 2: Try to join the lobby with a non-existent player ID
            var nonExistentPlayerId = "non-existent-player";
            var joinResponse = await _lobbyClient.PostAsync($"/api/lobby/{lobbyId}/join?playerId={nonExistentPlayerId}", null);

            // Assert: The response should be 404 Not Found
            Assert.Equal(System.Net.HttpStatusCode.NotFound, joinResponse.StatusCode);

            // Check the error message in the response body
            var joinResponseString = await joinResponse.Content.ReadAsStringAsync();
            var errorResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(joinResponseString);
            Assert.Equal("Player does not exist.", errorResponse["message"]);
        }

        // Test to check for duplicate lobby names
        [Fact]
        public async Task CreateLobby_ReturnsBadRequest_WhenLobbyNameAlreadyExists()
        {
            // Step 1: Create a unique lobby
            var lobbyName = $"DuplicateLobby_{Guid.NewGuid()}";
            await CreateLobbyAsync(lobbyName);

            // Step 2: Try to create another lobby with the same name
            var duplicateCreateResponse = await _lobbyClient.PostAsync($"/api/lobby?lobbyName={lobbyName}", null);

            // Assert: The response should be 400 Bad Request
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, duplicateCreateResponse.StatusCode);

            // Check the error message in the response body
            var errorResponseString = await duplicateCreateResponse.Content.ReadAsStringAsync();
            var errorResponse = JsonSerializer.Deserialize<Dictionary<string, string>>(errorResponseString);
            Assert.Equal("Lobby name already exists.", errorResponse["message"]);
        }
    }
}
