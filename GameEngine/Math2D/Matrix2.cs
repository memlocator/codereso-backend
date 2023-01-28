using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Math2D;
public readonly struct Matrix2
{
    public readonly float[] Data = new float[9];
    public Vector2 Right { get => new(Data[0], Data[1]); }
    public Vector2 Up { get => new(Data[3], Data[4]); }
    public Vector2 Translation { get => new(Data[6], Data[7], 1); }
    public Vector2 Scale2 { get => new(Right.Length, Up.Length); }
    public Vector2 Scale2Squared { get => new(Right.LengthSquared, Up.LengthSquared); }
    public float Rotation { get => MathF.Atan2(Data[4], Data[3]); }
    public float Determinant  { get => Data[0] * Data[4] - Data[1] * Data[3]; }
    public Matrix2 Inverse { get => GetInverse(); }

    public Matrix2()
    {
        Data[0] = 1;
        Data[3] = 1;
        Data[6] = 1;
    }
    public Matrix2(float[] data)
    {
        if (data.Length != 9 || data.Rank != 1)
            throw new RankException($"Attempted to create Matrix2 of rank {data.Rank} length {data.Length}. Expected rank 1, length 9.");

        Data = data;
    }
    public Matrix2(Vector2 position)
    {
        Data[0] = 1;
        Data[4] = 1;

        Data[6] = position.X;
        Data[7] = position.Y;
        Data[8] = 1;
    }
    public Matrix2(Vector2 position, Vector2 right, Vector2 up)
    {
        Data[0] = right.X;
        Data[1] = right.Y;

        Data[3] = up.X;
        Data[4] = up.Y;

        Data[6] = position.X;
        Data[7] = position.Y;
        Data[8] = 1;
    }
    public Matrix2(Vector2 position, Vector2 scale)
    {
        Data[0] = scale.X;
        Data[4] = scale.Y;

        Data[6] = position.X;
        Data[7] = position.Y;
        Data[8] = 1;
    }
    public Matrix2(Vector2 position, float rotation)
    {
        float sin = MathF.Sin(rotation);
        float cos = MathF.Cos(rotation);

        Data[0] = cos;
        Data[1] = -sin;

        Data[3] = sin;
        Data[4] = cos;

        Data[6] = position.X;
        Data[7] = position.Y;
        Data[8] = 1;
    }
    public Matrix2(Vector2 position, Vector2 scale, float rotation)
    {
        float sin = MathF.Sin(rotation);
        float cos = MathF.Cos(rotation);

        Data[0] = scale.X * cos;
        Data[1] = scale.X * -sin;

        Data[3] = scale.Y * sin;
        Data[4] = scale.Y * cos;

        Data[6] = position.X;
        Data[7] = position.Y;
        Data[8] = 1;
    }

    public Matrix2 Transpose()
    {
        float[] data = new float[9];

        for (int i = 0; i < 9; ++i)
        {
            int x = i % 3;
            int y = i / 3;

            data[i] = Data[IndexOf(y, x)];
        }

        return new(data);
    }

    public Matrix2 Translate(float x, float y)
    {
        return new(Translation + new Vector2(x, y), Right, Up);
    }

    public Matrix2 Translate(Vector2 offset)
    {
        return Translate(offset.X, offset.Y);
    }

    public Matrix2 Rotate(float angle)
    {
        return new(Translation.Rotate(angle), Right.Rotate(angle), Up.Rotate(angle));
    }

    public Matrix2 Scale(float scale)
    {
        return Scale(scale, scale);
    }

    public Matrix2 Scale(float x, float y)
    {
        return new(Translation, x * Right, y * Up);
    }

    public Matrix2 Scale(Vector2 scale)
    {
        return Scale(scale.X, scale.Y);
    }

    public Matrix2 GetInverse()
    {
        float determinantInverse = 1 / Determinant;

        Vector2 right = determinantInverse * Right;
        Vector2 up = determinantInverse * Up;
        Vector2 translation = determinantInverse * Translation;

        right = new(-up.Y, right.Y);
        up = new(up.X, -right.X);

        translation = new(
            translation.X * right.X + translation.Y * up.X,
            translation.X * right.Y + translation.Y * up.Y
        );

        return new(-translation, right, up);
    }

    public static Matrix2 operator+(Matrix2 left, Matrix2 right)
    {
        float[] data = new float[9];

        for (int i = 0; i < 9; ++i)
        {
            data[i] = left.Data[i] + right.Data[i];
        }

        return new(data);
    }

    public static Matrix2 operator -(Matrix2 left, Matrix2 right)
    {
        float[] data = new float[9];

        for (int i = 0; i < 9; ++i)
        {
            data[i] = left.Data[i] + right.Data[i];
        }

        return new(data);
    }

    public static Matrix2 operator +(Matrix2 matrix)
    {
        float[] data = new float[9];

        for (int i = 0; i < 9; ++i)
        {
            data[i] = -matrix.Data[i];
        }

        return new(data);
    }

    public static Matrix2 operator *(Matrix2 left, Matrix2 right)
    {
        float[] data = new float[9];

        for (int i = 0; i < 9; ++i)
        {
            int x = i % 3;
            int y = i / 3;

            for (int j = 0; j < 3; ++j)
            {
                data[i] += left[j, y] * right[x, j];
            }
        }

        return new(data);
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is Matrix2 vector)
        {
            return Equals(vector);
        }

        return false;
    }

    public bool Equals(Matrix2 other)
    {
        for (int i = 0; i < 9; ++i)
        {
            if (!Utils.IsFloatClose(Data[i], other.Data[i]))
            {
                return false;
            }
        }

        return true;
    }

    public bool Equals(Matrix2 other, float epsilon)
    {
        for (int i = 0; i < 9; ++i)
        {
            if (!Utils.IsFloatClose(Data[i], other.Data[i], epsilon))
            {
                return false;
            }
        }

        return true;
    }

    public override string ToString()
    {
        return $"r:[ {Data[0]} {Data[1]} {Data[2]} ] u:[ {Data[3]} {Data[4]} {Data[5]} ] t:[ {Data[6]} {Data[7]} {Data[8]} ]";
    }

    public float this[int x, int y]
    {
        get => Data[IndexOf(x, y)];
    }

    public static int IndexOf(int x, int y)
    {
        return x + 3 * y;
    }
}