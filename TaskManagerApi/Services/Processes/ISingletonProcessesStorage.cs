using TaskManagerApi.Models;

namespace TaskManagerApi.Services.Processes
{
    public interface ISingletonProcessesStorage
    {
        ProcessChangesModel GetChanges();
        ProcessChangesModel GetInitialProcesses();
    }
}