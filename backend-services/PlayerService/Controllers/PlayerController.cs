using Microsoft.AspNetCore.Mvc;
using player_service.Services;
using System.Threading.Tasks;
using player_service.DTOs;

namespace player_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService _playerService;

        public PlayerController(IPlayerService playerService)
        {
            _playerService = playerService;
        }

        // POST api/player
        [HttpPost]
        public async Task<IActionResult> CreatePlayer([FromQuery] PlayerDto playerDto)
        {
            try
            {
                var player = await _playerService.CreatePlayerAsync(playerDto.PlayerName);
                return Ok(player);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        // GET api/player
        [HttpGet]
        public async Task<IActionResult> GetPlayers()
        {
            var players = await _playerService.GetPlayersAsync();
            return Ok(players);
        }
    }
}
