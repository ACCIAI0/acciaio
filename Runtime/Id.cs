using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using UnityEngine;

namespace Acciaio
{
    [Serializable]
    public class Id : IEquatable<Id>
    {
        public static readonly Id Empty = new();

        public static Id OfValue(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));
            return new(value);
        }

        public static Id FromBytes(byte[] bytes) => OfValue(Encoding.Unicode.GetString(bytes));

        public static Id FromBytes(ReadOnlySpan<byte> span) => OfValue(Encoding.Unicode.GetString(span));

        [SerializeField]
        private string _value;

        private Id() : this(string.Empty) { } // Used by Unity

        protected internal Id(string value) => _value = value;

        public byte[] GetBytes() => Encoding.Unicode.GetBytes(_value);

        public int GetBytes(byte[] bytes, int startIndex) 
            => Encoding.Unicode.GetBytes(_value, 0, _value.Length, bytes, startIndex);

        public int GetBytes(Span<byte> span) => Encoding.Unicode.GetBytes(_value, span);

        public bool Equals(string other) 
            => ReferenceEquals(_value, other) || _value.Equals(other, StringComparison.Ordinal);

        public bool Equals(Id other)
        {
            return other is not null &&
                    (ReferenceEquals(this, other) || _value.Equals(other._value, StringComparison.Ordinal));
        }

        public override bool Equals(object other) => other is Id id && Equals(id);
        
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
        public static AutoId Generate() => new();
        
        private AutoId() : base(Guid.NewGuid().ToString()) { }
        
        private AutoId(string id) : base(id) { }
        
        public static implicit operator AutoId(string str) => str is null ? null : new(str);
    }

    public abstract class IdReference
    {
        [field: SerializeField]
        public Id ReferencedId { get; private set; }

        public bool IsValid => ReferencedId != null && !string.IsNullOrEmpty(ReferencedId.ToString());

        protected IdReference(Id value) => ReferencedId = value;
        
        public bool References(Id value) => ReferencedId == value;
        
        public static implicit operator Id(IdReference @ref) => @ref.ReferencedId;
    }

    [Serializable]
    public class IdReference<T> : IdReference where T : IIdentifiable
    {
#if UNITY_EDITOR
#pragma warning disable CS0414
        [SerializeField]
        private string _assetGuid;
#pragma warning restore CS0414
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
        
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        public override int GetHashCode() => ReferencedId.GetHashCode();

        public static implicit operator Id(IdReference<T> @ref) => @ref.ReferencedId;
    }
}