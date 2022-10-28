using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Atoms.Persistence
{
    public  class Vector3Converter : JsonConverter<Vector3>
    {
        public override bool CanRead => false;

        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
            => serializer.Serialize(writer, new {value.x, value.y, value.z});

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue,
            bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}