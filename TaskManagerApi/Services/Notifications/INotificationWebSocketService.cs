
using System.Net.WebSockets;

namespace TaskManagerApi.Services.Notifications
{
    public interface INotificationWebSocketService
    {
        Task ExecuteSendingAsync(WebSocket webSocket);
    }
}