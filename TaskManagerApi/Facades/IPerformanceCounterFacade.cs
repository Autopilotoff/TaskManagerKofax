
namespace TaskManagerApi.Facades
{
    /// <summary>
    /// Performance counter.
    /// </summary>
    public interface IPerformanceCounterFacade
    {
        /// <summary>
        /// Get value function.
        /// </summary>
        Func<float> GetCounterNextValues { get; init; }

        /// <summary>
        /// Counter name.
        /// </summary>
        string? Name { get; }
    }
}