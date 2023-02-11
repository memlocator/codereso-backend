using System.Text;
using System.Net.WebSockets;

namespace GameEngine.Networking
{

    public class Connection
    {
        public string clientIP;
        public WebSocket wsSocket;

        public Connection(string clientIP, WebSocket wsSocket)
        {
            this.clientIP = clientIP;
            this.wsSocket = wsSocket;
        }
        public async void sendMessage(string jsonData)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);
            buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonData));
            await wsSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}