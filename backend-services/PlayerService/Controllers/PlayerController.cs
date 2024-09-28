using Microsoft.AspNetCore.Mvc;
using player_service.Services;

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
        public IActionResult CreatePlayer([FromQuery] string playerName)
        {
            var player = _playerService.CreatePlayer(playerName);
            return Ok(player);
        }

        // GET api/player
        [HttpGet]
        public IActionResult GetPlayers()
        {
            var players = _playerService.GetPlayers();
            return Ok(players);
        }
    }
}