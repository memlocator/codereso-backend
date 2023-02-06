namespace GameEngine.Math2D;

public readonly struct Aabb
{
    public readonly Vector2 Min;
    public readonly Vector2 Max;
    public Vector2 Center { get => 0.5f * (Min + Max); }
    public Vector2 Size { get => Max - Min; }
    public float Area { get => (Max.X - Min.X) * (Max.Y - Min.Y); }
    public float Perimeter { get => 2 * (Max.X - Min.X + Max.Y - Min.Y); }

    public Aabb() { }

    public Aabb(Vector2 min, Vector2 max)
    {
        Min = new Vector2(
            MathF.Min(min.X, max.X),
            MathF.Min(min.Y, max.Y),
            1
        );
        Max = new Vector2(
            MathF.Max(min.X, max.X),
            MathF.Max(min.Y, max.Y),
            1
        );
    }

    public bool Contains(Vector2 point)
    {
        return (
            point.X >= Min.X && point.X <= Max.X &&
            point.Y >= Min.Y && point.Y <= Max.Y
        );
    }

    public bool Intersects(Aabb other)
    {
        return new Aabb(Min, Max + other.Size).Contains(other.Max);
    }

    public Aabb Expand(float amount)
    {
        return new Aabb(Min - new Vector2(amount, amount), Max + new Vector2(amount, amount));
    }

    public Aabb Transform(Matrix2 matrix)
    {
        Vector2 translation = matrix.Translation;

        Span<float> min = stackalloc float[3] { Min.X, Min.Y, Min.Z };
        Span<float> max = stackalloc float[3] { Max.X, Max.Y, Max.Z };

        Span<float> outMin = stackalloc float[3] { translation.X, translation.Y, translation.Z };
        Span<float> outMax = stackalloc float[3] { translation.X, translation.Y, translation.Z };

        for (int i = 0; i < 2; ++i)
        {
            for (int j = 0; j < 2; ++j)
            {
                float a = matrix[i, j] * min[j];
                float b = matrix[i, j] * max[j];

                if (a > b)
                {
                    float c = a;
                    a = b;
                    b = c;
                }

                outMin[i] += a;
                outMax[i] += b;
            }
        }

        return new Aabb(
            new Vector2(outMin[0], outMin[1], outMin[2]),
            new Vector2(outMax[0], outMax[1], outMax[2])
        );
    }

    public override bool Equals(object? obj)
    {
        if (obj is Aabb vector)
        {
            return Equals(vector);
        }

        return false;
    }

    public bool Equals(Aabb other)
    {
        return Min == other.Min && Max == other.Max;
    }

    public bool Equals(Aabb other, float epsilon)
    {
        return Min.Equals(other.Min, epsilon) && Max.Equals(other.Max, epsilon);
    }

    public static bool operator ==(Aabb left, Aabb right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Aabb left, Aabb right)
    {
        return !left.Equals(right);
    }

    public override int GetHashCode()
    {
        int hash = 17;

        hash = hash * 23 + Min.GetHashCode();
        hash = hash * 23 + Max.GetHashCode();

        return hash;
    }

    public static Aabb Compute(List<Vector2> points)
    {
        if (points.Count == 0)
            return new();

        float minX = points[0].X;
        float minY = points[0].Y;
        float maxX = minX;
        float maxY = minY;

        for (int i = 1; i < points.Count; i++)
        {
            minX = MathF.Min(minX, points[i].X);
            minY = MathF.Min(minY, points[i].Y);

            maxX = MathF.Max(maxX, points[i].X);
            maxY = MathF.Max(maxY, points[i].Y);
        }

        return new Aabb(new Vector2(minX, minY, 1), new Vector2(maxX, maxY, 1));
    }

    public override string ToString()
    {
        return $"[ min: ({Min.X}, {Min.Y}), max: ({Max.X}, {Max.Y}) ]";
    }
}
