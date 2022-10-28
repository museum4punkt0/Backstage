using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Atoms.Persistence
{
    public class ColorConverter : JsonConverter<Color>
    {
        public override bool CanRead => false;

        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
            => serializer.Serialize(writer, new {value.r, value.g, value.b, value.a});

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class Color32Converter : JsonConverter<Color32>
    {
        public override bool CanRead => false;

        public override void WriteJson(JsonWriter writer, Color32 value, JsonSerializer serializer)
            => serializer.Serialize(writer, new {value.r, value.g, value.b, value.a});

        public override Color32 ReadJson(JsonReader reader, Type objectType, Color32 existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}