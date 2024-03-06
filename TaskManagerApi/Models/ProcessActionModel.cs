using System.Diagnostics;
using System.Text.Json.Serialization;

namespace TaskManagerApi.Models
{
    public class ProcessActionModel
    {
        [JsonPropertyName("added")]
        public IEnumerable<ProcessModel> AddedProcesses { get; set; }

        [JsonPropertyName("updated")]
        public IEnumerable<ProcessModel> UpdatedProcesses { get; set; }

        [JsonPropertyName("deleted")]
        public IEnumerable<int> DeletedProcesses { get; set; }
    }
}
