using System.Net.WebSockets;

namespace TaskManagerApi.Services.Processes
{
    public interface ISingletonProcessesWebSocketService : IWebSocketService
    {
        Task AddSocketAsync(string token, WebSocket socket);
        
        Task<bool> TryUpdateWebSocketLifeTimeAsync(string token);

        int CheckMillisecondsInterval { get; set; }
    }
}