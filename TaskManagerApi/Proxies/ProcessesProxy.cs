using System.Diagnostics;
using TaskManagerApi.Models;

namespace TaskManagerApi.Proxies
{
    public class ProcessesProxy : IProcessesProxy
    {
        public ProcessModel[] GetCurrentProcesses()
        {
            return Process.GetProcesses()
                .Select(x => new ProcessModel(x))
                .ToArray();
        }
    }
}
