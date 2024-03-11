using TaskManagerApi.Facades;

namespace TaskManagerApi.Services.Notifications
{
    /// <summary>
    /// Performance counter monitor.
    /// </summary>
    public class PerformanceMonitor
    {
        private readonly IPerformanceCounterFacade _counterFacade;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private int Limit { get; }

        private string Alert { get; }

        internal int CheckIntervalMilliseconds { get; init; } = 500;

        internal int PauseAfterDetectionMilliseconds { get; init; } = 500;


        /// <param name="counterFacade">Performance counter.</param>
        /// <param name="limit">Upper value limit.</param>
        /// <param name="checkIntervalMilliseconds">Check interval in milliseconds.</param>
        /// <param name="pauseAfterDetectionMilliseconds">Pause after detection in milliseconds.</param>
        public PerformanceMonitor(
            IPerformanceCounterFacade counterFacade,
            int limit,
            int? checkIntervalMilliseconds,
            int? pauseAfterDetectionMilliseconds)
        {
            _counterFacade = counterFacade;
            Limit = limit;
            Alert = $"{_counterFacade.Name} more than {limit}";
            CheckIntervalMilliseconds = checkIntervalMilliseconds ?? CheckIntervalMilliseconds;
            PauseAfterDetectionMilliseconds = pauseAfterDetectionMilliseconds ?? PauseAfterDetectionMilliseconds;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Starting of monitoring the performance counter.
        /// </summary>
        /// <param name="notify">Notification action.</param>
        public void StartMonitoring(Action<string> notify)
        {
            Task.Run(() =>
            {
                _counterFacade.GetCounterNextValues();
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    if (_counterFacade.GetCounterNextValues() > Limit)
                    {
                        notify(Alert);
                        Thread.Sleep(PauseAfterDetectionMilliseconds);
                    }

                    Thread.Sleep(CheckIntervalMilliseconds);
                }
            }, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Stop monitoring the performance counter.
        /// </summary>
        public void StopMonitoring()
        {
            _cancellationTokenSource.Cancel(true);
        }

        /// <summary>
        /// Sign of the monitoring cancellation.
        /// </summary>
        public bool IsCancellationRequested => _cancellationTokenSource.IsCancellationRequested;
    }
}
