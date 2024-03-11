using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace TaskManagerApi.Services
{
    public class WebSocketsManager : IWebSocketsManager
    {        
        private readonly ConcurrentDictionary<string, DateTime> _expairedLimits;
        private readonly ILogger _logger;
        private int CancellationMillisecondsTimeOut { get; } = 10000;

        public ConcurrentDictionary<string, WebSocket> Sockets { get; private set; }

        public WebSocketsManager(ILogger logger, int? cancellationMillisecondsTimeOut)
        {
            _logger = logger;

            Sockets = new ConcurrentDictionary<string, WebSocket>();
            _expairedLimits = new ConcurrentDictionary<string, DateTime>();

            CancellationMillisecondsTimeOut = cancellationMillisecondsTimeOut ?? CancellationMillisecondsTimeOut;
        }

        public async Task<bool> TryUpdateWebSocketLifeTimeAsync(string token)
        {
            if (_expairedLimits.ContainsKey(token))
            {
                _expairedLimits[token] = GetLimit();
                return true;
            }

            return false;
        }

        public async Task<bool> AddSocketAsync(string token, WebSocket socket)
        {
            var isAdded = Sockets.TryAdd(token, socket)
                && _expairedLimits.TryAdd(token, GetLimit());
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

        public void ActualizeConnections()
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

        private DateTime GetLimit() => DateTime.Now.AddMilliseconds(CancellationMillisecondsTimeOut);

    }
}
