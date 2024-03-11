using System.Net.WebSockets;

namespace TaskManagerApi.Services.Notifications
{
    /// <summary>
    /// A service for sending notifications to webSockets.
    /// </summary>
    public interface ISingletonNotificationWebSocketService : IWebSocketService
    {
    }
}