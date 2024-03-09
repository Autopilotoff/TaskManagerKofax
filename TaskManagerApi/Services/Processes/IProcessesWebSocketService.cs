
using System.Net.WebSockets;

namespace TaskManagerApi.Services.Processes
{
    public interface IProcessesWebSocketService
    {
        Task ExecuteSendingAsync(WebSocket webSocket);
    }
}