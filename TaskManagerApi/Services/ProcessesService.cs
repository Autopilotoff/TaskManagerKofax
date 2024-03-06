using System.Diagnostics;
using TaskManagerApi.Models;

namespace TaskManagerApi.Services
{
    public class ProcessesService
    {
        public ProcessModel[] GetCurrentProcesses()
        {
            return Process.GetProcesses()
                .Select(x => new ProcessModel(x))
                .ToArray();
        }
    }
}
