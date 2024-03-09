using TaskManagerApi.Models;

namespace TaskManagerApi.Proxies
{
    public interface IProcessesProxy
    {
        ProcessModel[] GetCurrentProcesses();
    }
}