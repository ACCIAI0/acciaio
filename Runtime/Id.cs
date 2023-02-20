﻿using System;
using UnityEngine;

namespace Acciaio
{
    [Serializable]
    public class Id : IEquatable<Id>
    {
        [SerializeField]
        private string _value;

        protected Id() : this(string.Empty) { } // Used by Unity

        public Id(string value) => _value = value;

        public bool Equals(string other) 
            => ReferenceEquals(_value, other) || _value.Equals(other, StringComparison.Ordinal);

        public virtual bool Equals(Id other)
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
        public AutoId() : base(Guid.NewGuid().ToString()) { }
        
        public AutoId(Id id) : base(id) { }
    }

    [Serializable]
    public struct ReferenceId<T> where T : IIdentifiable
    {
#if UNITY_EDITOR
        [SerializeField]
        private string _assetGuid;
#endif

        [field: SerializeField]
        public Id ReferencedId { get; private set; }

        public ReferenceId(Id value)
        {
            _assetGuid = null;
            ReferencedId = value;
        }

        public bool Is(T value) => value?.Id?.Equals(ReferencedId) ?? false;

        public bool Equals(ReferenceId<T> refId) => ReferencedId == refId.ReferencedId;

        public override bool Equals(object other) => other is ReferenceId<T> refId && Equals(refId);
        
        // It's effectively readonly at runtime
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => ReferencedId.GetHashCode();
    }
}