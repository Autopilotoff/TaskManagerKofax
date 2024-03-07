using System.Diagnostics;

namespace TaskManagerApi.Services.Notifications.Counters
{
    public class CpuCounter : Counter
    {
        private readonly PerformanceCounter _counter;

        public CpuCounter()
        {
            _counter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ConditionPredicate = value => (float)value > 60;
            Alert = "% Processor Time > 60";
        }

        public override object GetCounterValue()
        {
            return _counter.NextValue();
        }
    }
}
