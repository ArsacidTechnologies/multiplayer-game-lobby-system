using lobby_service.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace lobby_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LobbyController : ControllerBase
    {
        private readonly ILobbyService _lobbyService;

        public LobbyController(ILobbyService lobbyService)
        {
            _lobbyService = lobbyService;
        }

        // POST api/lobby
        [HttpPost]
        public async Task<IActionResult> CreateLobby([FromQuery] string lobbyName)
        {
            var lobby = await _lobbyService.CreateLobbyAsync(lobbyName);
            return Ok(lobby);
        }

        // POST api/lobby/{lobbyId}/join
        [HttpPost("{lobbyId}/join")]
        public async Task<IActionResult> JoinLobby(string lobbyId, [FromQuery] string playerId)
        {
            try
            {
                var lobby = await _lobbyService.JoinLobbyAsync(lobbyId, playerId);
                return Ok(lobby);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        // GET api/lobby/{lobbyId}
        [HttpGet("{lobbyId}")]
        public async Task<IActionResult> GetLobbieAsync(string lobbyId)
        {
            try
            {
                var lobby = await _lobbyService.GetLobbieAsync(lobbyId);
                return Ok(lobby);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        // GET api/lobby
        [HttpGet]
        public async Task<IActionResult> GetLobbies()
        {
            var lobbies = await _lobbyService.GetLobbiesAsync();
            return Ok(lobbies);
        }

    }
}