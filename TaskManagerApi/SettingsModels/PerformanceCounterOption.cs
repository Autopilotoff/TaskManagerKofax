namespace TaskManagerApi.SettingsModels
{
    /// <summary>
    /// Performance counter options.
    /// </summary>
    public class PerformanceCounterOption
    {
        /// <summary>
        /// Category  name.
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Counter name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Instance name.
        /// </summary>
        public string? InstanceName { get; set; }

        /// <summary>
        /// Upper value limit.
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Check interval.
        /// </summary>
        public int? CheckIntervalMilliseconds { get; set; }

        /// <summary>
        /// Pause after detection.
        /// </summary>
        public int? PauseAfterDetectionMilliseconds { get; set; }
    }
}
