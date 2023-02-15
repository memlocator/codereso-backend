using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using GameEngine.Math2D;
using GameEngine.Utils;

namespace GameEngine.ECS;

public class TransformJsonConverter : JsonConverter<Transform>
{
    public override Transform Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new Transform();
    }

    public override void Write(Utf8JsonWriter writer, Transform value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        value.WriteHeader(writer, options);
        writer.Write("position", value.Position, options, true);
        writer.Write("scale", value.Scale, options, true);
        writer.WritePropertyName("rotation");
        writer.WriteNumberValue(value.Rotation * 180 / float.Pi);
        writer.WriteEndObject();
    }
}

[JsonConverter(typeof(TransformJsonConverter))]
public class Transform : Component
{
    public override bool ShouldBeReplicated { get => true; }

    private Matrix2 Data = new();
    public Matrix2 Matrix
    {
        get => Data;
        set
        {
            Data = value;
            IsStale = true;
        }
    }
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
    public int MovementControllers { get; private set; } = 0;
    public bool IsStatic { get => MovementControllers == 0; }
    public bool IsStale { get; private set; } = false;

    public Transform()
    {
        Name = "Transform";
    }

    public void AddMovementController()
    {
        ++MovementControllers;
    }

    public void RemoveMovementController()
    {
        --MovementControllers;
    }

    public void MarkStale()
    {
        IsStale = true;
    }

    public void Updated()
    {
        IsStale = false;
    }

    public override void Update(float delta)
    {
        Updated();
    }
}