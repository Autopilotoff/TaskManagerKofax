using System.Diagnostics;
using TaskManagerApi.Models;

namespace TaskManagerApi.Proxies
{
    /// <inheritdoc />
    public class ProcessesProxy : IProcessesProxy
    {
        /// <inheritdoc />
        public ProcessModel[] GetCurrentProcesses()
        {
            return Process.GetProcesses()
                .Select(x => new ProcessModel(x))
                .ToArray();
        }
    }
}
