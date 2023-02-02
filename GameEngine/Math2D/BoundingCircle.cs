namespace GameEngine.Math2D;

public struct BoundingCircle
{
    public float RadiusSquared { get => Radius * Radius; }
    public float Radius;
    public Vector2 Center;

    public BoundingCircle() { }

    public BoundingCircle(Vector2 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public bool Contains(Vector2 point)
    {
        return (point - Center).LengthSquared <= RadiusSquared;
    }

    public bool Intersects(BoundingCircle circle)
    {
        float radius = circle.Radius + Radius;

        return (circle.Center - Center).LengthSquared <= radius * radius;
    }

    public BoundingCircle Transform(Matrix2 transformation)
    {
        Vector2 newCenter = transformation * Center;
        Vector2 size = transformation.Scale2;
        float newRadius = Radius * MathF.Max(size.X, size.Y);

        return new BoundingCircle(newCenter, newRadius);
    }

    public static BoundingCircle Compute(List<Vector2> points)
    {
        if (points.Count == 0)
            return new();

        Aabb boundingBox = Aabb.Compute(points);

        return Compute(points, boundingBox.Center);
    }

    public static BoundingCircle Compute(List<Vector2> points, Vector2 center)
    {
        if (points.Count == 0)
            return new();

        float farthestSquared = 0;

        foreach (Vector2 point in points)
        {
            farthestSquared = MathF.Max(farthestSquared, (point - center).LengthSquared);
        }

        return new BoundingCircle(center, MathF.Sqrt(farthestSquared) + Utils.LooseEpsilonF);
    }
}
