using GameEngine.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using GameEngine.Utils;

namespace GameEngine.Networking
{
    
    namespace ClientEvents
    {
        [JsonConverter(typeof(MessageWriterJsonConverter))]
        public class MessageWriter
        {
            public BaseEvent Message;
        }
        public class MessageWriterJsonConverter : JsonConverter<MessageWriter>
        {
            public override MessageWriter? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }

            public override void Write(Utf8JsonWriter writer, MessageWriter value, JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteMessage(value.Message, options);
                writer.WriteEndObject();
          }
        }

        public class BaseEvent
        {
            public BaseEvent(string type)
            {
                this.type = type;
            }
            public string type;
        }

        public class NewEntityEvent : BaseEvent
        {
            public Entity? entity;
            public NewEntityEvent(Entity entity) : base("NewEntityEvent")
            {
                this.entity = entity;
            }
        }

        public class DestroyedEntityEvent : BaseEvent
        {
            public Entity? entity;
            public DestroyedEntityEvent(Entity entity) : base("DestroyedEntityEvent")
            {
                this.entity = entity;
            }
        }

        public class EntityUpdateEvent : BaseEvent
        {
            public Entity? entity;
            public EntityUpdateEvent(Entity entity) : base("EntityUpdateEvent")
            {
                this.entity = entity;
            }
        }

    }
    
}
