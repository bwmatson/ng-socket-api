using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ChatWebSocket
{
    public class ChatMessageHandler: WebSocketHandler
    {
        public ChatMessageHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);

            var socketId = WebSocketConnectionManager.GetId(socket);
            await SendMessageToAllAsync($"{socketId} is now connected");
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var message = $"{Encoding.UTF8.GetString(buffer, 0, result.Count)}";

            await SendMessageToAllButSelfAsync(socket, message);
        }

        public async Task SendMessageToAllButSelfAsync(WebSocket socket, string message)
        {
            var dictionary = WebSocketConnectionManager.GetAll();
            foreach (var pair in dictionary)
            {
                if (pair.Value.State == WebSocketState.Open)
                {
                    if (pair.Value == socket)
                    {
                        if (dictionary.Count == 1)
                        {
                            await SendMessageAsync(socket, "Sorry, no one else is here");
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        await SendMessageAsync(pair.Value, message);
                    }
                }
            }
        }
    }
}
