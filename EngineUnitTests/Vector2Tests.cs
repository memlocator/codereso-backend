using GameEngine.Math2D;

namespace EngineUnitTests;

public class Vector2Tests
{
    [Theory]
    [InlineData(Utils.LooseEpsilonF)]
    [InlineData(Utils.PreciseEpsilonF)]
    public void TestIsFloatZero(float epsilon)
    {
        float increment = epsilon / 16;

        for (float x = -2 * epsilon; x <= 2 * epsilon; x += increment)
        {
            bool isNearZero = x > -epsilon && x < epsilon;

            Assert.True(isNearZero == Utils.IsFloatZero(x, epsilon));
        }
    }

    [Theory]
    [InlineData(Utils.LooseEpsilonF)]
    [InlineData(Utils.PreciseEpsilonF)]
    public void TestSafeDivide(float epsilon)
    {
        float increment = epsilon / 16;

        for (float x = -2 * epsilon; x <= 2 * epsilon; x += increment)
        {
            bool isNearZero = x > -epsilon && x < epsilon;
            bool returnedOne = Utils.SafeDivide(1, x, epsilon, true) == 1;
            bool returnedZero = Utils.SafeDivide(1, x, epsilon, false) == 0;
            bool returnedTwo = Utils.SafeDivide(2, x, epsilon, true) == 2;

            Assert.True(!isNearZero || returnedOne);
            Assert.True(!isNearZero || returnedZero);
            Assert.True(isNearZero == returnedTwo);
        }
    }

    [Theory]
    [InlineData(5, Utils.LooseEpsilonF)]
    [InlineData(5, Utils.PreciseEpsilonF)]
    [InlineData(-3, Utils.LooseEpsilonF)]
    [InlineData(-3, Utils.PreciseEpsilonF)]
    [InlineData(MathF.PI, Utils.LooseEpsilonF)]
    [InlineData(MathF.PI, Utils.PreciseEpsilonF)]
    [InlineData(MathF.E, Utils.LooseEpsilonF)]
    [InlineData(MathF.E, Utils.PreciseEpsilonF)]
    public void TestIsFloatEqualRange(float x, float epsilon)
    {
        float increment = epsilon / 16;

        for (float y = -2 * epsilon; y <= 2 * epsilon; y += increment)
        {
            float yValue = x + y;
            bool isNearZero = x - yValue > -epsilon && x - yValue < epsilon;

            Assert.True(isNearZero == Utils.IsFloatClose(x, yValue, epsilon));
        }
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(MathF.PI, MathF.E, 12356)]
    public void TestConstructor(float x, float y, float z)
    {
        Vector2 vector = new(x, y, z);

        Assert.True(vector.X == x && vector.Y == y && vector.Z == z);
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(1, 0, 0, 1)]
    [InlineData(0, 1, 0, 1)]
    [InlineData(0, 0, 1, 0)]
    [InlineData(0, 0, 5, 0)]
    [InlineData(0, 5, 0, 5)]
    [InlineData(5, 0, 0, 5)]
    [InlineData(-1, 0, 0, 1)]
    [InlineData(0, -1, 0, 1)]
    [InlineData(0, 0, -1, 0)]
    [InlineData(0, 0, -5, 0)]
    [InlineData(0, -5, 0, 5)]
    [InlineData(-5, 0, 0, 5)]
    [InlineData(1, 1, 0, 1.41421356237)]
    [InlineData(1, -1, 0, 1.41421356237)]
    [InlineData(-1, 1, 0, 1.41421356237)]
    [InlineData(-1, -1, 0, 1.41421356237)]
    [InlineData(-5, 3, 0, 5.83095189)]
    [InlineData(-5, 3, 16, 5.83095189)]
    public void TestLength(float x, float y, float z, float expected)
    {
        Vector2 vector = new(x, y, z);

        Assert.True(Utils.IsFloatClose(vector.Length, expected));
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 0, 0)]
    [InlineData(0, 1, 0)]
    [InlineData(0, 0, 1)]
    [InlineData(0, 0, 5)]
    [InlineData(0, 5, 0)]
    [InlineData(5, 0, 0)]
    [InlineData(-1, 0, 0)]
    [InlineData(0, -1, 0)]
    [InlineData(0, 0, -1)]
    [InlineData(0, 0, -5)]
    [InlineData(0, -5, 0)]
    [InlineData(-5, 0, 0)]
    [InlineData(1, 1, 0)]
    [InlineData(1, -1, 0)]
    [InlineData(-1, 1, 0)]
    [InlineData(-1, -1, 0)]
    [InlineData(-5, 3, 0)]
    [InlineData(-5, 3, 16)]
    public void TestEquals(float x, float y, float z)
    {
        Vector2 vector = new(x, y, z);
        Vector2 sameVector = new(x, y, z);

        Assert.True(vector == sameVector);
        Assert.False(vector != sameVector);
    }

    [Theory]
    [InlineData(0, 0, 0, 0.1, 0, 0)]
    [InlineData(0, 0, 0, 0, 0.1, 0)]
    [InlineData(0, 0, 0, 0, 0, 0.1)]
    [InlineData(0, 0, 5, 0.1, 0, 5)]
    [InlineData(0, 0, 5, 0, 0.1, 5)]
    [InlineData(0, 0, 5, 0, 0, 5.1)]
    [InlineData(0, 5, 0, 0.1, 5, 0)]
    [InlineData(0, 5, 0, 0, 5.1, 0)]
    [InlineData(0, 5, 0, 0, 5, 0.1)]
    [InlineData(5, 0, 0, 5.1, 0, 0)]
    [InlineData(5, 0, 0, 5, 0.1, 0)]
    [InlineData(5, 0, 0, 5, 0, 0.1)]
    public void TestNotEquals(float x, float y, float z, float x2, float y2, float z2)
    {
        Vector2 vector = new(x, y, z);
        Vector2 sameVector = new(x2, y2, z2);

        Assert.False(vector == sameVector);
        Assert.True(vector != sameVector);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 0, 0)]
    [InlineData(0, 1, 0)]
    [InlineData(0, 0, 1)]
    [InlineData(0, 0, 5)]
    [InlineData(0, 5, 0)]
    [InlineData(5, 0, 0)]
    [InlineData(-1, 0, 0)]
    [InlineData(0, -1, 0)]
    [InlineData(0, 0, -1)]
    [InlineData(0, 0, -5)]
    [InlineData(0, -5, 0)]
    [InlineData(-5, 0, 0)]
    [InlineData(1, 1, 0)]
    [InlineData(1, -1, 0)]
    [InlineData(-1, 1, 0)]
    [InlineData(-1, -1, 0)]
    [InlineData(-5, 3, 0)]
    [InlineData(-5, 3, 16)]
    public void TestSubscript(float x, float y, float z)
    {
        Vector2 vector = new(x, y, z);

        Assert.True(vector[0] == x && vector[1] == y && vector[2] == z);
    }

    [Theory]
    [InlineData(0, 0, 0, 0, 0, 0)]
    [InlineData(1, 0, 0, 1, 0, 0)]
    [InlineData(0, 1, 0, 0, 1, 0)]
    [InlineData(0, 0, 1, 0, 0, 1)]
    [InlineData(0, 0, 5, 0, 0, 5)]
    [InlineData(0, 5, 0, 0, 1, 0)]
    [InlineData(5, 0, 0, 1, 0, 0)]
    [InlineData(-1, 0, 0, -1, 0, 0)]
    [InlineData(0, -1, 0, 0, -1, 0)]
    [InlineData(0, 0, -1, 0, 0, -1)]
    [InlineData(0, 0, -5, 0, 0, -5)]
    [InlineData(0, -5, 0, 0, -1, 0)]
    [InlineData(-5, 0, 0, -1, 0, 0)]
    [InlineData(1, 1, 0, 0.707106781186548, 0.707106781186548, 0)]
    [InlineData(1, -1, 0, 0.707106781186548, -0.707106781186548, 0)]
    [InlineData(-1, 1, 0, -0.707106781186548, 0.707106781186548, 0)]
    [InlineData(-1, -1, 0, -0.707106781186548, -0.707106781186548, 0)]
    [InlineData(-5, 3, 0, -0.857492925712544, 0.514495755427527, 0)]
    [InlineData(-5, 3, 16, -0.857492925712544, 0.514495755427527, 2.74397736228)]
    public void TestUnit(float x, float y, float z, float x2, float y2, float z2)
    {
        Vector2 vector = new(x, y, z);
        Vector2 vector2 = new(x2, y2, z2);
        Vector2 vectorUnit = vector.Unit;

        Assert.True(vectorUnit == vector2);
    }

    [Theory]
    [InlineData(0, 0, 0, 0)]
    [InlineData(1, 0, 0, 1.570796326795)]
    [InlineData(0, 1, 0, 0)]
    [InlineData(0, 0, 1, 0)]
    [InlineData(0, 0, 5, 0)]
    [InlineData(0, 5, 0, 0)]
    [InlineData(5, 0, 0, 1.570796326795)]
    [InlineData(-1, 0, 0, -1.570796326795)]
    [InlineData(0, -1, 0, 3.141592653590)]
    [InlineData(0, 0, -1, 0)]
    [InlineData(0, 0, -5, 0)]
    [InlineData(0, -5, 0, 3.141592653590)]
    [InlineData(-5, 0, 0, -1.570796326795)]
    [InlineData(1, 1, 0, 0.785398163397)]
    [InlineData(1, -1, 0, 2.356194490192)]
    [InlineData(-1, 1, 0, -0.785398163397)]
    [InlineData(-1, -1, 0, -2.356194490192)]
    [InlineData(-5, 3, 0, -1.030376826524)]
    [InlineData(-5, 3, 16, -1.030376826524)]
    public void TestAngle(float x, float y, float z, float angle)
    {
        Vector2 vector = new(x, y, z);

        Assert.True(Utils.IsFloatClose(vector.Angle, angle));
    }

    [Theory]
    [InlineData(-5, 3, 16, 5, 1, 0)]
    [InlineData(-5, 3, 16, 0, 5, 1)]
    [InlineData(-5, 3, 16, 1, 0, 5)]
    public void TestScale(float x, float y, float z, float sx, float sy, float sz)
    {
        Vector2 vector = new(x, y, z);
        Vector2 scaled = vector.Scale(new Vector2(sx, sy, sz));

        Assert.True(vector.X * sx == scaled.X && vector.Y * sy == scaled.Y && vector.Z == scaled.Z);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 0, 0)]
    [InlineData(0, 1, 0)]
    [InlineData(0, 0, 1)]
    [InlineData(0, 0, 5)]
    [InlineData(0, 5, 0)]
    [InlineData(5, 0, 0)]
    [InlineData(-1, 0, 0)]
    [InlineData(0, -1, 0)]
    [InlineData(0, 0, -1)]
    [InlineData(0, 0, -5)]
    [InlineData(0, -5, 0)]
    [InlineData(-5, 0, 0)]
    [InlineData(1, 1, 0)]
    [InlineData(1, -1, 0)]
    [InlineData(-1, 1, 0)]
    [InlineData(-1, -1, 0)]
    [InlineData(-5, 3, 0)]
    [InlineData(-5, 3, 16)]
    public void TestRotate(float x, float y, float z)
    {
        Vector2 vector = new(x, y, z);
        float angle = vector.Angle;
        bool isOrigin = vector.IsOrigin();
        
        for (float theta = 0; theta < 6 * MathF.PI; theta += 1 / 32.0f)
        {
            Vector2 rotated = vector.Rotate(theta);
            float rotatedAngle = rotated.Angle;
            float wrappedAngle = Utils.WrapAngle(angle + theta);
            bool originTestPassed = isOrigin && rotatedAngle == 0;

            Assert.True(originTestPassed || Utils.IsFloatClose(rotatedAngle, wrappedAngle, Utils.LooseEpsilonF));
        }
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 0, 0)]
    [InlineData(0, 1, 0)]
    [InlineData(0, 0, 1)]
    [InlineData(0, 0, 5)]
    [InlineData(0, 5, 0)]
    [InlineData(5, 0, 0)]
    [InlineData(-1, 0, 0)]
    [InlineData(0, -1, 0)]
    [InlineData(0, 0, -1)]
    [InlineData(0, 0, -5)]
    [InlineData(0, -5, 0)]
    [InlineData(-5, 0, 0)]
    [InlineData(1, 1, 0)]
    [InlineData(1, -1, 0)]
    [InlineData(-1, 1, 0)]
    [InlineData(-1, -1, 0)]
    [InlineData(-5, 3, 0)]
    [InlineData(-5, 3, 16)]
    public void TestScale2(float x, float y, float z)
    {
        float scalar = 2;
        Vector2 vector = new(x, y, z);
        Vector2 scaled = 2 * vector;
        Vector2 scaled2 = vector * scalar;
        Vector2 correct = new(scalar * x, scalar* y, scalar * z);

        Assert.Equal(scaled, correct);
        Assert.Equal(scaled2, correct);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 0, 0)]
    [InlineData(0, 1, 0)]
    [InlineData(0, 0, 1)]
    [InlineData(0, 0, 5)]
    [InlineData(0, 5, 0)]
    [InlineData(5, 0, 0)]
    [InlineData(-1, 0, 0)]
    [InlineData(0, -1, 0)]
    [InlineData(0, 0, -1)]
    [InlineData(0, 0, -5)]
    [InlineData(0, -5, 0)]
    [InlineData(-5, 0, 0)]
    [InlineData(1, 1, 0)]
    [InlineData(1, -1, 0)]
    [InlineData(-1, 1, 0)]
    [InlineData(-1, -1, 0)]
    [InlineData(-5, 3, 0)]
    [InlineData(-5, 3, 16)]
    public void TestAdd(float x, float y, float z)
    {
        Vector2 vector = new(x, y, z);
        Vector2 offset = new(1, -2, 3);
        Vector2 changed = vector + offset;
        Vector2 changed2 = offset + vector;
        Vector2 correct = new(x + 1, y - 2, z + 3);

        Assert.Equal(changed, correct);
        Assert.Equal(changed2, correct);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 0, 0)]
    [InlineData(0, 1, 0)]
    [InlineData(0, 0, 1)]
    [InlineData(0, 0, 5)]
    [InlineData(0, 5, 0)]
    [InlineData(5, 0, 0)]
    [InlineData(-1, 0, 0)]
    [InlineData(0, -1, 0)]
    [InlineData(0, 0, -1)]
    [InlineData(0, 0, -5)]
    [InlineData(0, -5, 0)]
    [InlineData(-5, 0, 0)]
    [InlineData(1, 1, 0)]
    [InlineData(1, -1, 0)]
    [InlineData(-1, 1, 0)]
    [InlineData(-1, -1, 0)]
    [InlineData(-5, 3, 0)]
    [InlineData(-5, 3, 16)]
    public void TestSubtract(float x, float y, float z)
    {
        Vector2 vector = new(x, y, z);
        Vector2 offset = new(1, -2, 3);
        Vector2 changed = vector - offset;
        Vector2 changed2 = offset - vector;
        Vector2 correct = new(x - 1, y + 2, z - 3);
        Vector2 correct2 = new(1 - x, -2 - y, 3 - z);

        Assert.Equal(changed, correct);
        Assert.Equal(changed2, correct2);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 0, 0)]
    [InlineData(0, 1, 0)]
    [InlineData(0, 0, 1)]
    [InlineData(0, 0, 5)]
    [InlineData(0, 5, 0)]
    [InlineData(5, 0, 0)]
    [InlineData(-1, 0, 0)]
    [InlineData(0, -1, 0)]
    [InlineData(0, 0, -1)]
    [InlineData(0, 0, -5)]
    [InlineData(0, -5, 0)]
    [InlineData(-5, 0, 0)]
    [InlineData(1, 1, 0)]
    [InlineData(1, -1, 0)]
    [InlineData(-1, 1, 0)]
    [InlineData(-1, -1, 0)]
    [InlineData(-5, 3, 0)]
    [InlineData(-5, 3, 16)]
    public void TestDot(float x, float y, float z)
    {
        Vector2 vector = new(x, y, z);
        float angle = vector.Angle;
        bool isOrigin = vector.IsOrigin();
        Vector2 unit = vector.Unit;
        float length = vector.Length;

        for (float theta = 0; theta < 6 * MathF.PI; theta += 1 / 32.0f)
        {
            Vector2 rotated = new Vector2(0, 1).Rotate(theta);
            float dot = vector * rotated;
            float dot2 = unit * rotated;
            float wrappedAngle = Utils.WrapAngle(angle - theta);
            float cos = MathF.Cos(wrappedAngle);
            bool originTestPassed = isOrigin && dot == 0;

            Assert.True(originTestPassed || Utils.IsFloatClose(dot, length * cos, Utils.LooseEpsilonF));
            Assert.True(originTestPassed || Utils.IsFloatClose(dot2, cos, Utils.LooseEpsilonF));
        }
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 0, 0)]
    [InlineData(0, 1, 0)]
    [InlineData(0, 0, 1)]
    [InlineData(0, 0, 5)]
    [InlineData(0, 5, 0)]
    [InlineData(5, 0, 0)]
    [InlineData(-1, 0, 0)]
    [InlineData(0, -1, 0)]
    [InlineData(0, 0, -1)]
    [InlineData(0, 0, -5)]
    [InlineData(0, -5, 0)]
    [InlineData(-5, 0, 0)]
    [InlineData(1, 1, 0)]
    [InlineData(1, -1, 0)]
    [InlineData(-1, 1, 0)]
    [InlineData(-1, -1, 0)]
    [InlineData(-5, 3, 0)]
    [InlineData(-5, 3, 16)]
    public void TestNegate(float x, float y, float z)
    {
        Vector2 vector = new(x, y, z);
        Vector2 negated = -vector;
        Vector2 correct = new(-x, -y, -z);

        Assert.Equal(correct, negated);
    }
}