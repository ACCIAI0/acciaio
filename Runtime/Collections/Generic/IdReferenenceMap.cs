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
    /// <typeparam name="TReference">Type of the identifiable to reference</typeparam>
    /// <typeparam name="TValue">Type of value items</typeparam>
    [Serializable]
    public class IdReferencesMap<TReference, TValue> : MapBase<Id, IdReference<TReference>, TValue> where TReference : IIdentifiable
    {
        public static Map<Id, TValue> WrapDictionary(Dictionary<Id, TValue> dictionary) => new(dictionary);

        public TValue this[IdReference<TReference> @ref] => this[@ref.ReferencedId];

        protected override Id MiddleToKey(IdReference<TReference> middle) => middle.ReferencedId;

        protected override IdReference<TReference> KeyToMiddle(Id key) => new(key);

        public bool TryGetValue(IdReference<TReference> reference, out TValue value)
            => TryGetValue(reference.ReferencedId, out value);
    }
}