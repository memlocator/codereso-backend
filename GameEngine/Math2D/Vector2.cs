using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Math2D;

public class Utils
{
    public const float PreciseEpsilonF = 1e-6f;
    public const float LooseEpsilonF = 1e-4f;

    public static bool IsFloatZero(float value, float epsilon = PreciseEpsilonF)
    {
        return value > -epsilon && value < epsilon;
    }

    public static bool IsFloatClose(float x, float y, float epsilon = PreciseEpsilonF)
    {
        return IsFloatZero(x - y, epsilon);
    }

    public const double StrictEpsilonD = 1e-12d;
    public const double PreciseEpsilonD = 1e-8d;
    public const double LooseEpsilonD = 1e-4d;

    public static bool IsDoubleZero(double value, double epsilon = StrictEpsilonD)
    {
        return value > -epsilon && value < epsilon;
    }

    public static bool IsDoubleClose(double x, double y, double epsilon = StrictEpsilonD)
    {
        return IsDoubleZero(x - y, epsilon);
    }
}

public readonly struct Vector2
{
    public readonly float X = 0;
    public readonly float Y = 0;
    public readonly float Z = 0;

    public float LengthSquared { get => Dot(this); }
    public float Length { get => MathF.Sqrt(LengthSquared); }
    public Vector2 Unit { get => (1 / Length) * this; }
    public float Angle { get => MathF.Atan2(Y, X); }

    public Vector2(float x = 0, float y = 0, float z = 0)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public float Dot(Vector2 other)
    {
        return X * other.X + Y * other.Y;
    }

    public Vector2 Scale(Vector2 other)
    {
        return new(X * other.X, Y * other.Y, Z);
    }

    public Vector2 Rotate(float angle)
    {
        float sin = MathF.Sin(angle);
        float cos = MathF.Cos(angle);

        return new(
             X * cos + Y * sin,
            -X * sin + Y * cos,
            Z
        );
    }

    public static Vector2 operator+(Vector2 left, Vector2 right)
    {
        return new(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    public static Vector2 operator -(Vector2 left, Vector2 right)
    {
        return new(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
    }

    public static float operator *(Vector2 left, Vector2 right)
    {
        return left.Dot(right);
    }

    public static Vector2 operator *(float left, Vector2 right)
    {
        return new(left * right.X, left * right.Y, left * right.Z);
    }

    public static Vector2 operator *(Vector2 left, float right)
    {
        return new(left.X * right, left.Y * right, left.Z * right);
    }

    public static Vector2 operator -(Vector2 vector)
    {
        return new(-vector.X, -vector.Y, -vector.Z);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Vector2 vector)
        {
            return Equals(vector);
        }

        return false;
    }

    public bool Equals(Vector2 other)
    {
        return Utils.IsFloatClose(X, other.X) && Equals(Y, other.Y) && Utils.IsFloatClose(Z, other.Z);
    }

    public bool Equals(Vector2 other, float epsilon)
    {
        return Utils.IsFloatClose(X, other.X, epsilon) && Utils.IsFloatClose(Y, other.Y, epsilon) && Utils.IsFloatClose(Z, other.Z, epsilon);
    }

    public override int GetHashCode()
    {
        int hash = 17;

        hash = hash * 23 + X.GetHashCode();
        hash = hash * 23 + Y.GetHashCode();
        hash = hash * 23 + Z.GetHashCode();

        return hash;
    }

    public override string ToString()
    {
        if (Utils.IsFloatZero(Z))
        {
            return $"<{X}, {Y}>";
        }

        if (Utils.IsFloatClose(Z, 1))
        {
            return $"({X}, {Y})";
        }

        return $"({X}, {Y}, {Z})";
    }

    public float this[int key]
    {
        get => key switch
        {
            0 => X,
            1 => Y,
            2 => Z,
            _ => throw new IndexOutOfRangeException($"Attempt to index Vector2 with index {key}")
        };
    }
}