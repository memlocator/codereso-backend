using GameEngine.Physics;
using GameEngine.Math2D;

namespace EngineUnitTests.Math2D;

public class ColliderDataTests
{
    public Random Generator = new();

    public float Next(float min, float max)
    {
        return min + (max - min) * (float)Generator.NextDouble();
    }

    public Vector2 NextIn(float minX, float maxX, float minY, float maxY)
    {
        return new Vector2(Next(minX, maxX), Next(minY, maxY));
    }

    List<Vector2> Vertices = new();

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
        return Vertices[Wrap(index)];
    }

    public float DotAt(int index)
    {
        return (VertexAt(index) - VertexAt(index - 1)).LeftNormal * (VertexAt(index + 1) - VertexAt(index));
    }

    public float DotAtNext(int index)
    {
        return (VertexAt(index - 1) - VertexAt(index)) * (VertexAt(index + 1) - VertexAt(index)).LeftNormal;
    }

    [Fact]
    public void TestMesh()
    {
        return;

        Vertices = new(103);
        Vector2 center;
        ColliderData data;
    }
}
