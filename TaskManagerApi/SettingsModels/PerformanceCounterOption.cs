namespace TaskManagerApi.SettingsModels
{
    public class PerformanceCounterOption
    {
        public string? Category { get; set; }
        public string? Name { get; set; }
        public string? InstanceName { get; set; }
        public int Limit { get; set; }

        public int? CountMillisecondsTimeout { get; set; }

        public int? PauseAfterNotifyMilliseconds { get; set; }
    }
}
