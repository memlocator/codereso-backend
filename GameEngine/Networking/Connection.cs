using System.Text;
using System.Net.WebSockets;
using GameEngine.ECS;
using System.Text.Json;
using GameEngine.Networking.ClientEvents;
using GameEngine.Utils;
using System;

namespace GameEngine.Networking
{
    public class Connection
    {
        public string clientIP;
        private WebSocket wsSocket;

        List<bool> replicatedObjects = new List<bool>() { false };

        public void Replicate(int id)
        {
            if (id + 1 > replicatedObjects.Count)
                replicatedObjects.Resize((replicatedObjects.Count)*2);

            if (IsReplicated(id))
                return;

            Entity? replicationEntity = Entity.GetEntity(id);
            if (replicationEntity == null)
                throw new InvalidOperationException("Attempted to replicate entity with an id that did not return an entity object.");

            replicatedObjects[id] = true;
            MessageWriter msgWriter = new MessageWriter();
            msgWriter.Message = new NewEntityEvent(replicationEntity);
            string serializedMsg = JsonSerializer.Serialize(msgWriter);
            SendMessage(serializedMsg);
        }

        public void update(int id)
        {
            if (!IsReplicated(id)) return;

            Entity? e = Entity.GetEntity(id);

            if (e == null)
                throw new Exception("Entity is null");

            MessageWriter msgWriter = new MessageWriter();
            msgWriter.Message = new UpdateEntityEvent(e);
            string serializedMsg = JsonSerializer.Serialize(msgWriter);
            SendMessage(serializedMsg);
        }

        public bool IsReplicated(int id)
        {
            if (replicatedObjects.Count - 1 >= id)
                return replicatedObjects[id];
            else
                return false;
        }

        public Connection(string clientIP, WebSocket wsSocket)
        {
            this.clientIP = clientIP;
            this.wsSocket = wsSocket;
        }

        public void RemoveReplication(int id)
        {
            if (!IsReplicated(id)) return;
            //TODO: remove object from client
            replicatedObjects[id] = false;

            Entity? replicationEntity = Entity.GetEntity(id);
            if (replicationEntity == null)
                throw new InvalidOperationException("Attempted to replicate entity with an id that did not return an entity object.");

            MessageWriter msgWriter = new MessageWriter();
            msgWriter.Message = new DestroyedEntityEvent(replicationEntity);
            string serializedMsg = JsonSerializer.Serialize(msgWriter);
            SendMessage(serializedMsg);
        }
        private async void SendMessage(string jsonData)
        {
            if (wsSocket.State == WebSocketState.Open)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);
                buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonData));
                await wsSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        internal void UpdateReplicatedEntities()
        {
            for (int i = 0; i < replicatedObjects.Count; i++) 
            {
                if (replicatedObjects[i])
                {
                    try
                    {
                        update(i);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to network update for id {0}, {1}", i, e.Message);
                    }
                }
                    
                    
            }
        }
    }
}