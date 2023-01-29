using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameEngine.Math2D;

namespace GameEngine.ECS;

public class Transform : Component
{
    public Matrix2 Matrix;
    public float Rotation
    {
        get => Matrix.Rotation;
        set => Matrix = new(Matrix.Translation, Matrix.Scale2, value);
    }
    public Vector2 Position
    {
        get => Matrix.Translation;
        set => Matrix = new(value, Matrix.Right, Matrix.Up);
    }
    public Vector2 Scale
    {
        get => Matrix.Scale2;
        set => Matrix = new(Matrix.Translation, value.X * Matrix.Right.Unit, value.Y * Matrix.Up.Unit);
    }

    public Transform()
    {
        Name = "Transform";
    }
}