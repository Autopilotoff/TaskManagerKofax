using TaskManagerApi.Models;

namespace TaskManagerApi.Proxies
{
    /// <summary>
    /// Current processes source.
    /// </summary>
    public interface IProcessesProxy
    {
        /// <summary>
        /// Getting current processes.
        /// </summary>
        /// <returns>Collection of <see cref="ProcessModel"/>.</returns>
        ProcessModel[] GetCurrentProcesses();
    }
}