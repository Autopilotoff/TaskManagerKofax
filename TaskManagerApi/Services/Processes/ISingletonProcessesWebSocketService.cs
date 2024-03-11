using System.Net.WebSockets;

namespace TaskManagerApi.Services.Processes
{
    /// <summary>
    /// A service for sending a current processes data to webSockets.
    /// </summary>
    public interface ISingletonProcessesWebSocketService : IWebSocketService
    {
    }
}