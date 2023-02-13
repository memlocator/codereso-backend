using System.Text;
using System.Net.WebSockets;
using GameEngine.ECS;
using System.Text.Json;
using GameEngine.Networking.ClientEvents;
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
            if (id + 1 > replicatedObjects.Capacity - 1)
                replicatedObjects.EnsureCapacity((replicatedObjects.Capacity + 1 )*2);

            if (IsReplicated(id))
                return;

            Entity? replicationEntity = Entity.GetEntity(id);
            if (replicationEntity == null)
                throw new InvalidOperationException("Attempted to replicate entity with an id that did not return an entity object.");

            replicatedObjects[id] = true;
            string serializedMsg = JsonSerializer.Serialize(new NewEntityEvent(replicationEntity));
            SendMessage(serializedMsg);
            
        }

        public void update(int id)
        {
            if (!IsReplicated(id)) return;

            Entity? e = Entity.GetEntity(id);

            if (e == null)
                throw new Exception("Entity is null");

            string serializedMsg = JsonSerializer.Serialize(e);
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
        }
        private async void SendMessage(string jsonData)
        {
            ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[4096]);
            buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonData));
            await wsSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        internal void UpdateReplicatedEntities()
        {
            for (int i = 0; i < replicatedObjects.Count; i++) 
            {
                if (replicatedObjects[i])
                    update(i);
            }
        }
    }
}