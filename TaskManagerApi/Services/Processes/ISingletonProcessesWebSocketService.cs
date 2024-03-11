using System.Net.WebSockets;

namespace TaskManagerApi.Services.Processes
{
    public interface ISingletonProcessesWebSocketService
    {
        Task AddSocketAsync(string token, WebSocket socket);
        bool TryUpdateWebSocketLifeTime(string token);

        int NotifyMillisecondsDelay { get; set; }
    }
}