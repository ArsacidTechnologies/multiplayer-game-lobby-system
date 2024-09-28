using Moq;
using player_service.Models;
using player_service.Repositories;
using player_service.Services;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace player_service.Tests
{
    public class PlayerServiceTests
    {
        private readonly Mock<IPlayerRepository> _mockPlayerRepository;
        private readonly IPlayerService _playerService;

        public PlayerServiceTests()
        {
            _mockPlayerRepository = new Mock<IPlayerRepository>();
            _playerService = new PlayerService(_mockPlayerRepository.Object);
        }

        [Fact]
        public async Task CreatePlayerAsync_WithValidName_ShouldCreatePlayer()
        {
            // Arrange
            var playerName = "Mehran";
            _mockPlayerRepository.Setup(repo => repo.GetPlayersAsync())
                .ReturnsAsync(Enumerable.Empty<Player>());

            // Act
            var player = await _playerService.CreatePlayerAsync(playerName);

            // Assert
            Assert.NotNull(player);
            Assert.Equal(playerName, player.Name);
            _mockPlayerRepository.Verify(repo => repo.AddPlayerAsync(It.IsAny<Player>()), Times.Once);
        }

        [Fact]
        public async Task CreatePlayerAsync_WithDuplicateName_ShouldThrowArgumentException()
        {
            // Arrange
            var playerName = "Mehran";
            var existingPlayers = new[] { new Player { Name = playerName } };
            _mockPlayerRepository.Setup(repo => repo.GetPlayersAsync())
                .ReturnsAsync(existingPlayers);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _playerService.CreatePlayerAsync(playerName));
            Assert.Equal("The player name is already taken.", exception.Message);
            _mockPlayerRepository.Verify(repo => repo.AddPlayerAsync(It.IsAny<Player>()), Times.Never);
        }

        [Fact]
        public async Task CreatePlayerAsync_WithNoName_ShouldGenerateUniqueId()
        {
            // Arrange
            _mockPlayerRepository.Setup(repo => repo.GetPlayersAsync())
                .ReturnsAsync(Enumerable.Empty<Player>());

            // Act
            var player = await _playerService.CreatePlayerAsync();

            // Assert
            Assert.NotNull(player);
            Assert.StartsWith("Guest_", player.Name);
            _mockPlayerRepository.Verify(repo => repo.AddPlayerAsync(It.IsAny<Player>()), Times.Once);
        }

        [Fact]
        public async Task GetPlayersAsync_ShouldReturnListOfPlayers()
        {
            // Arrange
            var players = new[]
            {
                new Player { Name = "Mehran" },
                new Player { Name = "Mehraneh" }
            };
            _mockPlayerRepository.Setup(repo => repo.GetPlayersAsync())
                .ReturnsAsync(players);

            // Act
            var result = await _playerService.GetPlayersAsync();

            // Assert
            Assert.Equal(players.Length, result.Count());
            Assert.Contains(result, p => p.Name == "Mehran");
            Assert.Contains(result, p => p.Name == "Mehraneh");
        }
    }
}
