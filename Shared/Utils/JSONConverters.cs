using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Numerics;

namespace dfe.Shared.Utils
{
    class JSONConverters
    {
    }

    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Vector2);
        }

        public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();

        }

        public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
