using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine.ECS;
using GameEngine.Math2D;

namespace GameEngine.Physics;

public class PhysicsController : MovementController
{
    public Vector2 Velocity;

    public override void UpdatePhysics(float delta)
    {
        if (ParentObject?.Transform == null) return;

        ParentObject.Transform.Position += delta * Velocity;
    }
}
