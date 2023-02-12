using GameEngine.Math2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Physics;

public enum PhysicsResultsType
{
    NoInteraction,
    Detected,
    Resolution
}

public struct PhysicsResults
{
    public PhysicsResultsType Type;
    public Vector2 MinimumTranslation;
}

public class PhysicsResolver
{
    public static PhysicsResults Resolve(Collider collider1, Collider collider2, List<Vector2>? workbench = null)
    {
        if (collider1.Data.IsMesh == collider2.Data.IsMesh)
        {
            if (collider1.Data.IsMesh)
                return ResolveMeshToMesh(collider1, collider2, workbench ?? new());
            else
                return ResolveCircleToCircle(collider1, collider2);
        }

        if (!collider1.Data.IsMesh)
            return ResolveCircleToMesh(collider1, collider2, workbench ?? new());

        PhysicsResults results = ResolveCircleToMesh(collider2, collider1, workbench ?? new());

        results.MinimumTranslation *= -1;

        return results;
    }

    public static PhysicsResults ResolveCircleToCircle(Collider collider1, Collider collider2)
    {
        float combinedRadius = collider1.Data.BoundingCircle.Radius + collider2.Data.BoundingCircle.Radius;
        Vector2 offset = collider1.Data.BoundingCircle.Center - collider2.Data.BoundingCircle.Center;
        float distance = offset.LengthSquared;

        if (distance <= combinedRadius * combinedRadius)
        {
            distance = MathF.Sqrt(distance);
            offset *= 1 / distance;

            return new PhysicsResults
            {
                MinimumTranslation = MathF.Max(0, combinedRadius - distance) * offset,
                Type = PhysicsResultsType.Resolution
            };
        }

        return new PhysicsResults { Type = PhysicsResultsType.NoInteraction };
    }

    public static PhysicsResults ResolveCircleToMesh(Collider collider1, Collider collider2, List<Vector2> workbench)
    {
        return new PhysicsResults { Type = PhysicsResultsType.NoInteraction };
    }

    public static PhysicsResults ResolveMeshToMesh(Collider collider1, Collider collider2, List<Vector2> workbench)
    {
        return new PhysicsResults { Type = PhysicsResultsType.NoInteraction };
    }
}
