using TaskManagerApi.Facades;

namespace TaskManagerApi.Services.Notifications
{
    public class PerformanceWatcher
    {
        private readonly IPerformanceCounterFacade _counterFacade;
        private readonly CancellationTokenSource _cancellationTokenSource;

        private int Limit { get; }

        private string Alert { get; }

        internal int CountMillisecondsTimeout { get; init; } = 500;

        internal int PauseAfterNotifyMilliseconds { get; init; } = 500;


        public PerformanceWatcher(
            IPerformanceCounterFacade counterFacade,
            int limit,
            int? countMillisecondsTimeout,
            int? pauseAfterNotifyMilliseconds)
        {
            _counterFacade = counterFacade;
            Limit = limit;
            Alert = $"{_counterFacade.Name} more than {limit}";
            CountMillisecondsTimeout = countMillisecondsTimeout ?? CountMillisecondsTimeout;
            PauseAfterNotifyMilliseconds = pauseAfterNotifyMilliseconds ?? PauseAfterNotifyMilliseconds;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public Task StartWatchingAsync(Action<string> notify)
        {
            Task.Run(() =>
            {
                _counterFacade.GetCounterNextValues();
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    if (_counterFacade.GetCounterNextValues() > Limit)
                    {
                        notify(Alert);
                        Thread.Sleep(PauseAfterNotifyMilliseconds);
                    }

                    Thread.Sleep(CountMillisecondsTimeout);
                }
            }, _cancellationTokenSource.Token);

            return Task.CompletedTask;
        }

        public Task StopWatchingAsync()
        {
            _cancellationTokenSource.Cancel(true);
            return Task.CompletedTask;
        }

        public bool IsCancellationRequested => _cancellationTokenSource.IsCancellationRequested;
    }
}
