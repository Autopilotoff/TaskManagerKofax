using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Models;
using TaskManagerApi.Services;

namespace TaskManagerApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TaskManagerController : ControllerBase
    {
        [HttpGet(Name = "GetCurrentProcesses")]
        public ProcessModel[] GetCurrentProcesses()
        {
            var processesService = new ProcessesService();
            return processesService.GetCurrentProcesses();
        }
    }
}
