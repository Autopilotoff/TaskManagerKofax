using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using TaskManagerApi.Facades;
using TaskManagerApi.Services.ConnectionManager;
using TaskManagerApi.SettingsModels;

namespace TaskManagerApi.Services.Notifications
{
    /// <inheritdoc />
    public class SingletonNotificationWebSocketService : ISingletonNotificationWebSocketService
    {
        private readonly ILogger<SingletonNotificationWebSocketService> _logger;
        private readonly ConcurrentQueue<string> _messagesQueue;
        private readonly List<PerformanceMonitor> _monitors;

        private readonly IWebSocketsManager _webSocketsManager;
        private readonly Timer _sendingTimer;

        private int ConnectionLifetimeMilliseconds { get; } = 30000;
        
        /// <inheritdoc />
        public int SendDataMillisecondsInterval { get; } = 500;

        /// <param name="logger">.</param>
        /// <param name="performanceCountersOptions"><see cref="PerformanceCounterSettings"/></param>
        /// <param name="notificationSettings"><see cref="NotificationServiceSettings"/></param>
        public SingletonNotificationWebSocketService(
            ILogger<SingletonNotificationWebSocketService> logger,
            IOptions<PerformanceCounterSettings> performanceCountersOptions,
            IOptions<NotificationServiceSettings> notificationSettings
            )
        {
            _logger = logger;

            _messagesQueue = new ConcurrentQueue<string>();
            _monitors = CreateMonitors(performanceCountersOptions);

            ConnectionLifetimeMilliseconds = notificationSettings?.Value?.ConnectionLifetimeMilliseconds ?? ConnectionLifetimeMilliseconds;
            _webSocketsManager = new WebSocketsManager(logger, ConnectionLifetimeMilliseconds);

            SendDataMillisecondsInterval = notificationSettings?.Value?.SendDataMillisecondsInterval ?? SendDataMillisecondsInterval;
            _sendingTimer = new Timer(callback: SendNotifications, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(SendDataMillisecondsInterval));

            StartMonitoring();
        }

        ~SingletonNotificationWebSocketService()
        {
            StopMonitoring();
            _sendingTimer.Dispose();
        }

        /// <inheritdoc />
        public Task AddSocketAsync(string token, WebSocket socket)
        {
            return Task.FromResult(_webSocketsManager.AddSocket(token, socket));
        }

        /// <inheritdoc />
        public Task<bool> TryUpdateWebSocketLifeTimeAsync(string token)
        {
            return Task.FromResult(_webSocketsManager.TryUpdateWebSocketLifeTime(token));
        }

        private void SendNotifications(object? state)
        {
            _webSocketsManager.UpdateConnections();

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
                var task = SendDataAsync(socket.Value, data);
                task.Wait();
                task.Dispose();
            }
        }

        private async Task SendDataAsync(WebSocket webSocket, ArraySegment<byte> arraySegment)
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

        private void StartMonitoring()
        {
            _logger.LogInformation("Monitoring started...");

            foreach (var monitor in _monitors)
            {
                monitor.StartMonitoring(this.EnqueueMessage);
            }
        }

        private void StopMonitoring()
        {
            _monitors.ForEach(counter => counter.StopMonitoring());
            _logger.LogInformation("...Monitoring stopped.");
        }

        private void EnqueueMessage(string message)
        {
            _messagesQueue.Enqueue(message);
        }

        private List<PerformanceMonitor> CreateMonitors(IOptions<PerformanceCounterSettings> options)
        {
            return options?.Value?.CounterOptions?
                .Select(x => CreateMonitor(x))
                .ToList()
                 ?? throw new ArgumentNullException(nameof(options));
        }

        private PerformanceMonitor CreateMonitor(PerformanceCounterOption option)
        {
            return new PerformanceMonitor(
                new PerformanceCounterFacade(option.Category, option.Name, option.InstanceName),
                option.Limit,
                option.CheckIntervalMilliseconds,
                option.PauseAfterDetectionMilliseconds);
        }
    }
}
