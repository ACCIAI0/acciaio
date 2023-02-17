using System;
using UnityEngine;

namespace Acciaio
{
    [Serializable]
    public class Id : IEquatable<Id>
    {
        [SerializeField]
        private string _value;

        private Id() : this(string.Empty) { } // Used by Unity

        public Id(string value) => _value = value;

        public bool Equals(string other) 
            => ReferenceEquals(_value, other) || _value.Equals(other, StringComparison.Ordinal);

        public bool Equals(Id other)
        {
            return other is not null && 
                    (ReferenceEquals(this, other) || _value.Equals(other._value, StringComparison.Ordinal));
        }

        public override bool Equals(object other) 
            => other is Id id && _value.Equals(id._value) || other is string str && Equals(str);
        
        // It's effectively readonly at runtime
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => _value.GetHashCode();

        public static bool operator ==(Id id1, Id id2)
        {
            if (id1 is null) return id2 is null;
            return id1.Equals(id2);
        }

        public static bool operator !=(Id id1, Id id2) => !(id1 == id2);

        public static implicit operator string(Id id) => id?._value;

        public static implicit operator Id(string str) => str is null ? null : new(str);
    }
}