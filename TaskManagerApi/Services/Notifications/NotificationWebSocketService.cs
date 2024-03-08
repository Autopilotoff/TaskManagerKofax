using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using TaskManagerApi.SettingsModels;

namespace TaskManagerApi.Services.Notifications
{
    public class NotificationWebSocketService : INotificationWebSocketService
    {
        private readonly ILogger<INotificationWebSocketService> _logger;
        private readonly ConcurrentQueue<string> _messagesQueue;
        private readonly List<Counter> _counters;

        private WebSocket _webSocket;

        private int CancellationMillisecondsTimeOut { get; } = 30000;

        private int CheckMillisecondsTimeout { get; } = 500;

        public NotificationWebSocketService(
            ILogger<INotificationWebSocketService> logger,
            IOptions<PerformanceCounterSettings> options,
            IOptions<NotificationSettings> notificationSettings
            )
        {
            _logger = logger;
            _counters = CreateCounters(options);
            _messagesQueue = new ConcurrentQueue<string>();
            CancellationMillisecondsTimeOut = notificationSettings?.Value?.CancellationMillisecondsTimeOut ?? CancellationMillisecondsTimeOut;
            CheckMillisecondsTimeout = notificationSettings?.Value?.CheckMillisecondsTimeout ?? CheckMillisecondsTimeout;
        }

        public async Task ExecuteSendingAsync(WebSocket webSocket)
        {
            _webSocket = webSocket;

            await StartWatchingAsync();
            _logger.LogInformation("Watching started...");
            try
            {
                await EnableSendingAsync();
            }
            finally
            {
                await StopWatchingAsync();
                _logger.LogInformation("...Watching stoped.");
            }
        }

        private async Task EnableSendingAsync()
        {
            var buffer = new byte[1024 * 4];

            var cancellationTokenSource = new CancellationTokenSource(CancellationMillisecondsTimeOut);
            var receiveTask = _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationTokenSource.Token);
            while (receiveTask.IsCompleted == false || !receiveTask.Result.CloseStatus.HasValue)
            {
                if (receiveTask.IsCompleted)
                {
                    cancellationTokenSource = new CancellationTokenSource(CancellationMillisecondsTimeOut);
                    receiveTask = _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationTokenSource.Token);
                }

                if (_messagesQueue.TryDequeue(out var message))
                {
                    await NotifyAsync(message);
                }
                else
                {
                    Thread.Sleep(CheckMillisecondsTimeout);
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

        private async Task StartWatchingAsync()
        {
            foreach (var couter in _counters)
            {
                Task.Run(() => couter.StartCountAsync(this.Notify));
            }
        }

        private async Task StopWatchingAsync()
        {
            _counters.ForEach(counter => counter.StopCountAsync());
        }

        private void Notify(string message)
        {
            _messagesQueue.Enqueue(message);
        }

        private List<Counter> CreateCounters(IOptions<PerformanceCounterSettings> options)
        {
            return options?.Value?.CounterOptions?
                .Select(x => CreateCounter(x))
                .ToList()
                 ?? throw new ArgumentNullException(nameof(options));
        }

        private Counter CreateCounter(PerformanceCounterOption option)
        {
            return new Counter(
                option.Limit,
                option.Category,
                option.Name,
                option.InstanceName,
                option.CountMillisecondsTimeout,
                option.PauseAfterNotifyMilliseconds);
        }

    }
}
