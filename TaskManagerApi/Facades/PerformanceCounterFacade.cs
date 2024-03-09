using System.Diagnostics;

namespace TaskManagerApi.Facades
{
    public class PerformanceCounterFacade : IPerformanceCounterFacade
    {
        public PerformanceCounterFacade(
            string? category,
            string? name,
            string? instanceName)
        {
            var counter = new PerformanceCounter(category, name, instanceName);
            Name = name;
            GetCounterNextValues = counter.NextValue;
        }

        public string? Name { get; }

        public Func<float> GetCounterNextValues { get; init; }
    }
}
