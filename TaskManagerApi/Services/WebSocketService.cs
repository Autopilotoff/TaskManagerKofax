using System.Net.WebSockets;

namespace TaskManagerApi.Services
{
    public abstract class WebSocketService
    {
        private const int CancellationTimeOut = 5000;
        private readonly ILogger _logger;
        
        protected WebSocket _webSocket;

        public WebSocketService(WebSocket webSocket, ILogger logger)
        {
            _logger = logger;
            _webSocket = webSocket;
        }

        protected async Task ExecuteAsync()
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

        public abstract Task ExecuteSendingAsync();

        protected abstract Task NotifyAsync();
    }
}
