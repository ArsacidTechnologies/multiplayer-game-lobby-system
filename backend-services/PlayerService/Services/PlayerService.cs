using player_service.Models;
using player_service.Repositories;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Text;
namespace player_service.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IPlayerRepository _playerRepository;

        public PlayerService(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public Player CreatePlayer(string playerName = null)
        {
            string uniqueId = GenerateUniqueId();

            var player = new Player
            {
                Name = playerName ?? uniqueId
            };

            _playerRepository.AddPlayer(player);
            return player;
        }

        public IEnumerable<Player> GetPlayers()
        {
            return _playerRepository.GetPlayers();
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