using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using TaskManagerApi.Facades;
using TaskManagerApi.SettingsModels;

namespace TaskManagerApi.Services.Notifications
{
    public class SingletonNotificationWebSocketService : ISingletonNotificationWebSocketService
    {
        private readonly ILogger<ISingletonNotificationWebSocketService> _logger;
        private readonly ConcurrentQueue<string> _messagesQueue;
        private readonly List<PerformanceWatcher> _watchers;

        private readonly IWebSocketsManager _webSocketsManager;
        private readonly Timer _sendingTimer;

        private int CancellationMillisecondsTimeOut { get; } = 30000;

        public int CheckMillisecondsInterval { get; } = 500;


        public SingletonNotificationWebSocketService(
            ILogger<SingletonNotificationWebSocketService> logger,
            IOptions<PerformanceCounterSettings> options,
            IOptions<NotificationSettings> notificationSettings
            )
        {
            _logger = logger;

            _messagesQueue = new ConcurrentQueue<string>();
            _watchers = CreateWatchers(options);

            CancellationMillisecondsTimeOut = notificationSettings?.Value?.CancellationMillisecondsTimeOut ?? CancellationMillisecondsTimeOut;
            _webSocketsManager = new WebSocketsManager(logger, CancellationMillisecondsTimeOut);

            CheckMillisecondsInterval = notificationSettings?.Value?.CheckMillisecondsInterval ?? CheckMillisecondsInterval;
            _sendingTimer = new Timer(callback: Sending, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(CheckMillisecondsInterval));

            StartWatching();
        }

        ~SingletonNotificationWebSocketService()
        {
            StopWatching();
            _sendingTimer.Dispose();
        }

        public Task AddSocketAsync(string token, WebSocket socket)
        {
            return _webSocketsManager.AddSocketAsync(token, socket);
        }

        public Task<bool> TryUpdateWebSocketLifeTimeAsync(string token)
        {
            return _webSocketsManager.TryUpdateWebSocketLifeTimeAsync(token);
        }

        private void Sending(object? state)
        {
            _webSocketsManager.ActualizeConnections();

            if (_webSocketsManager.Sockets.Count == 0)
            {
                _messagesQueue.Clear();
                return;
            }

            if (_messagesQueue.Count == 0)
            {
                return;
            }

            var messageList = new List<string>();
            while (_messagesQueue.TryDequeue(out var message))
            {
                messageList.Add(message);
            }

            var data = ConvertMessageToByteArray(messageList);

            foreach (var socket in _webSocketsManager.Sockets)
            {
                var task = NotifyAsync(socket.Value, data);
                task.Wait();
                task.Dispose();
            }
        }

        private async Task NotifyAsync(WebSocket webSocket, ArraySegment<byte> arraySegment)
        {
            await webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private ArraySegment<byte> ConvertMessageToByteArray(IEnumerable<string> messages)
        {
            var message = JsonSerializer.Serialize(messages);
            var bytes = Encoding.UTF8.GetBytes(message);
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
            return arraySegment;
        }

        private void StartWatching()
        {
            _logger.LogInformation("Watching started...");

            foreach (var watcher in _watchers)
            {
                watcher.StartWatching(this.EnqueueMessage);
            }
        }

        private void StopWatching()
        {
            _watchers.ForEach(counter => counter.StopWatching());
            _logger.LogInformation("...Watching stopped.");
        }

        private void EnqueueMessage(string message)
        {
            _messagesQueue.Enqueue(message);
        }

        private List<PerformanceWatcher> CreateWatchers(IOptions<PerformanceCounterSettings> options)
        {
            return options?.Value?.CounterOptions?
                .Select(x => CreateWatcher(x))
                .ToList()
                 ?? throw new ArgumentNullException(nameof(options));
        }

        private PerformanceWatcher CreateWatcher(PerformanceCounterOption option)
        {
            return new PerformanceWatcher(
                new PerformanceCounterFacade(option.Category, option.Name, option.InstanceName),
                option.Limit,
                option.CountMillisecondsTimeout,
                option.PauseAfterNotifyMilliseconds);
        }
    }
}
