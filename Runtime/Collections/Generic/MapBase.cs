using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Acciaio.Collections.Generic
{
    [Serializable]
    public abstract class MapBase<TKey, TMiddle, TValue> : IDictionary, IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        private struct Entry
        {
            public TMiddle Key;
            public TValue Value;

            public bool Equals(Entry e)
            {
                return EqualityComparer<TMiddle>.Default.Equals(Key, e.Key) &&
                       EqualityComparer<TValue>.Default.Equals(Value, e.Value);
            }

            public bool Equals(KeyValuePair<TMiddle, TValue> kvp)
            {
                return EqualityComparer<TMiddle>.Default.Equals(Key, kvp.Key) &&
                       EqualityComparer<TValue>.Default.Equals(Value, kvp.Value);
            }

            public override bool Equals(object obj)
            {
                return obj is Entry e && Equals(e) ||
                        obj is KeyValuePair<TMiddle, TValue> kvp && Equals(kvp);
            }

            public override int GetHashCode() => HashCode.Combine(Key);

            public static bool operator ==(Entry e1, Entry e2) => e1.Equals(e2);
            public static bool operator !=(Entry e1, Entry e2) => !(e1 == e2);
        }

        private readonly Dictionary<TKey, TValue> _internal;
        private readonly List<Entry> _serializationMiddleman = new();

        [SerializeField]
        private List<Entry> _serializedEntries;

        private IDictionary InternalIDict => _internal;
        private IDictionary<TKey, TValue> InternalGeneric => _internal;

        public object this[object key]
        {
            get => InternalIDict[key];
            set => InternalIDict[key] = value;
        }

        public TValue this[TKey key]
        {
            get => _internal[key];
            set => _internal[key] = value;
        }

        public bool IsFixedSize => InternalIDict.IsFixedSize;

        public bool IsReadOnly => InternalIDict.IsReadOnly;

        public ICollection<TKey> Keys => InternalGeneric.Keys;

        public ICollection<TValue> Values => InternalGeneric.Values;

        public int Count => _internal.Count;

        public bool IsSynchronized => InternalIDict.IsSynchronized;

        public object SyncRoot => InternalIDict.SyncRoot;

        ICollection IDictionary.Keys => InternalIDict.Keys;

        ICollection IDictionary.Values => InternalIDict.Values;

        private MapBase(Dictionary<TKey, TValue> dictionaryToWrap) => _internal = dictionaryToWrap;

        internal MapBase() : this(new Dictionary<TKey, TValue>()) { }

        internal MapBase(IDictionary<TKey, TValue> dict) : this(new Dictionary<TKey, TValue>(dict)) { }

        internal MapBase(IEqualityComparer<TKey> comparer) : this(new Dictionary<TKey, TValue>(comparer)) { }

        internal MapBase(int capacity) : this(new Dictionary<TKey, TValue>(capacity)) { }

        internal MapBase(IDictionary<TKey, TValue> dict, IEqualityComparer<TKey> comparer) :
            this(new Dictionary<TKey, TValue>(dict, comparer)) { }

        internal MapBase(int capacity, IEqualityComparer<TKey> comparer) :
            this(new Dictionary<TKey, TValue>(capacity, comparer)) { }

        /// <summary>
        /// Accesses the wrapped dictionary directly. Changes to that dictionary are reflected in this Map and vice-versa.
        /// To create an independent copy, see ToDictionary() instead.
        /// </summary>
        public Dictionary<TKey, TValue> AsDictionary() => _internal;

        /// <summary>
        /// Creates a new Dictionary starting from this Map. The new Dictionary is independent and changes
        /// in it are not reflected on this Map. To create a view on this map, see AsDictionary() instead.
        /// </summary>
        public Dictionary<TKey, TValue> ToDictionary() => new(_internal);

        protected abstract TKey MiddleToKey(TMiddle middle);

        protected abstract TMiddle KeyToMiddle(TKey key);

        public void OnAfterDeserialize()
        {
            _internal.Clear();
            _serializationMiddleman.Clear();
            _serializationMiddleman.AddRange(_serializedEntries);
            var elementsNumber = _serializedEntries.Count;
            for(int i = 0, check = 0; i < elementsNumber; i++)
            {
                var entry = _serializationMiddleman[check];
                var key = MiddleToKey(entry.Key);
                if (entry.Key == null || _internal.ContainsKey(key))
                {
                    check++;
                }
                else
                {
                    InternalGeneric.Add(key, entry.Value);
                    _serializationMiddleman.RemoveAt(check);
                }
            }
        }

        public void OnBeforeSerialize()
        {
            Entry FromKeyValuePair(KeyValuePair<TKey, TValue> kvp)
            {
                return new Entry
                {
                    Key = KeyToMiddle(kvp.Key),
                    Value = kvp.Value
                };
            }
            
            _serializedEntries ??= new();
            _serializedEntries.Clear();
            _serializedEntries = _internal.Select(FromKeyValuePair).Concat(_serializationMiddleman).ToList();
        }

        public void Add(object key, object value) => InternalIDict.Add(key, value);

        public void Clear() => _internal.Clear();

        public bool Contains(object key) => InternalIDict.Contains(key);

        public void CopyTo(Array array, int index) => InternalIDict.CopyTo(array, index);

        public void Remove(object key) => InternalIDict.Remove(key);

        public void Add(TKey key, TValue value) => _internal.Add(key, value);

        public void Add(KeyValuePair<TKey, TValue> item) => InternalGeneric.Add(item);

        public bool Contains(KeyValuePair<TKey, TValue> item) => InternalGeneric.Contains(item);

        public bool ContainsKey(TKey key) => _internal.ContainsKey(key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => InternalGeneric.CopyTo(array, arrayIndex);

        public bool Remove(TKey key) => _internal.Remove(key);

        public bool Remove(KeyValuePair<TKey, TValue> item) => InternalGeneric.Remove(item);

        public bool TryGetValue(TKey key, out TValue value) => _internal.TryGetValue(key, out value);

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _internal.GetEnumerator();

        IDictionaryEnumerator IDictionary.GetEnumerator() => InternalIDict.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}