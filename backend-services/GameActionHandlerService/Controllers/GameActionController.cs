using game_action_handler.Models;
using game_action_handler.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace game_action_handler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameActionController : ControllerBase
    {
        private readonly IGameActionService _gameActionService;

        public GameActionController(IGameActionService gameActionService)
        {
            _gameActionService = gameActionService;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPlayerAction([FromBody] PlayerAction action)
        {
            var result = await _gameActionService.ProcessPlayerActionAsync(action);

            if (result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
    }
}
