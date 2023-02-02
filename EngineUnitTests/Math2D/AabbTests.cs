using GameEngine.Math2D;
using System.Runtime.CompilerServices;

namespace EngineUnitTests.Math2D;

public class AabbTests
{
    public Random Generator = new(); 

    public float Next(float min, float max)
    {
        return min + (max - min) * (float)Generator.NextDouble();
    }

    public Vector2 NextIn(float minX, float minY, float maxX, float maxY)
    {
        return new Vector2(Next(minX, maxX), Next(minY, maxY));
    }
    
    [Fact]
    public void TestProperties()
    {
        for (int i = 0; i < 1000; ++i)
        {
            Aabb box = new Aabb(
                new Vector2(Next(-100, 100), Next(-100, 100), Next(-10, 10)),
                new Vector2(Next(-100, 100), Next(-100, 100), Next(-10, 10))
            );

            Assert.Equal(1, box.Min.Z);
            Assert.Equal(1, box.Max.Z);

            Assert.True(box.Min.X <= box.Max.X);
            Assert.True(box.Min.Y <= box.Max.Y);

            Assert.True(box.Contains(box.Center));

            Assert.True(box.Equals(new Aabb(box.Min, box.Min + box.Size), Utils.LooseEpsilonF));

            Assert.NotEqual(box, new Aabb(box.Min, box.Max + new Vector2(1, 0, 0)));
            Assert.NotEqual(box, new Aabb(box.Min, box.Max + new Vector2(0, 1, 0)));
            Assert.NotEqual(box, new Aabb(box.Min - new Vector2(1, 0, 0), box.Max));
            Assert.NotEqual(box, new Aabb(box.Min - new Vector2(0, 1, 0), box.Max));

            float sideX = box.Max.X - box.Min.X;
            float sideY = box.Max.Y - box.Min.Y;

            Assert.True(Utils.IsFloatClose(sideY, Utils.SafeDivide(box.Area, sideX, Utils.PreciseEpsilonF, false), Utils.LooseEpsilonF));
            Assert.True(Utils.IsFloatClose(sideX, Utils.SafeDivide(box.Area, sideY, Utils.PreciseEpsilonF, false), Utils.LooseEpsilonF));
            Assert.True(Utils.IsFloatClose(sideY, 0.5f * box.Perimeter - sideX, Utils.LooseEpsilonF));
            Assert.True(Utils.IsFloatClose(sideX, 0.5f * box.Perimeter - sideY, Utils.LooseEpsilonF));
            Assert.True(Utils.IsFloatClose(box.Size.X * box.Size.Y, box.Area, Utils.LooseEpsilonF));
            Assert.Equal(sideX, box.Size.X);
            Assert.Equal(sideY, box.Size.Y);
            Assert.True(Utils.IsFloatClose((box.Center - box.Min).Length, (0.5f * (box.Max - box.Min)).Length, Utils.LooseEpsilonF));

            float amount = Next(1.1f, 10);
            Aabb expandedBox = box.Expand(amount);

            Assert.True(Utils.IsFloatClose((box.Size.X + 2 * amount) * (box.Size.Y + 2 * amount), expandedBox.Area, 0.01f));
            Assert.True(Utils.IsFloatClose(box.Perimeter + 8 * amount, expandedBox.Perimeter, Utils.LooseEpsilonF));

            Assert.True(expandedBox.Intersects(box));
            Assert.True(expandedBox.Contains(box.Min));
            Assert.True(expandedBox.Contains(box.Max));
            Assert.False(box.Contains(expandedBox.Min));
            Assert.False(box.Contains(expandedBox.Max));
            Assert.True(box.Contains(expandedBox.Center));

            Matrix2 rotation = Matrix2.NewRotation(Next(-6 * MathF.PI, 6 * MathF.PI));
            Matrix2 transform = Matrix2.NewTranslation(expandedBox.Center) * rotation * Matrix2.NewScale(Next(1.1f, 10), Next(1.1f, 10)) * Matrix2.NewTranslation(-expandedBox.Center);

            Aabb transformedBox = expandedBox.Transform(transform);

            Assert.True(transformedBox.Intersects(expandedBox));
            Assert.True(transformedBox.Contains(expandedBox.Center));
            Assert.True(expandedBox.Contains(transformedBox.Center));
            Assert.True(transformedBox.Contains(expandedBox.Center + 0.5f * expandedBox.Size.X * rotation.Right));
            Assert.True(transformedBox.Contains(expandedBox.Center - 0.5f * expandedBox.Size.X * rotation.Right));
            Assert.True(transformedBox.Contains(expandedBox.Center + 0.5f * expandedBox.Size.Y * rotation.Up));
            Assert.True(transformedBox.Contains(expandedBox.Center - 0.5f * expandedBox.Size.Y * rotation.Up));
        }
    }

    [Theory]
    [InlineData(0.4f, 1, false)]
    [InlineData(0.51f, 1, true)]
    [InlineData(0.5f, 2.1f, false)]
    [InlineData(1.051f, 2.1f, true)]
    public void TestIntersection(float size, float sideOffset, bool allIntersects)
    {
        Vector2 offset;
        Vector2 boxSize = new Vector2(size, size);
        Vector2 boxOffset = new Vector2(sideOffset, sideOffset);

        for (int i = 0; i < 10000; ++i)
        {
            offset = NextIn(-1000, 1000, -1000, 1000);

            Aabb centerBox = new Aabb(offset - boxSize, offset + boxSize);

            for (float x = -1; x < 2; ++x)
            {
                for (float y = -1; y < 2; ++y)
                {
                    Aabb otherBox = new Aabb(offset - boxSize + boxOffset.Scale(x, y), offset + boxSize + boxOffset.Scale(x, y));

                    Assert.Equal((x == 0 && y == 0) || allIntersects, centerBox.Intersects(otherBox));
                }
            }
        }
    }

    [Fact]
    public void TestCompute()
    {
        const int iterations = 1000;
        List<Vector2> vertices = new(iterations);

        for (int i = 0; i < 100; ++i)
        {
            Vector2 offset = NextIn(-1000, 1000, -1000, 1000);
            Vector2 size = NextIn(0.1f, 1000, 0.1f, 1000);
            Aabb outerRange = new Aabb(size - 0.5f * offset, size + 0.5f * offset);

            vertices.Clear();

            for (int j = 0; j < iterations; ++j)
            {
                vertices.Add(NextIn(outerRange.Min.X, outerRange.Max.X, outerRange.Min.Y, outerRange.Max.Y));
            }

            Aabb computed = Aabb.Compute(vertices);

            Assert.True(outerRange.Intersects(computed));
            Assert.True(outerRange.Contains(computed.Min));
            Assert.True(outerRange.Contains(computed.Max));

            foreach (Vector2 vertex in vertices)
            {
                Assert.True(computed.Contains(vertex));
            }
        }

        for (int i = 0; i < 1000; ++i)
        {
            Vector2 offset = NextIn(-1000, 1000, -1000, 1000);
            Vector2 size = NextIn(0.1f, 1000, 0.1f, 1000);
            Aabb outerRange = new Aabb(size - 0.5f * offset, size + 0.5f * offset);

            vertices.Clear();

            for (int j = 0; j < 5 + Generator.Next() % 25; ++j)
            {
                vertices.Add(NextIn(outerRange.Min.X, outerRange.Max.X, outerRange.Min.Y, outerRange.Max.Y));
            }

            Aabb computed = Aabb.Compute(vertices);

            Assert.True(outerRange.Intersects(computed));
            Assert.True(outerRange.Contains(computed.Min));
            Assert.True(outerRange.Contains(computed.Max));

            foreach (Vector2 vertex in vertices)
            {
                Assert.True(computed.Contains(vertex));
            }
        }
    }
}
