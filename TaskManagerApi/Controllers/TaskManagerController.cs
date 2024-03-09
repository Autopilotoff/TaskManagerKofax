using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Services.Processes;
using TaskManagerApi.Services.Notifications;

namespace TaskManagerApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TaskManagerController : ControllerBase
    {
        private readonly ILogger<TaskManagerController> _logger;
        private readonly INotificationWebSocketService _notificationWebSocketService;
        private readonly IProcessesWebSocketService _processesWebSocketService;

        public TaskManagerController(
            ILogger<TaskManagerController> logger,
            INotificationWebSocketService notificationWebSocketService,
            IProcessesWebSocketService processesWebSocketService)
        {
            _logger = logger;
            _notificationWebSocketService = notificationWebSocketService;
            _processesWebSocketService = processesWebSocketService;

        }

        // [HttpGet(Name = "GetCurrentProcesses")]
        // public ProcessModel[] GetCurrentProcesses()
        // {
        //     var processesService = new ProcessesService();
        //     return processesService.GetCurrentProcesses();
        // }
        // 
        // [HttpGet(Name = "GetCurrentProcessActions")]
        // public async Task<ProcessActionModel> GetCurrentProcessActionsAsync()
        // {
        //     var processesService = new ProcessesService();
        //     var processStorage = new ProcessesStorage();
        //     var ienumerable = await processStorage.GetChangesAsync(processesService.GetCurrentProcesses());
        //     _logger.LogInformation(ienumerable.AddedProcesses.Count().ToString());
        // 
        //     var t = 3;
        //     while (t > 0)
        //     {
        //         var testienumerable = await processStorage.GetChangesAsync(processesService.GetCurrentProcesses());
        //         _logger.LogInformation(testienumerable.AddedProcesses.Count().ToString());
        //         _logger.LogInformation(JsonSerializer.Serialize(testienumerable));
        // 
        //         t--;
        //         Thread.Sleep(2000);
        //     }
        // 
        //     return ienumerable;
        // }

        [HttpGet(Name = "GetNotifications")]
        public async Task GetNotificationsAsync()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _notificationWebSocketService.ExecuteSendingAsync(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        [HttpGet(Name = "SendCurrentProcessActions")]
        public async Task SendCurrentProcessActionsAsync()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _processesWebSocketService.ExecuteSendingAsync(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
