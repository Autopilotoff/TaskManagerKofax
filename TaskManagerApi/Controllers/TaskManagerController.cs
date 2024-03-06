using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TaskManagerApi.Models;
using TaskManagerApi.Services;

namespace TaskManagerApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TaskManagerController : ControllerBase
    {
        private readonly ILogger<TaskManagerController> _logger;

        public TaskManagerController(ILogger<TaskManagerController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetCurrentProcesses")]
        public ProcessModel[] GetCurrentProcesses()
        {
            var processesService = new ProcessesService();
            return processesService.GetCurrentProcesses();
        }

        [HttpGet(Name = "GetCurrentProcessActions")]
        public async Task<IEnumerable<ProcessActionModel>> GetCurrentProcessActionsAsync()
        {
            var processesService = new ProcessesService();
            var processStorage = new ProcessesStorage();
            var ienumerable = await processStorage.GetChangesAsync(processesService.GetCurrentProcesses());
            _logger.LogInformation(ienumerable.Count().ToString());

            var t = 3;
            while (t > 0)
            {
                var testienumerable = await processStorage.GetChangesAsync(processesService.GetCurrentProcesses());
                _logger.LogInformation(testienumerable.Count().ToString());
                _logger.LogInformation(JsonSerializer.Serialize(testienumerable));

                t--;
                Thread.Sleep(2000);
            }

            return ienumerable;
        }
    }
}
