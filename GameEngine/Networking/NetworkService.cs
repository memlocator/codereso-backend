﻿using Microsoft.AspNetCore.Builder;
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
        WebsocketServer server;
        List<Connection> connections = new List<Connection>();
        public NetworkService()
        {
            WebsocketServer server = new WebsocketServer(ref connections);
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