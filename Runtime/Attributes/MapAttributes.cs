using UnityEngine;

namespace Acciaio
{
    public class MapNames : PropertyAttribute
    {
        public static readonly string DEFAULT_KEY = "Key";
        public static readonly string DEFAULT_VALUE = "Value";

        public string KeyName { get; }
        public string ValueName { get; }

        public MapNames(string key, string value)
        {
            KeyName = key ?? DEFAULT_KEY;
            ValueName = value ?? DEFAULT_VALUE;
        }

        public MapNames(string key) : this(key, null) {}
    }
}
