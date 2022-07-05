using System;
using System.Collections.Generic;
using UnityEngine;

namespace Acciaio
{
    public interface IMapKey
    {
        public string KeyName { get; }
    }

    public interface IMapValue
    {
        public string ValueName { get; }
    }

    public class MapKey : PropertyAttribute, IMapKey
    {
        public string KeyName { get; }

        public MapKey(string value) => KeyName = value ?? "Key";
    }

    public class MapValue : PropertyAttribute, IMapValue
    {
        public string ValueName { get; }

        public MapValue(string value) => ValueName = value ?? "Value";
    }

    public class MapKeyValue : PropertyAttribute, IMapKey, IMapValue
    {
        public string KeyName { get; }
        public string ValueName { get; }

        public MapKeyValue(string key, string value)
        {
            KeyName = key ?? "Key";
            ValueName = value ?? "Value";
        }

        public MapKeyValue(MapKey key, MapValue value) : this(key?.KeyName, value?.ValueName) { }
    }
}
