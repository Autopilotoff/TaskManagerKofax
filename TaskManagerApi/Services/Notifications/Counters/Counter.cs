namespace TaskManagerApi.Services.Notifications.Counters
{
    public abstract class Counter
    {
        internal const int CountMillisecondsTimeout = 1000;

        protected Func<object, bool> ConditionPredicate { get; init; }

        protected string Alert { get; init; }

        private bool IsCountRunning { get; set; }

        public void StartCount(Action<string> notify)
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

        public void StopCount()
        {
            IsCountRunning = false;
        }

        public abstract object GetCounterValue();
    }
}
