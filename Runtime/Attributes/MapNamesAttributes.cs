using System;
using System.Diagnostics;
using UnityEngine;

namespace Acciaio.Collections.Generic
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class MapNamesAttribute : PropertyAttribute
    {
        public static readonly string DEFAULT_KEY = "Key";
        public static readonly string DEFAULT_VALUE = "Value";

        public string KeyName { get; }
        public string ValueName { get; }

        public MapNamesAttribute(string key, string value)
        {
            KeyName = key ?? DEFAULT_KEY;
            ValueName = value ?? DEFAULT_VALUE;
        }

        public MapNamesAttribute(string key) : this(key, null) {}
    }
}
