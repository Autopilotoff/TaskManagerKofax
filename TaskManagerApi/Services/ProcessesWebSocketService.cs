using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Options;
using TaskManagerApi.SettingsModels;

namespace TaskManagerApi.Services
{
    public class ProcessesWebSocketService : IProcessesWebSocketService
    {
        private readonly ProcessesService _processesService;
        private readonly ProcessesStorage _processStorage;

        private readonly ILogger<IProcessesWebSocketService> _logger;

        private int CancellationMillisecondsTimeOut { get; } = 5000;

        public ProcessesWebSocketService(ILogger<IProcessesWebSocketService> logger, IOptions<ProcessesSettings> processesSettings)
        {
            _processesService = new ProcessesService();
            _processStorage = new ProcessesStorage();
            _logger = logger;
            CancellationMillisecondsTimeOut = processesSettings?.Value?.CancellationMillisecondsTimeOut ?? CancellationMillisecondsTimeOut;
        }

        public async Task ExecuteSendingAsync(WebSocket webSocket)
        {
            _logger.LogInformation("Sending started...");

            var buffer = new byte[1024 * 4];
            try
            {
                var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), new CancellationTokenSource(CancellationMillisecondsTimeOut).Token);

                while (!receiveResult.CloseStatus.HasValue)
                {
                    await NotifyAsync(webSocket);
                    receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), new CancellationTokenSource(CancellationMillisecondsTimeOut).Token);
                }

                await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
            }
            catch (OperationCanceledException e)
            {
                _logger.LogError(exception: e, e.Message);
            }

            _logger.LogInformation("...Sending stoped.");
        }

        private async Task NotifyAsync(WebSocket webSocket)
        {
            var data = await _processStorage.GetChangesAsync(_processesService.GetCurrentProcesses());
            var message = JsonSerializer.Serialize(data);
            var bytes = Encoding.UTF8.GetBytes(message);
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);

            await webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
