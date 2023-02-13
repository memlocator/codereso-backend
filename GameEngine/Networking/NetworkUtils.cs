using GameEngine.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Networking
{
    namespace ClientEvents
    {
        class BaseEvent
        {
            public BaseEvent(string type)
            {
                this.type = type;
            }
            protected string type;
        }

        class NewEntityEvent : BaseEvent
        {
            //public Entity? entity;
            public NewEntityEvent(Entity entity) : base("NewEntityEvent")
            {
                //this.entity = entity;
            }
        }

        class DestroyedEntityEvent : BaseEvent
        {
            //public Entity? entity;
            public DestroyedEntityEvent(Entity entity) : base("DestroyedEntityEvent")
            {
                //this.entity = entity;
            }
        }

    }
    
}
