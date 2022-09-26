using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Acciaio.Collections.Generic
{
    /// <summary>
    /// Serializable, generic Dictionary implementation. No more boilerplate code
    /// for when you need to serialize a list of key-value pairs. It's called Map to
    /// better highlight it's serializable, but it's implemented as a Dictionary which
    /// is retrievable by calling AsDictionary().
    /// </summary>
    /// <typeparam name="TKey">Type of key items</typeparam>
    /// <typeparam name="TValue">Type of value items</typeparam>
    [Serializable]
    public sealed class Map<TKey, TValue> : IDictionary, IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [Serializable]
        private struct Entry
        {
            public TKey Key;
            public TValue Value;

            public Entry(KeyValuePair<TKey, TValue> kvp)
            {
                Key = kvp.Key;
                Value = kvp.Value;
            }

            public bool Equals(Entry e) => 
                Key.Equals(e.Key) && Value.Equals(e.Value);

            public bool Equals(KeyValuePair<TKey, TValue> kvp) => 
                Key.Equals(kvp.Key) && Value.Equals(kvp.Value);

            public override bool Equals(object obj)
            {
                return obj is Entry e && Equals(e) ||
                        obj is KeyValuePair<TKey, TValue> kvp && Equals(kvp);
            }

            public override int GetHashCode() => Key.GetHashCode();

            public static implicit operator KeyValuePair<TKey, TValue>(Entry entry) => new(entry.Key, entry.Value);
            public static implicit operator Entry(KeyValuePair<TKey, TValue> pair) => new(pair);

            public static bool operator ==(Entry e1, Entry e2) => e1.Equals(e2);
            public static bool operator !=(Entry e1, Entry e2) => !(e1 == e2);
            public static bool operator ==(Entry e, KeyValuePair<TKey, TValue> kvp) => e.Equals(kvp);
            public static bool operator !=(Entry e, KeyValuePair<TKey, TValue> kvp) => !(e == kvp);
            public static bool operator ==(KeyValuePair<TKey, TValue> kvp, Entry e) => e.Equals(kvp);
            public static bool operator !=(KeyValuePair<TKey, TValue> kvp, Entry e) => !(e == kvp);
        }

        public static Map<TKey, TValue> WrapDictionary(Dictionary<TKey, TValue> dictionary) => new(dictionary);

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

        private Map(Dictionary<TKey, TValue> dictionaryToWrap) => 
            _internal = dictionaryToWrap;

        public Map() =>
            _internal = new Dictionary<TKey, TValue>();

        public Map(IDictionary<TKey, TValue> dict) =>
            _internal = new Dictionary<TKey, TValue>(dict);

        public Map(IEqualityComparer<TKey> comparer) =>
            _internal = new Dictionary<TKey, TValue>(comparer);

        public Map(int capacity) =>
            _internal = new Dictionary<TKey, TValue>(capacity);

        public Map(IDictionary<TKey, TValue> dict, IEqualityComparer<TKey> comparer) =>
            _internal = new Dictionary<TKey, TValue>(dict, comparer);

        public Map(int capacity, IEqualityComparer<TKey> comparer) =>
            _internal = new Dictionary<TKey, TValue>(capacity, comparer);

        /// <summary>
        /// Accesses the wrapped dictionary directly. Changes to that dictionary are reflected in this Map and vice-versa.
        /// To create an independent copy, see ToDictionary() instead.
        /// </summary>
        public Dictionary<TKey, TValue> AsDictionary() => _internal;

        /// <summary>
        /// Creates a new Dictionary starting from this Map. The new Dictionary is independent and changes
        /// in it are not reflected on this Map. To create a view on this map, see AsDictionary() instead.
        /// </summary>
        /// <returns></returns>
        public Dictionary<TKey, TValue> ToDictionary() => new(_internal);

        public void OnAfterDeserialize()
        {
            _internal.Clear();
            _serializationMiddleman.Clear();
            _serializationMiddleman.AddRange(_serializedEntries);
            var elementsNumber = _serializedEntries.Count;
            for(int i = 0, check = 0; i < elementsNumber; i++)
            {
                var entry = _serializationMiddleman[check];
                if (entry.Key == null || _internal.ContainsKey(entry.Key))
                {
                    check++;
                }
                else
                {
                    InternalGeneric.Add(entry);
                    _serializationMiddleman.RemoveAt(check);
                }
            }
        }

        public void OnBeforeSerialize()
        {
            _serializedEntries ??= new();
            _serializedEntries.Clear();
            _serializedEntries = _internal.Select(kvp => new Entry(kvp))
                    .Concat(_serializationMiddleman)
                    .ToList();
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
