using System;
using System.Numerics;
using Newtonsoft.Json;

namespace Atoms.Persistence
{
    /// <summary>
    /// Serializes an anonymous, simplified representation of a complex type that can be deserialized into the same type.
    /// </summary>
    public class ReadbackTypeConverter<T> : JsonConverter<T>
    {
        public override bool CanRead => false;

        private readonly Func<T, object> _converter;
        
        /// <summary>
        /// Construct a new instance of the converter
        /// </summary>
        /// <param name="converter">Function that converts <see cref="T"/> into a minimal object that, if deserialized
        /// and applied to an object of type <see cref="T"/> will recreate its state as it was when serialized.</param>
        public ReadbackTypeConverter(Func<T, object> converter) => _converter = converter;

        public override void WriteJson(JsonWriter writer, T value, JsonSerializer serializer)
            => serializer.Serialize(writer, _converter.Invoke(value));

        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, JsonSerializer serializer) 
            => throw new NotImplementedException();
    }
}