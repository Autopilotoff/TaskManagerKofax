using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;
using TaskManagerApi.SettingsModels;

namespace TaskManagerApi.Services.Processes
{
    public class SingletonProcessesWebSocketService : ISingletonProcessesWebSocketService
    {
        public int NotifyMillisecondsDelay { get; set; } = 2000;
        
        private int CancellationMillisecondsTimeOut { get; } = 10000;

        private readonly ConcurrentDictionary<string, WebSocket> _sockets;
        private readonly ConcurrentDictionary<string, DateTime> _expairedLimits;
        private Timer _sendingTimer;

        private readonly ISingletonProcessesStorage _processesStorage;

        public SingletonProcessesWebSocketService(
            ISingletonProcessesStorage processesStorage,
            IOptions<ProcessesSettings> processesSettings
            )
        {
            NotifyMillisecondsDelay = processesSettings?.Value?.NotifyMillisecondsDelay ?? NotifyMillisecondsDelay;
            CancellationMillisecondsTimeOut = processesSettings?.Value?.CancellationMillisecondsTimeOut ?? CancellationMillisecondsTimeOut;
            _processesStorage = processesStorage;
            _sockets = new ConcurrentDictionary<string, WebSocket>();
            _expairedLimits = new ConcurrentDictionary<string, DateTime>();
            
            _sendingTimer = new Timer(callback: Sending, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(NotifyMillisecondsDelay));
        }

        public bool TryUpdateWebSocketLifeTime(string token)
        {
            if (_expairedLimits.ContainsKey(token))
            {
                _expairedLimits[token] = GetLimit();
                return true;
            }

            return false;
        }

        public async Task AddSocketAsync(string token, WebSocket socket)
        {
            _sockets.TryAdd(token, socket);
            _expairedLimits.TryAdd(token, GetLimit());

            var initialData = _processesStorage.GetInitialProcesses();

            var arraySegment = ConvertDataToArraySegment(initialData);
            await SendDataAsync(socket, arraySegment);

            Console.WriteLine($"{token} websocket started...");
        }

        private DateTime GetLimit() => DateTime.Now.AddMilliseconds(CancellationMillisecondsTimeOut);

        private void Sending(object? state)
        {
            if (_sockets.Count == 0)
            {
                return;
            }

            ActualizeConnections();

            var data = _processesStorage.GetChanges();

            var arraySegment = ConvertDataToArraySegment(data);
            foreach (var socket in _sockets)
            {
                var task = SendDataAsync(socket.Value, arraySegment);
                task.Wait();
                task.Dispose();
            }
        }

        private async Task SendDataAsync(WebSocket socket, ArraySegment<byte> arraySegment)
        {
            await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private ArraySegment<byte> ConvertDataToArraySegment<T>(T data)
        {
            var message = JsonSerializer.Serialize(data);
            var bytes = Encoding.ASCII.GetBytes(message);
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            return arraySegment;
        }

        private void ActualizeConnections()
        {
            var tokens = _expairedLimits
                .Where(x => x.Value < DateTime.Now)
                .Select(x => x.Key)
                .ToList();

            foreach (var token in tokens)
            {
                _sockets.Remove(token, out var webSocket);
                if (webSocket != null)
                {
                    Console.WriteLine($"{token} websocket is closing...");
                    var task = webSocket.CloseAsync(WebSocketCloseStatus.EndpointUnavailable, nameof(SingletonProcessesWebSocketService), CancellationToken.None);
                    task.Wait();
                    task.Dispose();
                    Console.WriteLine($"{token} websocket closed.");
                }

                _expairedLimits.Remove(token, out var time);
            }
        }
    }
}
