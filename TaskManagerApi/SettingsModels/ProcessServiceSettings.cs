namespace TaskManagerApi.SettingsModels
{
    /// <summary>
    /// Options for process service.
    /// </summary>
    public class ProcessServiceSettings
    {
        /// <summary>
        /// Life time of connections.
        /// </summary>
        public int? ConnectionLifetimeMilliseconds { get; set; }

        /// <summary>
        /// Interval for sending data to clients.
        /// </summary>
        public int? SendDataMillisecondsInterval { get; set; }
    }
}
