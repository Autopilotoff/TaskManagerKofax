﻿using System.Net.WebSockets;

namespace TaskManagerApi.Services.Notifications
{
    public interface ISingletonNotificationWebSocketService : IWebSocketService
    {
        Task AddSocketAsync(string token, WebSocket socket);
        Task<bool> TryUpdateWebSocketLifeTimeAsync(string token);
        int CheckMillisecondsInterval { get; }
    }
}