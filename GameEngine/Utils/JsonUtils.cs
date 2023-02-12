using GameEngine.ECS;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GameEngine.Utils;

public class JsonUtils
{
    public static void Write<Type>(Utf8JsonWriter writer, string propertyName, Type value, JsonSerializerOptions options, bool allowNull)
    {
        if (value == null)
        {
            if (!allowNull) return;

            writer.WriteNull(propertyName);

            return;
        }

        foreach (Attribute attribute in Attribute.GetCustomAttributes(typeof(Type)))
        {
            if (attribute is JsonConverterAttribute jsonConverter && jsonConverter.ConverterType != null)
            {
                JsonConverter<Type>? converter = (JsonConverter<Type>?)Activator.CreateInstance(jsonConverter.ConverterType);

                writer.WritePropertyName(propertyName);
                converter?.Write(writer, value, options);
            }
        }
    }
    public static void Write<Type>(Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
    {
        if (value == null) return;

        foreach (Attribute attribute in Attribute.GetCustomAttributes(typeof(Type)))
        {
            if (attribute is JsonConverterAttribute jsonConverter && jsonConverter.ConverterType != null)
            {
                JsonConverter<Type>? converter = (JsonConverter<Type>?)Activator.CreateInstance(jsonConverter.ConverterType);

                converter?.Write(writer, value, options);
            }
        }
    }
}
