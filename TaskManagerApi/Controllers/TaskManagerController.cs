using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Services.Processes;
using TaskManagerApi.Services.Notifications;
using TaskManagerApi.Services;

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

        /// <summary>
        /// Notifications subscription.
        /// </summary>
        /// <param name="token">Client token.</param>
        /// <returns>.</returns>
        [HttpGet(Name = "GetNotifications")]
        public async Task GetNotificationsAsync(string token)
        {
            await AddSubscriptionAsync(token, _notificationWebSocketService);
        }

        /// <summary>
        /// Current processes subscription.
        /// </summary>
        /// <param name="token">Client token.</param>
        /// <returns>.</returns>
        [HttpGet(Name = "GetCurrentProcessActions")]
        public async Task GetCurrentProcessActionsAsync(string token)
        {
            await AddSubscriptionAsync(token, _processesWebSocketService);
        }

        private async Task AddSubscriptionAsync(string token, IWebSocketService webSocketService)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await webSocketService.AddSocketAsync(token, webSocket);
                while (!webSocket.CloseStatus.HasValue)
                {
                    await Task.Delay(webSocketService.SendDataMillisecondsInterval);
                }
            }
            else
            {
                var isUpdated = await webSocketService.TryUpdateWebSocketLifeTimeAsync(token);
                if (!isUpdated)
                {
                    HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                }
            }
        }
    }

}
