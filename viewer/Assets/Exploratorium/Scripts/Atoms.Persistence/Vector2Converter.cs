using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Atoms.Persistence
{
    public class Vector2Converter : JsonConverter<Vector2>
    {
        public override bool CanRead => false;

        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
            => serializer.Serialize(writer, new {value.x, value.y});

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}