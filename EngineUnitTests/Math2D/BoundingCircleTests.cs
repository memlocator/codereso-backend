using GameEngine.Math2D;

namespace EngineUnitTests.Math2D;

public class BoundingCircleTests
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

    [Fact]
    public void TestIntersection()
    {
        const float increment = 0.001f;

        for (int i = 0; i < 25; ++i)
        {
            BoundingCircle circle = new(NextIn(-1000, 1000, -1000, 1000), Next(0.1f, 100f));

            for (int j = 0; j < 25; ++j)
            {
                Vector2 offset = new Vector2(0, 1).Rotate(Next(-6 * MathF.PI, 6 * MathF.PI));

                for (float distance = 0.75f; distance < 1.25f; distance += increment)
                {
                    float offsetDistance = distance * circle.Radius;
                    Vector2 point = circle.Center + offsetDistance * offset;

                    bool insideRadius = distance <= 1;
                    bool closeToRadius = Utils.IsFloatClose(distance, 1, Utils.LooseEpsilonF);
                    bool circleContainsPoint = circle.Contains(point);

                    Assert.True(insideRadius == circleContainsPoint || closeToRadius);

                    for (int k = 0; k < 25; ++k)
                    {
                        float radius = Next(0.1f, 5);
                        float combinedRadius = circle.Radius + radius;

                        bool circlesShouldIntersect = offsetDistance <= combinedRadius;
                        bool circleEdgesAreClose = Utils.IsFloatClose(offsetDistance, combinedRadius, Utils.LooseEpsilonF);
                        bool circlesIntersect = circle.Intersects(new BoundingCircle(point, radius));

                        Assert.True(circlesShouldIntersect == circlesIntersect || circleEdgesAreClose);
                    }
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

            Aabb computedBox = Aabb.Compute(vertices);
            BoundingCircle computed = BoundingCircle.Compute(vertices, computedBox.Center);

            foreach (Vector2 vertex in vertices)
            {
                Assert.True(computed.Contains(vertex));
            }

            vertices.Clear();

            vertices.Add(computedBox.Center + 0.5f * computedBox.Size.Scale( 1,  1));
            vertices.Add(computedBox.Center + 0.5f * computedBox.Size.Scale( 1, -1));
            vertices.Add(computedBox.Center + 0.5f * computedBox.Size.Scale(-1,  1));
            vertices.Add(computedBox.Center + 0.5f * computedBox.Size.Scale(-1, -1));

            Matrix2 rotation = Matrix2.NewRotation(Next(-6 * MathF.PI, 6 * MathF.PI));
            Matrix2 transform = Matrix2.NewTranslation(computedBox.Center) * rotation * Matrix2.NewScale(Next(1.1f, 10), Next(1.1f, 10)) * Matrix2.NewTranslation(-computedBox.Center);

            Aabb transformedBox = computedBox.Transform(transform);
            BoundingCircle transformedCircle = BoundingCircle.Compute(vertices).Transform(transform);

            Assert.True(transformedCircle.Contains(computedBox.Center + 0.5f * computedBox.Size.Scale( 1,  1)));
            Assert.True(transformedCircle.Contains(computedBox.Center + 0.5f * computedBox.Size.Scale( 1, -1)));
            Assert.True(transformedCircle.Contains(computedBox.Center + 0.5f * computedBox.Size.Scale(-1,  1)));
            Assert.True(transformedCircle.Contains(computedBox.Center + 0.5f * computedBox.Size.Scale(-1, -1)));
        }
    }
}
