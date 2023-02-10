using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net;
using GameEngine.Math2D;



namespace GameEngine.Networking
{
    public class EntityUpdate
    {
        public Vector2 position { get; set; }
        public Vector2 scale { get; set; }
        public float rotation { get; set; }

    };


    public class NetworkService
    {
        //private WebApplicationBuilder? builder;
        //private WebApplication? app;
        //public WebSocket? webSocket;
        //public List<WebSocket> webSockets = new List<WebSocket>();

        //private void HandleRequests()
        //{
        //    ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);


        //    foreach (WebSocket socket in webSockets )
        //    {
        //        WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
        //    }
        //}
        async void Start()
        {
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/");
            listener.Start();
            Console.WriteLine("Started listening on localhost:5000");

            while (true)
            {
                HttpListenerContext httpListenerContext = await listener.GetContextAsync();
                Console.WriteLine("was a ws req");
                if (httpListenerContext.Request.IsWebSocketRequest)
                {
                    
                    ProcessRequest(httpListenerContext);
                }
                else
                {
                    httpListenerContext.Response.Close();
                }
            }
        }

        private async void ProcessRequest(HttpListenerContext context)
        {
            WebSocketContext wsContext = null;
            try
            {
                wsContext = await context.AcceptWebSocketAsync(subProtocol: null);
                string ipAddr = context.Request.RemoteEndPoint.Address.ToString();
                Console.WriteLine("Connected: {0}", ipAddr);
            }
            catch(Exception ex)
            {
                Console.WriteLine("Error while getting wsContext: {0}", ex.Message);
            }

            WebSocket wsSocket = wsContext.WebSocket;
            try
            {
                ArraySegment<byte> recBuff = new ArraySegment<byte>(new byte[4096]);//new byte[4096];
                EntityUpdate update = new EntityUpdate();
                while (wsSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult recResult = await wsSocket.ReceiveAsync(new ArraySegment<byte>(recBuff.Array), CancellationToken.None);
                    if (recResult.MessageType == WebSocketMessageType.Close) 
                    {
                        await wsSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client terminated connection.", CancellationToken.None);

                    }
                    else
                    {
                        update.position += new Vector2(0.004f, 0f);
                        string response = JsonSerializer.Serialize(update);

                        string message = Encoding.UTF8.GetString(recBuff.Array, recBuff.Offset, recResult.Count);
                        Console.WriteLine("Received message: " + message);

                        ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);
                        buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(response));
                        await wsSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                        //await wsSocket.SendAsync(new ArraySegment<byte>(recBuff, 0, recResult.Count), WebSocketMessageType.Binary, recResult.EndOfMessage, CancellationToken.None);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally 
            {
                if (wsSocket != null)
                {
                    wsSocket.Dispose();
                }
            }
        }

        public NetworkService()
        {
            Start();

            //builder = WebApplication.CreateBuilder();
            //app = builder.Build();

            //app.UseWebSockets();
            ////app.UseHttpsRedirection();
            ////app.UseAuthorization();
            ////app.MapControllers();
            //app.Use(async (context, next) =>
            //{
            //    Console.WriteLine("Web request!");
            //    if (context.WebSockets.IsWebSocketRequest)
            //    {
            //        Console.WriteLine("WS initiated!");
            //        webSocket = await context.WebSockets.AcceptWebSocketAsync();
            //        webSockets.Add(webSocket);
            //        //while (webSocket.State == WebSocketState.Open)
            //        //{
            //        //    ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);
            //        //    WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

            //        //    if (result.MessageType == WebSocketMessageType.Text)
            //        //    {
            //        //        // Process the received message
            //        //        string message = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, result.Count);
            //        //        Console.WriteLine("Received message: " + message);

            //        //        // Send a response back to the client
            //        //        //string response = "Hello from the server!";
            //        //        //x += 0.01f;
            //        //        //PositionUpdate update = new PositionUpdate();
            //        //        //update.x = x;
            //        //        //string response = JsonSerializer.Serialize(update);


            //        //        //buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(response));
            //        //        await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            //        //    }
            //        //    else if (result.MessageType == WebSocketMessageType.Close)
            //        //    {
            //        //        // Close the WebSocket
            //        //        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            //        //    }
            //        //}
            //    }
            //    else
            //    {
            //        await next();
            //    }
            //});
            //app.RunAsync();

            
        }
    }
}

//app.Use(async (context, next) =>
//{

//    if (context.WebSockets.IsWebSocketRequest)
//    {
//        Console.WriteLine("WS initiated!");
//        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
//        while (webSocket.State == WebSocketState.Open)
//        {
//            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);
//            WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

//            if (result.MessageType == WebSocketMessageType.Text)
//            {
//                // Process the received message
//                string message = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, result.Count);
//                Console.WriteLine("Received message: " + message);

//                // Send a response back to the client
//                string response = "Hello from the server!";
//                buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(response));
//                await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
//            }
//            else if (result.MessageType == WebSocketMessageType.Close)
//            {
//                // Close the WebSocket
//                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
//            }
//        }
//    }
//    else
//    {
//        await next();
//    }
//});