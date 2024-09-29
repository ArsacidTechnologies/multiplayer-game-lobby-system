using MassTransit;
using Microsoft.AspNetCore.SignalR;
using notification_service.Hubs;
using SharedUtils.Events;
using System.Threading.Tasks;

namespace notification_service.Consumers
{
    public class PlayerJoinedLobbyEventConsumer : IConsumer<PlayerJoinedLobbyEvent>
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public PlayerJoinedLobbyEventConsumer(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<PlayerJoinedLobbyEvent> context)
        {
            var message = $"Player {context.Message.PlayerId} joined lobby {context.Message.LobbyName}.";
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}
