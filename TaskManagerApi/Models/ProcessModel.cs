namespace TaskManagerApi.Models
{
    public class ProcessModel
    {
        public int Id { get; set; }
        public string ProcessName { get; set; }
        public long NonpagedSystemMemorySize64 { get; set; }
    }
}
