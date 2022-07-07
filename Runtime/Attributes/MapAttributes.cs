using UnityEngine;

namespace Acciaio
{
    public class MapNames : PropertyAttribute
    {
        public string KeyName { get; }
        public string ValueName { get; }

        public MapNames(string key, string value)
        {
            KeyName = key ?? "Key";
            ValueName = value ?? "Value";
        }

        public MapNames(string key) : this(key, null) {}
    }
}
