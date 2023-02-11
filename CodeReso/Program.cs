// See https://aka.ms/new-console-template for more information

using GameEngine;
using GameEngine.Math2D;
using GameEngine.Networking;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        // Send a response back to the client
        EntityUpdate update = new EntityUpdate();
        update.rotation = MathF.PI / 4;
        Random rnd = new Random();
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