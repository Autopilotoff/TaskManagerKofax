using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace TaskManagerApi.Services.ConnectionManager
{
    /// <summary>
    /// Connection manager.
    /// </summary>
    public interface IWebSocketsManager
    {
        /// <summary>
        /// Updating the list of connections.
        /// </summary>
        void UpdateConnections();

        /// <summary>
        /// Adding a connection.
        /// </summary>
        /// <param name="token">Client token.</param>
        /// <param name="socket">.</param>
        /// <returns>Sign of successful addition.</returns>
        bool AddSocket(string token, WebSocket socket);

        /// <summary>
        /// Updating of the connection lifetime.
        /// </summary>
        /// <param name="token">Client token.</param>
        /// <returns>Sign of successful update.</returns>
        bool TryUpdateWebSocketLifeTime(string token);

        /// <summary>
        /// Read-only list of connections.
        /// </summary>
        ConcurrentDictionary<string, WebSocket> Sockets { get; }
    }
}