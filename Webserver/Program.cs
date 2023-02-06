using System.Net.WebSockets;
using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.

//builder.Services.AddControllers();
//// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
////if (app.Environment.IsDevelopment())
////{
////    app.UseSwagger();
////    app.UseSwaggerUI();
////}
//app.UseHttpsRedirection();
//app.UseAuthorization();
//app.MapControllers();
//app.UseWebSockets();



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

//app.Run();
