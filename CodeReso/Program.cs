// See https://aka.ms/new-console-template for more information

using GameEngine;
using GameEngine.ECS;
using GameEngine.Math2D;
using GameEngine.Networking;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using GameEngine.Utils;

public class GameInit
{

    
    static void Main(string[] args)
    {
        Matrix2 a = new(new(), new Vector2(2, 2));
        Matrix2 b = new(new Vector2(1, 1));
        Matrix2 c = new(new(), new Vector2(0, 1), new Vector2(-1, 0));

        Matrix2 ab = a * b;
        Matrix2 ba = b * a;
        Matrix2 cb = c * b;
        Matrix2 bc = b * c;

        NetworkService networkService = new GameEngine.Networking.NetworkService();
        Entity ent = new Entity();
        Entity ent1 = new Entity();
        
        while (true)
        {
            //GameEngine.Networking.ClientEvents.MessageWriter msgWriter = new GameEngine.Networking.ClientEvents.MessageWriter();
            //msgWriter.Message = new GameEngine.Networking.ClientEvents.NewEntityEvent(ent);
            //string serializedMsg = JsonSerializer.Serialize(msgWriter);
            //Console.WriteLine(serializedMsg);

            ent.Update(1f);
            networkService.Update();
            networkService.ReplicateAllEntitiesToAllConnections();
            Thread.Sleep(100);
        }
        Console.WriteLine(ent.ToString());
        // Send a response back to the client
        //Console.WriteLine("Finished websocket");
        Console.ReadKey();
        //JsonSerializerOptions options = new()
        //{
        //    ReferenceHandler = ReferenceHandler.Preserve,
        //    //WriteIndented = true
        //};

        //while (true)
        //{
        //    string response = JsonSerializer.Serialize(update);
        //    var buffer = new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(response));
        //    //update.position += new Vector2(0.0014f, 0);
        //    //update.scale += new Vector2((float)rnd.NextDouble(), 0);
        //    update.rotation += 0.05f;

        //    //networkService.webSocket?.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        //    //Console.WriteLine("Sent");

        //    Thread.Sleep(100);
        //};

        Console.WriteLine("Hello, World!");
    }
}