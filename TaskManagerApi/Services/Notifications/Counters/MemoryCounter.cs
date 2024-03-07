using System.Diagnostics;

namespace TaskManagerApi.Services.Notifications.Counters
{
    public class MemoryCounter : Counter
    {
        private readonly PerformanceCounter _counter;

        public MemoryCounter()
        {
            _counter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            ConditionPredicate = value => (float)value > 90;
            Alert = "% Committed Bytes In Use > 90";
        }

        public override object GetCounterValue()
        {
            return _counter.NextValue();
        }
    }
}
