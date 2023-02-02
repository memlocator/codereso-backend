using GameEngine.Math2D;
using GameEngine.ECS;

namespace GameEngine.Physics;

public class ColliderAsset : Component
{
    public List<Vector2> Vertices { get; private set; } = new();
    public List<Vector2> Normals { get; private set; } = new();
    public int TopVertex { get; private set; } = -1;
    public int BottomVertex { get; private set; } = -1;
    public Aabb BoundingBox { get; private set; }
    public BoundingCircle BoundingCircle { get; private set; }


    public void SetVertices(List<Vector2> vertices)
    {
        Vertices = vertices;
        Normals = new(Vertices.Count);

        ComputeAabb();
        ComputeBoundingCircle();

        if (!IsClockwise())
        {
            FlipDirection();
        }

        ComputeNormals();
    }

    public void ComputeAabb()
    {
        if (Vertices.Count == 0)
        {
            BoundingBox = new Aabb();

            return;
        }

        Vector2 min = Vertices[0];
        Vector2 max = Vertices[0];

        for (int i = 1; i < Vertices.Count; i++)
        {
            Vector2 vertex = Vertices[i];

            min = new Vector2(MathF.Min(min.X, vertex.X), MathF.Min(min.Y, vertex.Y), 1);
            max = new Vector2(MathF.Max(max.X, vertex.X), MathF.Max(max.Y, vertex.Y), 1);
        }

        BoundingBox = new Aabb(min, max);
    }

    public void ComputeBoundingCircle()
    {
        if (Vertices.Count == 0)
        {
            BoundingCircle = new BoundingCircle();

            return;
        }

        Vector2 center = BoundingBox.Center;

        float farthestDistance = 0;

        for (int i = 0; i < Vertices.Count; ++i)
        {
            float distance = (Vertices[i] - center).LengthSquared;

            if (distance > farthestDistance)
            {
                farthestDistance = distance;
            }
        }

        BoundingCircle = new BoundingCircle(center, MathF.Sqrt(farthestDistance));
    }

    public bool IsClockwise()
    {
        if (Vertices.Count < 3)
        {
            return true;
        }

        Vector2 side = Vertices[1] - Vertices[0];

        return side * (Vertices[0] - BoundingBox.Center) > 0;
    }

    public void FlipDirection()
    {
        for (int i = 1; i < Vertices.Count; ++i)
        {
            Vector2 first = Vertices[i];

            Vertices[i] = Vertices[Vertices.Count - i];

            Vertices[Vertices.Count - i] = first;
        }
    }

    public void ComputeNormals()
    {
        Vertices.Clear();

        for (int i = 0; i < Vertices.Count; ++i)
        {
            Vertices.Add((Vertices[(i + 1) % Vertices.Count] - Vertices[i]).LeftNormal.Unit);
        }
    }
}
