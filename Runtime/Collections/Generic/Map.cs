using System;
using System.Collections.Generic;

namespace Acciaio.Collections.Generic
{
    /// <summary>
    /// Serializable, generic Dictionary implementation. No more boilerplate code
    /// for when you need to serialize a list of key-value pairs. It's called Map to
    /// better highlight it's serializable, but it's implemented under the hood as a
    /// Dictionary that is accessible by calling AsDictionary().
    /// </summary>
    /// <typeparam name="TKey">Type of key items</typeparam>
    /// <typeparam name="TValue">Type of value items</typeparam>
    [Serializable]
    public sealed class Map<TKey, TValue> : MapBase<TKey, TKey, TValue>
    {
        public static Map<TKey, TValue> WrapDictionary(Dictionary<TKey, TValue> dictionary) => new(dictionary);
        
        public Map() { }

        internal Map(IDictionary<TKey, TValue> dict) : base(new Dictionary<TKey, TValue>(dict)) { }

        internal Map(IEqualityComparer<TKey> comparer) : base(new Dictionary<TKey, TValue>(comparer)) { }

        internal Map(int capacity) : base(new Dictionary<TKey, TValue>(capacity)) { }

        internal Map(IDictionary<TKey, TValue> dict, IEqualityComparer<TKey> comparer) :
            base(new Dictionary<TKey, TValue>(dict, comparer)) { }

        internal Map(int capacity, IEqualityComparer<TKey> comparer) :
            base(new Dictionary<TKey, TValue>(capacity, comparer)) { }

        protected override TKey MiddleToKey(TKey middle) => middle;

        protected override TKey KeyToMiddle(TKey key) => key;
    }
}
