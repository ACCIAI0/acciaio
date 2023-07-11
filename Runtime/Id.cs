using System;
using UnityEngine;

namespace Acciaio
{
    [Serializable]
    public class Id : IEquatable<Id>
    {
        public static readonly Id Empty = new();

        public static Id NewId(string value) => new(value);
        
        [SerializeField]
        private string _value;

        private Id() : this(string.Empty) { } // Used by Unity

        protected internal Id(string value) => _value = value;

        public bool Equals(string other) 
            => ReferenceEquals(_value, other) || _value.Equals(other, StringComparison.Ordinal);

        public bool Equals(Id other)
        {
            return other is not null &&
                    (ReferenceEquals(this, other) || _value.Equals(other._value, StringComparison.Ordinal));
        }

        public override bool Equals(object other) => other is Id id && Equals(id);
        
        // It's effectively readonly at runtime
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => _value.GetHashCode();

        public override string ToString() => _value;

        public static bool operator ==(Id id1, Id id2)
        {
            if (id1 is null) return id2 is null;
            return id1.Equals(id2);
        }

        public static bool operator !=(Id id1, Id id2) => !(id1 == id2);

        public static implicit operator string(Id id) => id?._value;

        public static implicit operator Id(string str) => str is null ? null : new(str);
    }
    
    [Serializable]
    public sealed class AutoId : Id
    {
        public static AutoId NewId() => new();
        
        private AutoId() : base(Guid.NewGuid().ToString()) { }
        
        private AutoId(string id) : base(id) { }
        
        public static implicit operator AutoId(string str) => str is null ? null : new(str);
    }

    public abstract class IdReference
    {
        [field: SerializeField]
        public Id ReferencedId { get; private set; }

        protected IdReference(Id value) => ReferencedId = value;
    }

    [Serializable]
    public class IdReference<T> : IdReference where T : IIdentifiable
    {
#if UNITY_EDITOR
        [SerializeField]
        private string _assetGuid;
#endif
        private IdReference() : base(null) { }

        public IdReference(Id value) : base(value)
        {
#if UNITY_EDITOR
            _assetGuid = null;
#endif
        }

        public bool References(T value) => value?.Id?.Equals(ReferencedId) ?? false;

        public bool Equals(IdReference<T> @ref) => @ref is not null && ReferencedId == @ref.ReferencedId;

        public override bool Equals(object other) => other is IdReference<T> refId && Equals(refId);
        
        // It's effectively readonly at runtime
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => ReferencedId.GetHashCode();
    }
}