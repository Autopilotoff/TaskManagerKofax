
using System.Net.WebSockets;

namespace TaskManagerApi.Services
{
    public interface IProcessesWebSocketService
    {
        Task ExecuteSendingAsync(WebSocket webSocket);
    }
}