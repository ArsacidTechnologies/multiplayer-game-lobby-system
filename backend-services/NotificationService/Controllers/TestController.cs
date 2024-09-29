using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using notification_service.Hubs;

namespace notification_service.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendNotification([FromQuery] string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
            return Ok(new { Status = "Notification sent successfully" });
        }
    }
}
