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
        private readonly ISingletonProcessesWebSocketService _processesWebSocketService;

        public TaskManagerController(
            ILogger<TaskManagerController> logger,
            INotificationWebSocketService notificationWebSocketService,
            ISingletonProcessesWebSocketService processesWebSocketService)
        {
            _logger = logger;
            _notificationWebSocketService = notificationWebSocketService;
            _processesWebSocketService = processesWebSocketService;
        }

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
        public async Task SendCurrentProcessActionsAsync(string token)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await _processesWebSocketService.AddSocketAsync(token, webSocket);
                while (!webSocket.CloseStatus.HasValue)
                {
                    await Task.Delay(_processesWebSocketService.NotifyMillisecondsDelay);
                }
            }
            else
            {
                var isUpdated = _processesWebSocketService.TryUpdateWebSocketLifeTime(token);
                if (!isUpdated)
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
        }
    }

}
