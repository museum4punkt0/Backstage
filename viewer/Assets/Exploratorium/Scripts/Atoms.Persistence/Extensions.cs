using System;
using System.Text.RegularExpressions;
using UnityAtoms;
using UnityEngine;

namespace Atoms.Persistence
{

    public static class AtomExtensions
    {
        public static bool IsPrimitive(this AtomBaseVariable variable) => variable.BaseValue.GetType().IsPrimitive;
        public static bool IsValueType(this AtomBaseVariable variable) => variable.BaseValue.GetType().IsValueType;
        public static Type GetType(this AtomBaseVariable variable) => variable.BaseValue.GetType();
    }
    
    public static class StringExtensions
    {
        public static string TrimNonWord(this string value)
        {
            var regExp = new Regex(@"[^\w]+");
            return regExp.Replace(value, "");
        }
        
        public static string TrimNonAscii(this string value)
        {
            var regExp = new Regex(@"[^ -~]+");
            return regExp.Replace(value, "");
        }
    }
    
    public static class PrimitiveExtensions
    {
        public static object GetPrimitive(this Vector2 obj) => new {obj.x, obj.y};
        public static object GetPrimitive(this Vector3 obj) => new {obj.x, obj.y, obj.z};
        public static object GetPrimitive(this Vector4 obj) => new {obj.x, obj.y, obj.z, obj.w};
        public static object GetPrimitive(this Quaternion obj) => new {obj.x, obj.y, obj.z, obj.w};
        public static object GetPrimitive(this Color obj) => new {obj.r, obj.g, obj.b, obj.a};
        public static object GetPrimitive(this Color32 obj) => new {obj.r, obj.g, obj.b, obj.a};
        public static object GetPrimitive(this Rect obj) => new {obj.center, obj.size};
        public static object GetPrimitive(this Plane obj) => new {obj.distance, obj.normal};
        public static object GetPrimitive(this Vector2Int obj) => new {obj.x, obj.y};
        public static object GetPrimitive(this Vector3Int obj) => new {obj.x, obj.y, obj.z};
        public static object GetPrimitive(this Bounds obj) => new {obj.center, obj.size};
        public static object GetPrimitive(this AnimationCurve obj) => obj.keys;

        public static object GetPrimitive(this Matrix4x4 obj) => new
        {
            obj.m00, obj.m01, obj.m02, obj.m03,
            obj.m10, obj.m11, obj.m12, obj.m13,
            obj.m20, obj.m21, obj.m22, obj.m23,
            obj.m30, obj.m31, obj.m32, obj.m33,
        };
    }
}