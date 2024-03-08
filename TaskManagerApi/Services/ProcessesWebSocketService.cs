using System.Net.WebSockets;
using System.Text.Json;
using System.Text;

namespace TaskManagerApi.Services
{
    public class ProcessesWebSocketService
    {
        private const int CancellationTimeOut = 5000;

        private readonly ProcessesService _processesService;
        private readonly ProcessesStorage _processStorage;

        private readonly WebSocket _webSocket;
        private readonly ILogger _logger;

        public ProcessesWebSocketService(WebSocket webSocket, ILogger logger)
        {
            _processesService = new ProcessesService();
            _processStorage = new ProcessesStorage();
            _logger = logger;
            _webSocket = webSocket;
        }

        public async Task ExecuteSendingAsync()
        {
            var buffer = new byte[1024 * 4];
            try
            {
                var receiveResult = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), new CancellationTokenSource(CancellationTimeOut).Token);

                while (!receiveResult.CloseStatus.HasValue)
                {
                    await NotifyAsync();
                    receiveResult = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), new CancellationTokenSource(CancellationTimeOut).Token);
                }

                await _webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
            }
            catch (OperationCanceledException e)
            {
                _logger.LogError(exception: e, e.Message);
            }
        }

        private async Task NotifyAsync()
        {
            var data = await _processStorage.GetChangesAsync(_processesService.GetCurrentProcesses());
            var message = JsonSerializer.Serialize(data);
            var bytes = Encoding.UTF8.GetBytes(message);
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);

            await _webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
