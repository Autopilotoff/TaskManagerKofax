using System.Diagnostics;

namespace TaskManagerApi.Services.Notifications
{
    public class Counter
    {
        private readonly PerformanceCounter _counter;

        private int Limit { get; }

        private string Alert { get; }

        private bool IsCountRunning { get; set; }

        internal int CountMillisecondsTimeout { get; init; } = 500;

        internal int PauseAfterNotifyMilliseconds { get; init; } = 500;


        public Counter(
            int limit,
            string? category,
            string? name,
            string? instanceName,
            int? countMillisecondsTimeout,
            int? pauseAfterNotifyMilliseconds)
        {
            _counter = new PerformanceCounter(category, name, instanceName);
            Limit = limit;
            Alert = $"{name} more than {limit}";
            CountMillisecondsTimeout = countMillisecondsTimeout ?? CountMillisecondsTimeout;
            PauseAfterNotifyMilliseconds = pauseAfterNotifyMilliseconds ?? PauseAfterNotifyMilliseconds;
        }

        public async Task StartCountAsync(Action<string> notify)
        {
            IsCountRunning = true;
            _counter.NextValue();
            while (IsCountRunning)
            {
                if (_counter.NextValue() > Limit)
                {
                    notify(Alert);
                    Thread.Sleep(PauseAfterNotifyMilliseconds);
                }

                Thread.Sleep(CountMillisecondsTimeout);
            }
        }

        public async Task StopCountAsync()
        {
            IsCountRunning = false;
        }
    }
}
