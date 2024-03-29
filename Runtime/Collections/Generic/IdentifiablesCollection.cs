﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Acciaio.Collections.Generic
{
    [Serializable]
    public sealed class IdentifiablesCollection<T> : IReadOnlyList<T>, IList<T> where T : IIdentifiable
    {
        [SerializeField]
        private List<T> _list;

        private Dictionary<Id, T> _dictionary;

        private Dictionary<Id, T> Dictionary 
        {
            get
            {
                _dictionary ??= _list.ToDictionary(i => i.Id, i => i);
                
                if (_dictionary.Count != _list.Count)
                {
                    _dictionary.Clear();
                    foreach (var i in _list) _dictionary.Add(i.Id, i);
                }

                return _dictionary;
            }
        }

        public T this[int index]
        {
            get => _list[index];
            set => _list[index] = value;
        }

        public T this[Id key]
        {
            get => Dictionary[key];
            set => Dictionary[key] = value;
        }

        IEnumerable<Id> Ids => Dictionary.Keys;

        public int Count => _list.Count;
        
        public bool IsReadOnly => false;
        
        public IdentifiablesCollection()
        {
            _list = new();
            _dictionary = new();
        }

        public IdentifiablesCollection(int capacity)
        {
            _list = new(capacity);
            _dictionary = new(capacity);
        }

        public IdentifiablesCollection(IEnumerable<T> elements)
        {
            _list = elements.ToList();
            _dictionary = _list.ToDictionary(i => i.Id, i => i);
        }

        public bool TryGetValue(Id id, out T value) => Dictionary.TryGetValue(id, out value);

        public void Add(T item)
        {
            if (item == null) throw new NullReferenceException("Cannot add null item to collection");
            
            Dictionary.Add(item.Id, item);
            _list.Add(item);
        }

        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                if (item == null) continue;
                Add(item);
            }
        }

        public void Clear()
        {
            _list.Clear();
            Dictionary.Clear();
        }

        public bool Contains(KeyValuePair<Id, T> item) => Dictionary.Contains(item);

        public bool Contains(T item) => item != null && Dictionary.ContainsKey(item.Id);

        public bool Contains(Id id) => Dictionary.ContainsKey(id);

        public void CopyTo(KeyValuePair<Id, T>[] array, int arrayIndex) 
            => ((IDictionary<Id, T>)Dictionary).CopyTo(array, arrayIndex);

        public void CopyTo(T[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);

        public bool Remove(T item)
        {
            if (item == null) return false;
            _list.Remove(item);
            return Dictionary.Remove(item.Id);
        }

        public bool Remove(KeyValuePair<Id, T> item) 
            => ((IDictionary<Id, T>)Dictionary).Remove(item) && _list.Remove(item.Value);

        public bool Remove(Id id) => TryGetValue(id, out var value) && Remove(value);

        public int IndexOf(T item) => _list.IndexOf(item);

        public void Insert(int index, T item)
        {
            if (item == null) throw new NullReferenceException("Cannot add null item to collection");
            
            _list.Insert(index, item);
            Dictionary.Add(item.Id, item);
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count) 
                throw new ArgumentOutOfRangeException(nameof(index), "The index can't be less than 0 or equal to or greater than Count");
            var element = _list[index];
            _list.RemoveAt(index);
            Dictionary.Remove(element.Id);
        }

        public List<T>.Enumerator GetEnumerator() => _list.GetEnumerator();

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => _list.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}