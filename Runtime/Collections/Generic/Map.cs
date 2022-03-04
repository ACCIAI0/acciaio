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
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Serializable]
    public class Map<TKey, TValue> : IDictionary, ISerializationCallbackReceiver, IDictionary<TKey, TValue>
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

            public static implicit operator KeyValuePair<TKey, TValue>(Entry entry) => 
                    new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
            public static implicit operator Entry(KeyValuePair<TKey, TValue> pair) => 
                    new Entry(pair);
        }

        private readonly Dictionary<TKey, TValue> _internal;

        [SerializeField]
        private List<Entry> _serializedEntries = default;

        private IDictionary InternalIDict => _internal as IDictionary;
        private IDictionary<TKey, TValue> InternalGeneric => _internal as IDictionary<TKey, TValue>;

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

        public ICollection Keys => InternalIDict.Keys;

        public ICollection Values => InternalIDict.Values;

        public int Count => _internal.Count;

        public bool IsSynchronized => InternalIDict.IsSynchronized;

        public object SyncRoot => InternalIDict.SyncRoot;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => _internal.Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => _internal.Values;

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

        public Dictionary<TKey, TValue> AsDictionary() => _internal;

        public void OnAfterDeserialize()
        {
            _internal.Clear();
            var idict = _internal as IDictionary<TKey, TValue>;
            int elementsNumber = _serializedEntries.Count;
            for(int i = 0, check = 0; i < elementsNumber; i++) 
            {
                var entry = _serializedEntries[check];
                if (_internal.ContainsKey(entry.Key)) check++;
                else 
                {
                    idict.Add(entry);
                    _serializedEntries.RemoveAt(check);
                }
            }
        }

        public void OnBeforeSerialize()
        {
            if (_serializedEntries == null) _serializedEntries = new List<Entry>();
            _serializedEntries = _internal.Select(kvp => new Entry(kvp))
                    .Concat(_serializedEntries)
                    .ToList();
            _internal.Clear();
        }

        public void Add(object key, object value) => InternalIDict.Add(key, value);

        public void Clear() => _internal.Clear();

        public bool Contains(object key) => InternalIDict.Contains(key);

        public void CopyTo(Array array, int index) => InternalIDict.CopyTo(array, index);

        public void Remove(object key) => InternalIDict.Remove(key);

        public IDictionaryEnumerator GetEnumerator() => InternalIDict.GetEnumerator();

        public void Add(TKey key, TValue value) => _internal.Add(key, value);

        public void Add(KeyValuePair<TKey, TValue> item) => InternalGeneric.Add(item);

        public bool Contains(KeyValuePair<TKey, TValue> item) => InternalGeneric.Contains(item);

        public bool ContainsKey(TKey key) => _internal.ContainsKey(key);

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => InternalGeneric.CopyTo(array, arrayIndex);

        public bool Remove(TKey key) => _internal.Remove(key);

        public bool Remove(KeyValuePair<TKey, TValue> item) => InternalGeneric.Remove(item);

        public bool TryGetValue(TKey key, out TValue value) => _internal.TryGetValue(key, out value);


        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() => _internal.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
