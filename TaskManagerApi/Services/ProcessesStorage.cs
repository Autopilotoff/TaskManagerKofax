using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using TaskManagerApi.Models;

namespace TaskManagerApi.Services
{
    public class ProcessesStorage
    {
        private ConcurrentDictionary<int, ProcessModel> _collection = new ConcurrentDictionary<int, ProcessModel>();
        private readonly IdProcessModelComparer _idProcessModeComparer = new IdProcessModelComparer();
        private readonly ProcessModelComparer _processModeComparer = new ProcessModelComparer();


        public async Task<IEnumerable<ProcessActionModel>> GetChangesAsync(IEnumerable<ProcessModel> processes)
        {
            var taskAdd = AddAsync(processes);
            var taskDelete = DeleteAsync(processes);
            var taskUpdate = UpdateAsync(processes);
            await Task.WhenAll(taskAdd, taskDelete, taskUpdate);

            return taskAdd.Result
                .Union(taskDelete.Result)
                .Union(taskUpdate.Result);
        }

        private async Task<IEnumerable<ProcessActionModel>> AddAsync(IEnumerable<ProcessModel> processes)
        {
            var forAdd = processes.ExceptBy(_collection.Values, x => x, _idProcessModeComparer).ToList();

            foreach (var item in forAdd)
            {
                _collection.TryAdd(item.Id, item);
            }

            return forAdd.Select(x => new ProcessActionModel(x, ProcessActionModel.ActionEnum.Add));
        }

        private async Task<IEnumerable<ProcessActionModel>> DeleteAsync(IEnumerable<ProcessModel> processes)
        {
            var forDelete = _collection.ExceptBy(processes, x => x.Value, _idProcessModeComparer).ToList();

            foreach (var item in forDelete)
            {
                _collection.Remove(item.Key, out var result);
            }

            return forDelete.Select(x => new ProcessActionModel(x.Value, ProcessActionModel.ActionEnum.Delete));
        }

        private async Task<IEnumerable<ProcessActionModel>> UpdateAsync(IEnumerable<ProcessModel> processes)
        {
            var forUpdate = processes
                .IntersectBy(_collection.Values, x => x, _idProcessModeComparer)
                .ExceptBy(_collection.Values, x => x, _processModeComparer).ToList();

            foreach (var item in forUpdate)
            {
                _collection[item.Id] = item;
            }

            return forUpdate.Select(x => new ProcessActionModel(x, ProcessActionModel.ActionEnum.Update));
        }

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
                    && x?.NonpagedSystemMemorySize64 == y?.NonpagedSystemMemorySize64;
            }

            public int GetHashCode([DisallowNull] ProcessModel obj)
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + (obj.ProcessName != null ? obj.ProcessName.GetHashCode() : 0);
                    hash = hash * 23 + obj.NonpagedSystemMemorySize64.GetHashCode();
                    return hash;
                }
            }
        }
    }
}
