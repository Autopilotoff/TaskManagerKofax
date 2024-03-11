using System.Diagnostics.CodeAnalysis;
using TaskManagerApi.Models;
using TaskManagerApi.Proxies;

namespace TaskManagerApi.Services.Processes
{
    /// <inheritdoc />
    public class SingletonProcessesStorage : ISingletonProcessesStorage
    {
        private readonly IProcessesProxy _processesProxy;
        private static readonly IdProcessModelComparer _idProcessModeComparer = new IdProcessModelComparer();
        private static readonly ProcessModelComparer _processModeComparer = new ProcessModelComparer();

        private readonly Dictionary<int, ProcessModel> _collection;

        public SingletonProcessesStorage(IProcessesProxy processesProxy)
        {
            _processesProxy = processesProxy;
            var processes = processesProxy.GetCurrentProcesses();
            _collection = processes.ToDictionary(x => x.Id, y => y);
        }

        /// <inheritdoc />
        public ProcessesChangesModel GetInitialProcesses()
        {
            var model = new ProcessesChangesModel { AddedProcesses = _collection.Values };
            return model;
        }

        /// <inheritdoc />
        public ProcessesChangesModel GetChanges()
        {
            var processes = _processesProxy.GetCurrentProcesses();
            var taskDelete = Delete(processes);
            var taskUpdate = Update(processes);
            var taskAdd = Add(processes);

            var model = new ProcessesChangesModel
            {
                AddedProcesses = taskAdd,
                DeletedProcesses = taskDelete,
                UpdatedProcesses = taskUpdate
            };

            return model;
        }

        /// <summary>
        /// Adding new processes to the storage.
        /// </summary>
        /// <param name="processes">Current processes.</param>
        /// <returns>Added processes.</returns>
        private IEnumerable<ProcessModel> Add(IEnumerable<ProcessModel> processes)
        {
            var forAdd = processes.ExceptBy(_collection.Values, x => x, _idProcessModeComparer).ToList();

            foreach (var item in forAdd)
            {
                _collection.TryAdd(item.Id, item);
            }

            return forAdd;
        }

        /// <summary>
        /// Deleting removed processes from the stopage.
        /// </summary>
        /// <param name="processes">Current processes.</param>
        /// <returns>Deleted processes.</returns>
        private IEnumerable<int> Delete(IEnumerable<ProcessModel> processes)
        {
            var forDelete = _collection.ExceptBy(processes, x => x.Value, _idProcessModeComparer).ToList();

            foreach (var item in forDelete)
            {
                _collection.Remove(item.Key, out var result);
            }

            return forDelete.Select(x => x.Key).ToList();
        }

        /// <summary>
        /// Updating processes data in the stopage.
        /// </summary>
        /// <param name="processes">Current processes.</param>
        /// <returns>Updated processes.</returns>
        private IEnumerable<ProcessModel> Update(IEnumerable<ProcessModel> processes)
        {
            var forUpdate = processes
                .IntersectBy(_collection.Values, x => x, _idProcessModeComparer)
                .ExceptBy(_collection.Values, x => x, _processModeComparer).ToList();

            foreach (var item in forUpdate)
            {
                _collection[item.Id] = item;
            }

            return forUpdate;
        }

        /// <summary>
        /// Process comparer by Ids.
        /// </summary>
        private class IdProcessModelComparer : IEqualityComparer<ProcessModel>
        {
            public bool Equals(ProcessModel? x, ProcessModel? y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                return x?.Id == y?.Id;
            }

            public int GetHashCode([DisallowNull] ProcessModel obj)
            {
                return obj.Id.GetHashCode();
            }
        }

        /// <summary>
        /// Process comparer by values.
        /// </summary>
        private class ProcessModelComparer : IEqualityComparer<ProcessModel>
        {
            public bool Equals(ProcessModel? x, ProcessModel? y)
            {
                if (x == null && y == null)
                {
                    return true;
                }

                return
                    x?.ProcessName == y?.ProcessName
                    && x?.NonpagedSystemMemorySize64 == y?.NonpagedSystemMemorySize64
                    && x?.PagedMemorySize64 == y?.PagedMemorySize64;
            }

            public int GetHashCode([DisallowNull] ProcessModel obj)
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + (obj.ProcessName != null ? obj.ProcessName.GetHashCode() : 0);
                    hash = hash * 23 + obj.NonpagedSystemMemorySize64.GetHashCode();
                    hash = hash * 23 + obj.PagedMemorySize64.GetHashCode();
                    return hash;
                }
            }
        }
    }
}
