using System.Diagnostics;

namespace TaskManagerApi.Models
{
    public class ProcessModel
    {
        public ProcessModel() { }

        public ProcessModel(Process process)
        {
            Id = process.Id;
            ProcessName = process.ProcessName;
            NonpagedSystemMemorySize64 = process.NonpagedSystemMemorySize64;
            PagedMemorySize64 = process.PagedMemorySize64;
        }

        public int Id { get; set; }

        public string ProcessName { get; set; }

        public long NonpagedSystemMemorySize64 { get; set; }

        public long PagedMemorySize64 { get; set; }
    }
}
