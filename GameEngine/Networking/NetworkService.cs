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

namespace GameEngine.Networking
{

    public class NetworkService
    {
        private WebApplicationBuilder? builder;
        private WebApplication? app;
        public WebSocket? webSocket;


        public NetworkService()
        {
            builder = WebApplication.CreateBuilder();
            app = builder.Build();

            app.UseWebSockets();
            //app.UseHttpsRedirection();
            //app.UseAuthorization();
            //app.MapControllers();
            app.Use(async (context, next) =>
            {
                Console.WriteLine("Web request!");
                if (context.WebSockets.IsWebSocketRequest)
                {
                    Console.WriteLine("WS initiated!");
                    webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    while (webSocket.State == WebSocketState.Open)
                    {
                        ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);
                        WebSocketReceiveResult result = await webSocket.ReceiveAsync(buffer, CancellationToken.None);

                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            // Process the received message
                            string message = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, result.Count);
                            Console.WriteLine("Received message: " + message);

                            // Send a response back to the client
                            //string response = "Hello from the server!";
                            //x += 0.01f;
                            //PositionUpdate update = new PositionUpdate();
                            //update.x = x;
                            //string response = JsonSerializer.Serialize(update);


                            //buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(response));
                            await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                        else if (result.MessageType == WebSocketMessageType.Close)
                        {
                            // Close the WebSocket
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        }
                    }
                }
                else
                {
                    await next();
                }
            });
            app.RunAsync();

            
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