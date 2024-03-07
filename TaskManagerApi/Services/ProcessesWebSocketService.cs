using System.Net.WebSockets;
using System.Text.Json;
using System.Text;

namespace TaskManagerApi.Services
{
    public class ProcessesWebSocketService : WebSocketService
    {
        private readonly ProcessesService _processesService;

        private readonly ProcessesStorage _processStorage;

        public ProcessesWebSocketService(WebSocket webSocket, ILogger logger) : base(webSocket, logger)
        {
            _processesService = new ProcessesService();
            _processStorage = new ProcessesStorage();
        }

        public override async Task ExecuteSendingAsync()
        {
            await ExecuteAsync();
        }

        protected override async Task NotifyAsync()
        {
            var data = await _processStorage.GetChangesAsync(_processesService.GetCurrentProcesses());
            var message = JsonSerializer.Serialize(data);
            var bytes = Encoding.UTF8.GetBytes(message);
            var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);

            await _webSocket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
