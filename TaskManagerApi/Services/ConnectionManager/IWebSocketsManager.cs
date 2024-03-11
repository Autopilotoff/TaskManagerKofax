using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace TaskManagerApi.Services.ConnectionManager
{
    public interface IWebSocketsManager
    {
        void ActualizeConnections();
        Task<bool> AddSocketAsync(string token, WebSocket socket);
        Task<bool> TryUpdateWebSocketLifeTimeAsync(string token);

        ConcurrentDictionary<string, WebSocket> Sockets { get; }
    }
}