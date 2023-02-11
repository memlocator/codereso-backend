using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
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

    public static float SafeDivide(float num, float den, float epsilon = PreciseEpsilonF, bool useOne = true)
    {
        if (!IsFloatZero(den, epsilon))
        {
            return num / den;
        }

        return useOne ? num : 0;
    }

    public static float WrapAngle(float angle)
    {
        while (angle > MathF.PI)
        {
            angle -= 2 * MathF.PI;
        }

        while (angle < -MathF.PI)
        {
            angle += 2 * MathF.PI;
        }

        return angle;
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

    public static double SafeDivide(double num, double den, double epsilon = StrictEpsilonD, bool useOne = true)
    {
        if (!IsDoubleZero(den, epsilon))
        {
            return num / den;
        }

        return useOne ? num : 0;
    }

    public static double WrapAngle(double angle)
    {
        while (angle > Math.PI)
        {
            angle -= 2 * Math.PI;
        }

        while (angle < -Math.PI)
        {
            angle += 2 * Math.PI;
        }

        return angle;
    }
}

public class Vector2JsonConverter : JsonConverter<Vector2>
{
    public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new Vector2();
    }

    public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
    {
        //writer.WriteStringValue($"{{x: {value.X}, y: {value.Y}}}");
        writer.WriteStartObject();
        writer.WritePropertyName("x");
        writer.WriteNumberValue(value.X); // if this is a thing
        writer.WritePropertyName("y");
        writer.WriteNumberValue(value.Y); // if this is a thing
        writer.WriteEndObject();
    }
}

[JsonConverter(typeof(Vector2JsonConverter))]
public readonly struct Vector2 : IEquatable<Vector2>
{
    public readonly float X = 0;
    public readonly float Y = 0;
    public readonly float Z = 0;

    public float LengthSquared { get => Dot(this); }
    public float Length { get => MathF.Sqrt(LengthSquared); }
    public Vector2 Unit { get => Utils.SafeDivide(1, Length) * this; }
    public float Angle { get => GetAngle(); }
    public Vector2 LeftNormal { get => new Vector2(-Y, X); }
    public Vector2 RightNormal { get => new Vector2(Y, -X); }

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

    public Vector2 Scale(float x, float y)
    {
        return new(X * x, Y * y, Z);
    }

    public Vector2 Scale(Vector2 other)
    {
        return Scale(other.X, other.Y);
    }

    public bool IsOrigin(float epsilon = Utils.PreciseEpsilonF)
    {
        return Utils.IsFloatZero(X, epsilon) && Utils.IsFloatZero(Y, epsilon);
    }

    public float GetAngle(float epsilon = Utils.PreciseEpsilonF)
    {
        if (IsOrigin(epsilon))
        {
            return 0;
        }

        return MathF.Atan2(X, Y);
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

    public override bool Equals(object? obj)
    {
        if (obj is Vector2 vector)
        {
            return Equals(vector);
        }

        return false;
    }

    public bool Equals(Vector2 other)
    {
        return Utils.IsFloatClose(X, other.X) && Utils.IsFloatClose(Y, other.Y) && Utils.IsFloatClose(Z, other.Z);
    }

    public bool Equals(Vector2 other, float epsilon)
    {
        return Utils.IsFloatClose(X, other.X, epsilon) && Utils.IsFloatClose(Y, other.Y, epsilon) && Utils.IsFloatClose(Z, other.Z, epsilon);
    }

    public static bool operator==(Vector2 left, Vector2 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector2 left, Vector2 right)
    {
        return !left.Equals(right);
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