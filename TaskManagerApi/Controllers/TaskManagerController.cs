using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Text;
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
        public async Task<ProcessActionModel> GetCurrentProcessActionsAsync()
        {
            var processesService = new ProcessesService();
            var processStorage = new ProcessesStorage();
            var ienumerable = await processStorage.GetChangesAsync(processesService.GetCurrentProcesses());
            _logger.LogInformation(ienumerable.AddedProcesses.Count().ToString());

            var t = 3;
            while (t > 0)
            {
                var testienumerable = await processStorage.GetChangesAsync(processesService.GetCurrentProcesses());
                _logger.LogInformation(testienumerable.AddedProcesses.Count().ToString());
                _logger.LogInformation(JsonSerializer.Serialize(testienumerable));

                t--;
                Thread.Sleep(2000);
            }

            return ienumerable;
        }

        [HttpGet(Name = "SendCurrentProcessActions")]
        public async Task SendCurrentProcessActionsAsync()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await ExecSendingAsync(webSocket);
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }

        private async Task ExecSendingAsync(WebSocket webSocket)
        {
            var processesService = new ProcessesService();
            var processStorage = new ProcessesStorage();

            var buffer = new byte[1024 * 4];
            var timeOut = 5000;
            try
            {
                var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), new CancellationTokenSource(timeOut).Token);

                while (!receiveResult.CloseStatus.HasValue)
                {
                    var data = await processStorage.GetChangesAsync(processesService.GetCurrentProcesses());
                    var message = JsonSerializer.Serialize(data);
                    var bytes = Encoding.UTF8.GetBytes(message);
                    var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);

                    await webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);

                    receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), new CancellationTokenSource(timeOut).Token);
                }

                await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
            }
            catch (OperationCanceledException e)
            {
                _logger.LogError(exception: e, e.Message);
            }
        }
    }
}
