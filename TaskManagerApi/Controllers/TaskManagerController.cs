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
        
        private readonly ISingletonNotificationWebSocketService _notificationWebSocketService;

        private readonly ISingletonProcessesWebSocketService _processesWebSocketService;

        public TaskManagerController(
            ILogger<TaskManagerController> logger,
            ISingletonNotificationWebSocketService singletonNotificationWebSocketService,
            ISingletonProcessesWebSocketService processesWebSocketService)
        {
            _logger = logger;
            
            _notificationWebSocketService = singletonNotificationWebSocketService;
            _processesWebSocketService = processesWebSocketService;
        }

        [HttpGet(Name = "GetNotifications")]
        public async Task GetNotificationsAsync(string token)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _notificationWebSocketService.AddSocketAsync(token, webSocket);
                while (!webSocket.CloseStatus.HasValue)
                {
                    await Task.Delay(_notificationWebSocketService.CheckMillisecondsInterval);
                }
            }
            else
            {
                var isUpdated = await _notificationWebSocketService.TryUpdateWebSocketLifeTimeAsync(token);
                if (!isUpdated)
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
        }


        [HttpGet(Name = "SendCurrentProcessActions")]
        public async Task SendCurrentProcessActionsAsync(string token)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _processesWebSocketService.AddSocketAsync(token, webSocket);
                while (!webSocket.CloseStatus.HasValue)
                {
                    await Task.Delay(_processesWebSocketService.CheckMillisecondsInterval);
                }
            }
            else
            {
                var isUpdated = await _processesWebSocketService.TryUpdateWebSocketLifeTimeAsync(token);
                if (!isUpdated)
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
        }
    }

}
