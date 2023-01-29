using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameEngine.Math2D;
public readonly struct Matrix2
{
    public readonly float[] Data = new float[9];
    public Vector2 Right { get => new(this[0], this[1]); }
    public Vector2 Up { get => new(this[3], this[4]); }
    public Vector2 Translation { get => new(this[6], this[7], 1); }
    public Vector2 Scale2 { get => new(Right.Length, Up.Length); }
    public Vector2 Scale2Squared { get => new(Right.LengthSquared, Up.LengthSquared); }
    public float Rotation { get => GetRotation(); }
    public float Determinant  { get => this[0] * this[4] - this[1] * this[3]; }
    public Matrix2 Inverse { get => GetInverse(); }

    public Matrix2()
    {
        Data[0] = 1;
        Data[4] = 1;
        Data[8] = 1;
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
    public Matrix2(Vector2 position, Vector2 right, Vector2 up, bool copyAllZs = false)
    {
        Data[0] = right.X;
        Data[1] = right.Y;

        Data[3] = up.X;
        Data[4] = up.Y;

        Data[6] = position.X;
        Data[7] = position.Y;
        Data[8] = 1;

        if (copyAllZs)
        {
            Data[2] = right.Z;
            Data[5] = up.Z;
            Data[8] = position.Z;
        }
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

    public bool IsIdentity(float epsilon = Utils.PreciseEpsilonF)
    {
        for (int x = 0; x < 3; ++x)
        {
            for (int y = 0; y < 3; ++y)
            {
                if (!Utils.IsFloatClose(x == y ? 1 : 0, this[x, y]))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public Matrix2 Transpose()
    {
        float[] data = new float[9];

        for (int i = 0; i < 9; ++i)
        {
            int x = i % 3;
            int y = i / 3;

            data[i] = this[IndexOf(y, x)];
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
        Vector2 translation = Translation;

        float rightX = right.X;

        right = new(up.Y, -right.Y);
        up = new(-up.X, rightX);

        translation = new(
            right.X * translation.X + up.X * translation.Y,
            right.Y * translation.X + up.Y * translation.Y
        );

        return new(-translation, right, up);
    }

    public float GetRotation()
    {
        Vector2 upUnit = Up.Unit;

        return MathF.Atan2(upUnit.X, upUnit.Y);
    }

    public Vector2 Row(int row)
    {
        return new(this[row], this[row + 3], this[row + 6]);
    }

    public Vector2 Column(int column)
    {
        return new(this[3 * column], this[1 + 3 * column], this[2 + 3 * column]);
    }

    public static Matrix2 operator+(Matrix2 left, Matrix2 right)
    {
        float[] data = new float[9];

        for (int i = 0; i < 9; ++i)
        {
            data[i] = left[i] + right[i];
        }

        return new(data);
    }

    public static Matrix2 operator -(Matrix2 left, Matrix2 right)
    {
        float[] data = new float[9];

        for (int i = 0; i < 9; ++i)
        {
            data[i] = left[i] + right[i];
        }

        return new(data);
    }

    public static Matrix2 operator -(Matrix2 matrix)
    {
        float[] data = new float[9];

        for (int i = 0; i < 9; ++i)
        {
            data[i] = -matrix[i];
        }

        return new(data);
    }

    public static Matrix2 operator +(Matrix2 matrix)
    {
        float[] data = new float[9];

        for (int i = 0; i < 9; ++i)
        {
            data[i] = -matrix[i];
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
                data[i] += left[x, j] * right[j, y];
            }
        }

        return new(data);
    }

    public static Vector2 operator *(Matrix2 left, Vector2 right)
    {
        return new(
             left[0, 0] * right.X + left[0, 1] * right.Y + left[0, 2] * right.Z,
             left[1, 0] * right.X + left[1, 1] * right.Y + left[1, 2] * right.Z,
             left[2, 0] * right.X + left[2, 1] * right.Y + left[2, 2] * right.Z
        );
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
            if (!Utils.IsFloatClose(this[i], other[i]))
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
            if (!Utils.IsFloatClose(this[i], other[i], epsilon))
            {
                return false;
            }
        }

        return true;
    }

    public static bool operator ==(Matrix2 left, Matrix2 right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Matrix2 left, Matrix2 right)
    {
        return !left.Equals(right);
    }

    public override int GetHashCode()
    {
        int hash = 17;

        for (int i = 0; i < 9; ++i)
        {
            hash = hash * 23 + this[i].GetHashCode();
        }

        return hash;
    }

    public override string ToString()
    {
        return $"r:[ {this[0]} {this[1]} {this[2]} ] u:[ {this[3]} {this[4]} {this[5]} ] t:[ {this[6]} {this[7]} {this[8]} ]";
    }

    public float this[int x, int y]
    {
        get => this[IndexOf(x, y)];
    }

    public float this[int i]
    {
        get => Data[i];
    }

    public static int IndexOf(int x, int y)
    {
        return x + 3 * y;
    }

    public static Matrix2 NewTranslation(float x, float y)
    {
        return new(new Vector2(x, y));
    }

    public static Matrix2 NewTranslation(Vector2 translation)
    {
        return NewTranslation(translation.X, translation.Y);
    }

    public static Matrix2 NewRotation(float rotation)
    {
        return new(new(), rotation);
    }

    public static Matrix2 NewScale(float x, float y)
    {
        return new(new(), new Vector2(x, y));
    }

    public static Matrix2 NewScale(Vector2 scale)
    {
        return NewScale(scale.X, scale.Y);
    }
}