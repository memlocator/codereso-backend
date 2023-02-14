using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GameEngine.Math2D;

namespace GameEngine.Networking
{
    
    internal class WebsocketServer
    {
        public List<Connection> connections;

        bool IsAlreadyConnected(string clientIP)
        {
            bool found = false;

            return found;
        }

        async void Start()
        {
            HttpListener wsListener = new HttpListener();
            wsListener.Prefixes.Add("http://localhost:5000/"); //which address this listener listens to
            wsListener.Start();
            Console.WriteLine("Started listening on localhost:5000");

            while (true)
            {
                HttpListenerContext wsListenerContext = await wsListener.GetContextAsync();
                if (wsListenerContext.Request.IsWebSocketRequest)
                {


                    ProcessRequest(wsListenerContext);
                }
                else
                {
                    //CloseConnection(wsListenerContext.Request.RemoteEndPoint.Address.ToString());
                    wsListenerContext.Response.Close();
                }
            }
        }

        private void CloseConnection(string clientIP)
        {
            
            int idx = 0;
            foreach (Connection conn in connections)
            {
                if (conn.clientIP == clientIP)
                {
                    connections.RemoveAt(idx);
                    break;
                }
                idx++;
            }
            Console.WriteLine(clientIP + " is disconnecting online: " + connections.Count);
        }

        private void CreateConnection(string clientIP, WebSocket wsSocket)
        {
            Connection newConn = new Connection(clientIP, wsSocket);
            connections.Add(newConn);
            Console.WriteLine(clientIP + " connected, online: " + connections.Count);
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
            catch (Exception ex)
            {
                Console.WriteLine("Error while getting wsContext: {0}", ex.Message);
            }

            WebSocket wsSocket = wsContext.WebSocket;
            try
            {
                ArraySegment<byte> recBuff = new ArraySegment<byte>(new byte[4096]);//new byte[4096];
                if (!IsAlreadyConnected(context.Request.RemoteEndPoint.Address.ToString()))
                    CreateConnection(context.Request.RemoteEndPoint.Address.ToString(), wsSocket);

                while (wsSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult recResult = await wsSocket.ReceiveAsync(new ArraySegment<byte>(recBuff.Array), CancellationToken.None);
                    if (recResult.MessageType == WebSocketMessageType.Close)
                    {
                        await wsSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client terminated connection.", CancellationToken.None);
                    }
                    else
                    {
                        string message = Encoding.UTF8.GetString(recBuff.Array, recBuff.Offset, recResult.Count);
                        Console.WriteLine("Received message: " + message);


                        //for (int i = 0; i < 300; i++)
                        //{
                        //    update.position += new Vector2(0.004f, 0f);
                        //    string response = JsonSerializer.Serialize(update);
                        //    ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);
                        //    buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(response));
                        //    await wsSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                        //    Console.WriteLine("Responding: " + response);
                        //    Thread.Sleep(100);
                        //}

                        //await wsSocket.SendAsync(new ArraySegment<byte>(recBuff, 0, recResult.Count), WebSocketMessageType.Binary, recResult.EndOfMessage, CancellationToken.None);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (wsSocket != null)
                {
                    CloseConnection(context.Request.RemoteEndPoint.Address.ToString());
                    wsSocket.Dispose();
                }
            }
        }
        public WebsocketServer(ref List<Connection> connections)
        {
            this.connections = connections;
            Start();
        }
        
    }
}
