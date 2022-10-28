using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Atoms.Persistence
{
    public class Vector4Converter : ReadbackTypeConverter<Vector4>
    {
        public Vector4Converter() : base(value => new {value.x, value.y, value.z, value.w})
        {
        }
    }
}