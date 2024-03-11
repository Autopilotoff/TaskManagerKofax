using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;
using TaskManagerApi.SettingsModels;
using TaskManagerApi.Services.ConnectionManager;

namespace TaskManagerApi.Services.Processes
{
    public class SingletonProcessesWebSocketService : ISingletonProcessesWebSocketService
    {
        private readonly ISingletonProcessesStorage _processesStorage;
        
        private readonly IWebSocketsManager _webSocketsManager;
        private readonly Timer _sendingTimer;
        
        private int CancellationMillisecondsTimeOut { get; } = 10000;

        public int CheckMillisecondsInterval { get; set; } = 2000;

        public SingletonProcessesWebSocketService(
            ISingletonProcessesStorage processesStorage,
            IOptions<ProcessesSettings> processesSettings,
            ILogger<SingletonProcessesWebSocketService> logger
            )
        {
            CheckMillisecondsInterval = processesSettings?.Value?.NotifyMillisecondsDelay ?? CheckMillisecondsInterval;
            CancellationMillisecondsTimeOut = processesSettings?.Value?.CancellationMillisecondsTimeOut ?? CancellationMillisecondsTimeOut;
            
            _processesStorage = processesStorage;
            _webSocketsManager = new WebSocketsManager(logger, CancellationMillisecondsTimeOut);

            _sendingTimer = new Timer(callback: Sending, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(CheckMillisecondsInterval));
        }

        ~SingletonProcessesWebSocketService()
        {
            _sendingTimer.Dispose();
        }

        public Task<bool> TryUpdateWebSocketLifeTimeAsync(string token)
        {
            return _webSocketsManager.TryUpdateWebSocketLifeTimeAsync(token);
        }

        public async Task AddSocketAsync(string token, WebSocket socket)
        {
            await _webSocketsManager.AddSocketAsync(token, socket);
            
            var initialData = _processesStorage.GetInitialProcesses();

            var arraySegment = ConvertDataToArraySegment(initialData);
            await SendDataAsync(socket, arraySegment);
        }

        private void Sending(object? state)
        {
            _webSocketsManager.ActualizeConnections();

            if (_webSocketsManager.Sockets.Count == 0)
            {
                return;
            }

            var data = _processesStorage.GetChanges();

            var arraySegment = ConvertDataToArraySegment(data);
            foreach (var socket in _webSocketsManager.Sockets)
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
    }
}
