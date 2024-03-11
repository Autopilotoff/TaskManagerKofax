using System.Diagnostics;

namespace TaskManagerApi.Facades
{
    /// <inheritdoc />
    public class PerformanceCounterFacade : IPerformanceCounterFacade
    {
        /// <param name="category">Counter category name.</param>
        /// <param name="name">Counter name.</param>
        /// <param name="instanceName">Instance name.</param>
        public PerformanceCounterFacade(
            string? category,
            string? name,
            string? instanceName)
        {
            var counter = new PerformanceCounter(category, name, instanceName);
            Name = name;
            GetCounterNextValues = counter.NextValue;
        }

        /// <inheritdoc />
        public string? Name { get; }

        /// <inheritdoc />
        public Func<float> GetCounterNextValues { get; init; }
    }
}
