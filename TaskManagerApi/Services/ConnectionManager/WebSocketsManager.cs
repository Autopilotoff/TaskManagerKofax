using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace TaskManagerApi.Services.ConnectionManager
{
    /// <inheritdoc />
    public class WebSocketsManager : IWebSocketsManager
    {
        private readonly ConcurrentDictionary<string, DateTime> _expairedLimits;
        private readonly ILogger _logger;
        private int LifetimeMilliseconds { get; } = 10000;

        /// <inheritdoc />
        public ConcurrentDictionary<string, WebSocket> Sockets { get; private set; }

        /// <param name="logger">External logger.</param>
        /// <param name="lifetimeMilliseconds">Life time of the connections.</param>
        public WebSocketsManager(ILogger logger, int? lifetimeMilliseconds)
        {
            _logger = logger;

            Sockets = new ConcurrentDictionary<string, WebSocket>();
            _expairedLimits = new ConcurrentDictionary<string, DateTime>();

            LifetimeMilliseconds = lifetimeMilliseconds ?? LifetimeMilliseconds;
        }

        /// <inheritdoc />
        public bool TryUpdateWebSocketLifeTime(string token)
        {
            if (_expairedLimits.ContainsKey(token))
            {
                _expairedLimits[token] = GetEndDateTime();
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public bool AddSocket(string token, WebSocket socket)
        {
            var isAdded = Sockets.TryAdd(token, socket)
                && _expairedLimits.TryAdd(token, GetEndDateTime());
            if (isAdded)
            {
                _logger.LogInformation($"{token} websocket started...");
            }
            else
            {
                _logger.LogError($"{token} websocket has't started.");
            }

            return isAdded;
        }

        /// <inheritdoc />
        public void UpdateConnections()
        {
            var tokens = _expairedLimits
                .Where(x => x.Value < DateTime.Now)
                .Select(x => x.Key)
                .ToList();

            foreach (var token in tokens)
            {
                Sockets.Remove(token, out var webSocket);
                if (webSocket != null)
                {
                    _logger.LogInformation($"{token} websocket is closing...");
                    var task = webSocket.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, nameof(WebSocketsManager), CancellationToken.None);
                    task.Wait();
                    task.Dispose();
                    _logger.LogInformation($"{token} websocket closed.");
                }

                _expairedLimits.Remove(token, out var time);
            }
        }

        private DateTime GetEndDateTime() => DateTime.Now.AddMilliseconds(LifetimeMilliseconds);
    }
}
