using System;
using System.Diagnostics;
using UnityEngine;

namespace Acciaio.Collections.Generic
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class MapNamesAttribute : PropertyAttribute
    {
        public const string DefaultKey = "Key";
        public const string DefaultValue = "Value";

        public string KeyName { get; }
        public string ValueName { get; }

        public MapNamesAttribute(string key, string value = null)
        {
            KeyName = key ?? DefaultKey;
            ValueName = value ?? DefaultValue;
        }
    }
}
