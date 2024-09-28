using player_service.Models;
using player_service.Repositories;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace player_service.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<Player> CreatePlayerAsync(string playerName = null)
        {
            if (!string.IsNullOrEmpty(playerName))
            {
                var existingPlayers = await _playerRepository.GetPlayersAsync();

                // Check if the player name is already taken
                foreach (var player in existingPlayers)
                {
                    if (player.Name == playerName)
                    {
                        throw new ArgumentException("The player name is already taken.");
                    }
                }
            }

            string uniqueId = GenerateUniqueId();

            var newPlayer = new Player
            {
                Name = playerName ?? $"Guest_{uniqueId}" // Prefix with "Guest_"
                                                                              
            };

            await _playerRepository.AddPlayerAsync(newPlayer);
            return newPlayer;
        }

        public async Task<IEnumerable<Player>> GetPlayersAsync()
        {
            return await _playerRepository.GetPlayersAsync();
        }

        private string GenerateUniqueId()
        {
            var guid = Guid.NewGuid().ToString();
            var utcTicks = DateTime.UtcNow.Ticks.ToString();

            var combinedString = guid + utcTicks;

            //Hash the combined string using SHA256
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Compute the hash
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(combinedString));

                // Convert to Base64 
                return Convert.ToBase64String(bytes).Substring(0, 10); // Taking first 10 chars
            }
        }
    }
}