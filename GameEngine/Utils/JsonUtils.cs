using GameEngine.ECS;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GameEngine.Utils;

public static class Utf8JsonWriterExtensions
{
    public static void WriteObject(this Utf8JsonWriter writer, string propertyName, System.Object value, JsonSerializerOptions options)
    {
        writer.WritePropertyName(propertyName);
        writer.WriteRawValue(JsonSerializer.Serialize(value));
    }
    
    public static void Write<Type>(this Utf8JsonWriter writer, string propertyName, Type value, JsonSerializerOptions options, bool allowNull)
    {
        if (value == null)
        {
            if (!allowNull) return;

            writer.WriteNull(propertyName);

            return;
        }

        if (typeof(Type) == typeof(System.Object))
        {
            writer.WriteObject(propertyName, value, options);

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

    public static void Write<Type>(this Utf8JsonWriter writer, Type value, JsonSerializerOptions options)
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

    public static void WriteValue<Type>(this Utf8JsonWriter writer, MemberInfo memberInfo, Type value, JsonSerializerOptions options) where Type : notnull
    {
        switch (memberInfo.MemberType)
        {
            case MemberTypes.Field:
                Write(writer, memberInfo.Name, ((FieldInfo)memberInfo).GetValue(value), options, !memberInfo.IsNonNullableReferenceType());
                break;
            case MemberTypes.Property:
                Write(writer, memberInfo.Name, ((PropertyInfo)memberInfo).GetValue(value), options, !memberInfo.IsNonNullableReferenceType());
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public static void WriteMessage<Type>(this Utf8JsonWriter writer, Type value, JsonSerializerOptions options) where Type : notnull
    {
        foreach (FieldInfo member in value.GetType().GetFields())
        {
            writer.WriteValue(member, value, options);
        }

        foreach (PropertyInfo member in value.GetType().GetProperties())
        {
            writer.WriteValue(member, value, options);
        }
    }
}
