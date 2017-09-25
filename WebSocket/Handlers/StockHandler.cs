using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace ChatWebSocket
{
    public class StockHandler: WebSocketHandler
    {
        private Timer timer;
        private static int maxChange = 10;
        private int lastClose = maxChange/2;        

        public StockHandler(WebSocketConnectionManager webSocketConnectionManager) : base(webSocketConnectionManager)
        {
        }

        public override async Task OnConnected(WebSocket socket)
        {
            await base.OnConnected(socket);

            var socketId = WebSocketConnectionManager.GetId(socket);

            var dictionary = WebSocketConnectionManager.GetAll();
            if (dictionary.Count == 1)
            {
                var autoEvent = new AutoResetEvent(false);
                timer = new Timer(SendValueAsync, autoEvent, 0, 1000);
            }            
        }

        public override async Task OnDisconnected(WebSocket socket)
        {
            await WebSocketConnectionManager.RemoveSocket(WebSocketConnectionManager.GetId(socket));
            var dictionary = WebSocketConnectionManager.GetAll();
            if (dictionary.Count == 0)
            {
                timer.Dispose();
            }
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
        }

        private async void SendValueAsync(Object stateInfo)
        {
            AutoResetEvent autoEvent = (AutoResetEvent)stateInfo;

            string stockJson = CreateStockMessage();
            await SendMessageToAllAsync(stockJson);
        }

        private string CreateStockMessage()
        {
            var stock = new Contract.Stock();
            var rnd = new Random();
            stock.DateTime = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            int Open = lastClose - maxChange / 2 + rnd.Next(0, maxChange);
            if (Open < 0)
            {
                Open = 0;
            }
            stock.Open = Open;
            int Close = stock.Open - maxChange / 2 + rnd.Next(0, maxChange);
            if(Close < 0)
            {
                Close = 0;
            }
            stock.Close = Close;

            int Low = Math.Min(Open, Close);
            Low -= rnd.Next(0, maxChange / 2);
            if(Low < 0)
            {
                Low = 0;
            }
            stock.Low = Low;
            int High = Math.Max(Open, Close);
            High += rnd.Next(0, maxChange / 2);
            stock.High = High;

            return JsonConvert.SerializeObject(stock);
        }
    }
}
