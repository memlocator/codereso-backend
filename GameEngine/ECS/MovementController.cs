using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.ECS;

public abstract class MovementController : Component
{
    public override void ParentObjectChanged(Entity? oldParent)
    {
        oldParent?.Transform?.RemoveMovementController();
        ParentObject?.Transform?.AddMovementController();
    }

    public virtual void UpdatePhysics(float delta) { }
}
