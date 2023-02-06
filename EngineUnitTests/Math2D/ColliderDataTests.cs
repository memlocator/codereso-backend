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

        for (int i = 0; i < 1000; ++i)
        {
            int meshSize = 3 + Generator.Next() % 100;
            center = NextIn(-1000, 1000, -1000, 1000);
            float radius = Next(1, 250);

            Vertices.Clear();

            float increment = (float)1 / (float)meshSize * 2 * MathF.PI;

            for (int j = 0; j < meshSize; ++j)
            {
                float angle = increment * ((float)j + Next(-0.25f, 0.25f));
                float offset = Next(0, 0.5f);

                offset *= offset * offset;
                offset = 1 - offset;

                Vertices.Add(new Vector2(0, 1).Rotate(angle) * offset * radius + center);

                float max = 1;

                while (j > 1 && (DotAt(j - 1) > 0 || DotAtNext(j) > 0))
                {
                    max -= MathF.Max(0.5f, max - offset);

                    offset = Next(0, max - 0.5f);

                    if (max < 0.51f)
                    {
                        max = 0.5f;
                    }

                    offset *= offset * offset;
                    offset = max - offset;

                    Vertices[Vertices.Count - 1] = new Vector2(0, 1).Rotate(angle) * offset + center;
                }
            }

            data = new(Vertices);

            Assert.True(data.IsClockwise());
            
            for (int j = 0; j < meshSize; ++j)
            {
                Assert.True(data.Normals[j] * (data.Vertices[j] - center) > 0);
                Assert.True((data.VertexAt(j - 1) - data.VertexAt(j - 2)).LeftNormal * (data.VertexAt(j) - data.VertexAt(j - 1)) < 0);
            }
        }
    }
}
