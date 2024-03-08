using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using TaskManagerApi.Services.Notifications.Counters;

namespace TaskManagerApi.Services.Notifications
{
    public class NotificationWebSocketService
    {
        private const int CancellationTimeOut = 30000;

        private readonly ConcurrentQueue<string> _messagesQueue;
        private readonly List<Counter> _counters;

        private readonly WebSocket _webSocket;
        private readonly ILogger _logger;

        public NotificationWebSocketService(WebSocket webSocket, ILogger logger)
        {
            _counters = new List<Counter> { new CpuCounter(), new MemoryCounter() };
            _messagesQueue = new ConcurrentQueue<string>();
            _logger = logger;
            _webSocket = webSocket;
        }

        public async Task ExecuteSendingAsync()
        {
            await StartWatchingAsync();
            try
            {
                await EnableSendingAsync();
            }
            finally
            {
                await StopWatchingAsync();
            }
        }

        private async Task EnableSendingAsync()
        {
            var buffer = new byte[1024 * 4];

            var cancellationTokenSource = new CancellationTokenSource(CancellationTimeOut);
            var receiveTask = _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationTokenSource.Token);
            while (receiveTask.IsCompleted == false || !receiveTask.Result.CloseStatus.HasValue)
            {
                if (receiveTask.IsCompleted)
                {
                    cancellationTokenSource = new CancellationTokenSource(CancellationTimeOut);
                    receiveTask = _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationTokenSource.Token);
                }

                if (_messagesQueue.TryDequeue(out var message))
                {
                    await NotifyAsync(message);
                }
                else
                {
                    Thread.Sleep(Counter.CountMillisecondsTimeout);
                }
            }

            await _webSocket.CloseAsync(receiveTask.Result.CloseStatus.Value, receiveTask.Result.CloseStatusDescription, CancellationToken.None);
        }

        private async Task NotifyAsync(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            await _webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task StartWatchingAsync()
        {
            foreach (var couter in _counters)
            {
                Task.Run(() => couter.StartCountAsync(this.Notify));
            }
        }

        public async Task StopWatchingAsync()
        {
            foreach (var couter in _counters)
            {
                couter.StopCountAsync();
            }
        }

        private void Notify(string message)
        {
            _messagesQueue.Enqueue(message);
        }
    }
}
