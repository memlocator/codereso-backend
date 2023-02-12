using GameEngine.Math2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine.ECS;

namespace GameEngine.Physics;

public class PhysicsScene
{
    public static PhysicsScene GlobalScene { get; private set; } = new();

    private List<Collider> Colliders = new();
    private List<MovementController> MovementControllers = new();

    public void AddCollide(Collider collider)
    {
        Colliders.Add(collider);
    }

    public void RemoveCollide(Collider collider)
    {
        Colliders.Remove(collider);
    }

    public void AddPhysicsController(MovementController movementController)
    {
        MovementControllers.Add(movementController);
    }

    public void RemovePhysicsController(MovementController movementController)
    {
        MovementControllers.Remove(movementController);
    }

    private List<Vector2> Workbench = new();

    public void Update(float delta)
    {
        delta = MathF.Min(delta, 1.0f / 30);

        foreach (MovementController controller in MovementControllers)
        {
            controller.UpdatePhysics(delta);
        }

        for (int i = 0; i < Colliders.Count; ++i)
        {
            Collider collider = Colliders[i];

            if (collider.ParentObject?.Transform == null) continue;

            for (int j = i + 1; j < Colliders.Count; ++j)
            {
                Collider otherCollider = Colliders[j];

                if (otherCollider.ParentObject?.Transform == null) continue;
                if (!collider.ParentObject.Transform.IsStatic && !otherCollider.ParentObject.Transform.IsStatic) continue;

                Workbench.Clear();

                PhysicsResults results = PhysicsResolver.Resolve(collider, otherCollider, Workbench);

                if (results.Type == PhysicsResultsType.NoInteraction) continue;

                Vector2 minimumTranslation = results.MinimumTranslation;

                float movementWeight = collider.ParentObject.Transform.IsStatic ? 0 : 1; // we can replace 1 with mass later if needed
                float otherMovementWeight = otherCollider.ParentObject.Transform.IsStatic ? 0 : 1;
                
                if (!(movementWeight == 0 && otherMovementWeight == 0))
                {
                    movementWeight /= movementWeight + otherMovementWeight;
                    otherMovementWeight /= movementWeight + otherMovementWeight;
                }

                results.MinimumTranslation = minimumTranslation * movementWeight;

                collider.CollidedWith(otherCollider, results);

                results.MinimumTranslation = minimumTranslation * -otherMovementWeight;

                otherCollider.CollidedWith(collider, results);
            }
        }
    }
}
