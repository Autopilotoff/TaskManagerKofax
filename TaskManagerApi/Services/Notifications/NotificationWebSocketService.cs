using System.Net.WebSockets;
using System.Text;
using TaskManagerApi.Services.Notifications.Counters;

namespace TaskManagerApi.Services.Notifications
{
    public class NotificationWebSocketService : WebSocketService
    {
        private bool IsObservationRunning = false;
        private readonly Queue<string> _messagesQueue;
        private readonly List<Counter> _counters;

        public NotificationWebSocketService(WebSocket webSocket, ILogger logger) : base(webSocket, logger)
        {
            _counters = new List<Counter> { new CpuCounter(), new MemoryCounter() };
        }

        public override async Task ExecuteSendingAsync()
        {
            await StartObserveAsync();
            await ExecuteAsync();
            await StopObserveAsync();
        }

        protected override async Task NotifyAsync()
        {
            while (IsObservationRunning)
            {
                if (_messagesQueue.Count > 0)
                {
                    var message = _messagesQueue.Dequeue();
                    var bytes = Encoding.UTF8.GetBytes(message);
                    var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                    await _webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
                    return;
                }
                else
                {
                    Thread.Sleep(Counter.CountMillisecondsTimeout);
                }
            }
        }

        public async Task StartObserveAsync()
        {
            IsObservationRunning = true;
            foreach (var couter in _counters)
            {
                couter.StartCount(this.Notify);
            }
        }

        public async Task StopObserveAsync()
        {
            IsObservationRunning = false;
            foreach (var couter in _counters.AsParallel())
            {
                couter.StopCount();
            }
        }

        private void Notify(string message)
        {
            _messagesQueue.Enqueue(message);
        }
    }
}
