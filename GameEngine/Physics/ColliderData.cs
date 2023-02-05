using GameEngine.Math2D;

namespace GameEngine.Physics;

public struct ColliderData
{
    public List<Vector2> Vertices { get; private set; } = new();
    public List<Vector2> Normals { get; private set; } = new();
    public int TopVertex { get; private set; } = -1;
    public int BottomVertex { get; private set; } = -1;
    public Aabb BoundingBox { get; private set; }
    public BoundingCircle BoundingCircle { get; private set; }

    public ColliderData() { }
    public ColliderData(List<Vector2> vertices, Matrix2? optionalTransform = null)
    {
        SetVertices(vertices, optionalTransform);
    }

    public void SetVertices(List<Vector2> vertices, Matrix2? optionalTransform = null)
    {
        Vertices = new(vertices.Count);
        Normals = new(Vertices.Count);

        Vertices.AddRange(vertices);

        if (optionalTransform is Matrix2 transform)
        {
            for (int i = 0; i < Vertices.Count; ++i)
            {
                Vertices[i] = transform * Vertices[i];
            }
        }

        BoundingBox = Aabb.Compute(vertices);
        BoundingCircle = BoundingCircle.Compute(vertices, BoundingBox.Center);

        if (!IsClockwise())
        {
            FlipDirection();
        }

        BottomVertex = -1;
        TopVertex = -1;

        for (int i = 0; i < Vertices.Count; ++i)
        {
            if (Vertices[i].Y == BoundingBox.Min.Y)
            {
                BottomVertex = i;
            }

            if (Vertices[i].Y == BoundingBox.Max.Y)
            {
                TopVertex = i;
            }
        }

        ComputeNormals();
    }

    public bool IsClockwise()
    {
        if (Vertices.Count < 3)
        {
            return true;
        }

        Vector2 side = Vertices[1] - Vertices[0];

        return side.LeftNormal * (Vertices[0] - BoundingBox.Center) > 0;
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
        Normals.Clear();

        for (int i = 0; i < Vertices.Count; ++i)
        {
            Normals.Add((Vertices[(i + 1) % Vertices.Count] - Vertices[i]).LeftNormal.Unit);
        }
    }

    public int Wrap(int index)
    {
        if (Vertices.Count == 0)
        {
            return -1;
        }

        index %= Vertices.Count;

        if (index < 0)
        {
            index += Vertices.Count;
        }

        return index;
    }

    public Vector2 VertexAt(int index)
    {
        if (Vertices.Count == 0)
        {
            throw new ArgumentException($"Attempt to index 0 size mesh with index {index}");
        }

        return Vertices[Wrap(index)];
    }

    public Vector2 NormalAt(int index)
    {
        if (Normals.Count == 0)
        {
            throw new ArgumentException($"Attempt to index 0 size mesh with index {index}");
        }

        return Normals[Wrap(index)];
    }
}
