namespace TaskManagerApi.Services.Notifications.Counters
{
    public abstract class Counter
    {
        internal const int CountMillisecondsTimeout = 500;

        protected Func<object, bool> ConditionPredicate { get; init; }

        protected string Alert { get; init; }

        private bool IsCountRunning { get; set; }

        public async Task StartCountAsync(Action<string> notify)
        {
            IsCountRunning = true;
            GetCounterValue();
            while (IsCountRunning)
            {
                if (ConditionPredicate(GetCounterValue()))
                {
                    notify(Alert);
                }

                Thread.Sleep(CountMillisecondsTimeout);
            }
        }

        public async Task StopCountAsync()
        {
            IsCountRunning = false;
        }

        public abstract object GetCounterValue();
    }
}
