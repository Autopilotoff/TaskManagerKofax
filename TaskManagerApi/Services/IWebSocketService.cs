using System.Net.WebSockets;

namespace TaskManagerApi.Services
{
    public interface IWebSocketService
    {
        int CheckMillisecondsInterval { get; }

        Task AddSocketAsync(string token, WebSocket socket);
        Task<bool> TryUpdateWebSocketLifeTimeAsync(string token);
    }
}