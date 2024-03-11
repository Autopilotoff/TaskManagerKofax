namespace TaskManagerApi.SettingsModels
{
    /// <summary>
    /// Options for performance counter list.
    /// </summary>
    public class PerformanceCounterSettings
    {
        /// <summary>
        /// <see cref="PerformanceCounterOption"/> list.
        /// </summary>
        public IEnumerable<PerformanceCounterOption>? CounterOptions { get; set; }
    }
}
