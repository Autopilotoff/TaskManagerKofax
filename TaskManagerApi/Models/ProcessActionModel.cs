using System.Diagnostics;

namespace TaskManagerApi.Models
{
    public class ProcessActionModel
    {
        public ProcessActionModel(ProcessModel process, ActionEnum actionEnum)
        {
            Id = process.Id;
            ProcessName = process.ProcessName;
            NonpagedSystemMemorySize64 = process.NonpagedSystemMemorySize64;
            Action = actionEnum;
        }

        public int Id { get; set; }
        public string ProcessName { get; set; }
        public long NonpagedSystemMemorySize64 { get; set; }

        public ActionEnum Action { get; set; }

        public enum ActionEnum
        {
            Add, Update, Delete
        }
    }
}
