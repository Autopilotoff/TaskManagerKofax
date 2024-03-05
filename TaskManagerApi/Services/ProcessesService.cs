using System.Diagnostics;
using TaskManagerApi.Models;

namespace TaskManagerApi.Services
{
    public class ProcessesService
    {
        public ProcessModel[] GetCurrentProcesses()
        {
            return Process.GetProcesses()
                .Select(x => new ProcessModel { Id = x.Id, ProcessName = x.ProcessName, NonpagedSystemMemorySize64 = x.NonpagedSystemMemorySize64 })
                .ToArray();
        }
    }
}
