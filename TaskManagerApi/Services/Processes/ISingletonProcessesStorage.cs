using TaskManagerApi.Models;

namespace TaskManagerApi.Services.Processes
{
    /// <summary>
    /// A storage of processes.
    /// </summary>
    public interface ISingletonProcessesStorage
    {
        /// <summary>
        /// Gettting of proccesses changes.
        /// </summary>
        /// <returns>Changes model.</returns>
        ProcessesChangesModel GetChanges();

        /// <summary>
        /// Getting of an initial process list.
        /// </summary>
        /// <returns>Changes model.</returns>
        ProcessesChangesModel GetInitialProcesses();
    }
}