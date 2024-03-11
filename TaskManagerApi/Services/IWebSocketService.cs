using System.Net.WebSockets;

namespace TaskManagerApi.Services
{
    /// <summary>
    /// A service for working with webSockets.
    /// </summary>
    public interface IWebSocketService
    {
        /// <summary>
        /// Sending data interval.
        /// </summary>
        int SendDataMillisecondsInterval { get; }

        /// <summary>
        /// Adding a connection.
        /// </summary>
        /// <param name="token">Client token.</param>
        /// <param name="socket">.</param>
        /// <returns>Sign of successful addition.</returns>
        Task AddSocketAsync(string token, WebSocket socket);

        /// <summary>
        /// Updating of the connection lifetime.
        /// </summary>
        /// <param name="token">Client token.</param>
        /// <returns>Sign of successful update.</returns>
        Task<bool> TryUpdateWebSocketLifeTimeAsync(string token);
    }
}