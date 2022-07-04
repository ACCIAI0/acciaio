using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Acciaio
{
    public static class Enumerable
    {
        private readonly static Random s_rng = new Random(DateTime.Now.Millisecond);
        private readonly static RNGCryptoServiceProvider s_crng = new RNGCryptoServiceProvider();

        private static IEnumerable<T> InternalRandomSample<T>(IEnumerable<T> e, int n, bool withRepetitions)
        {
            if(n < 0)
                throw new ArgumentException("Negative sample count in RandomSample");
            if(n == 0 || e.IsEmpty() || (!withRepetitions && n > e.Count())) 
                yield break;
            var list = e.ToList();
            int start = 0;
            for(int i = 0; i < n; i++)
            {
                int index = s_rng.Next(start, list.Count);
                T element = list[index];
                if(!withRepetitions) list.Swap(start++, index);
                yield return element;

            }
        }

        private static IEnumerable<T> InternalShuffle<T>(IEnumerable<T> enumerable, Func<int, int> randomSupplier)
        {
            var shuffled = new List<T>();
            foreach (T v in enumerable)
            {
                int j = randomSupplier(shuffled.Count + 1);
                if (j == shuffled.Count)
                    shuffled.Add(v);
                else 
                {
                    shuffled.Add(shuffled[j]);
                    shuffled[j] = v;
                }
            }
            return shuffled;
        }

#region Int Range
        ///<summary>
        /// Builds an enumerable of int values ranging from 0 (inclusive) to 'to' (exclusive), using increments of length 'step' (default 1).
        ///</summary>
        ///<param name="to">the exclusive end of the range</param>
        public static IEnumerable<int> Range(int to, ushort step = 1) => Range(0, to, step);

        /// <summary>
        /// Builds an enumerable of int values ranging from 'from' (inclusive) to 'to' (exclusive), using increments of length 'step' (default 1).
        /// If from is higher than to, the range will be built with negative steps.
        /// </summary>
        /// <param name="to">the exclusive end of the range</param>
        /// <param name="from">the inclusive beginning of the range</param>
        /// <param name="step">the non-negative step increment to use</param>
        public static IEnumerable<int> Range(int from, int to, ushort step = 1)
        {
            bool isForward = from < to;
            int internalStep = (2 * Convert.ToInt32(isForward) - 1) * step;
            for(int i = from; (isForward && i < to) || (!isForward && i > to); i += internalStep)
                yield return i;
        }
#endregion

#region Long Range
        /// <summary>
        /// Builds an enumerable of long values ranging from 0 (inclusive) to 'to' (exclusive), using unary increments.
        /// </summary>
        /// <param name="to">the exclusive end of the range</param>
        public static IEnumerable<long> Range(long to, uint step = 1) => Range(0, to, step);

        /// <summary>
        /// Builds an enumerable of long values ranging from 'from' (inclusive) to 'to' (exclusive), using increments of length 'step'.
        /// If from is higher than to, the range will be built with negative steps.
        /// </summary>
        /// <param name="to">the exclusive end of the range</param>
        /// <param name="from">the inclusive beginning of the range</param>
        /// <param name="step">the non-negative step increment to use</param>
        public static IEnumerable<long> Range(long from, long to, uint step = 1)
        {
            bool isForward = from < to;
            long internalStep = (2 * Convert.ToInt32(isForward) - 1) * step;
            for(long i = from; (isForward && i < to) || (!isForward && i > to); i += internalStep)
                yield return i;
        }
#endregion

        /// <summery>
        /// Returns one random element from the given enumerable or default(T) if it is empty.
        /// </summary>
        public static T RandomSample<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable.Count() == 0) return default(T);
            return enumerable.ElementAt(s_rng.Next(0, enumerable.Count()));
        }

        /// <summery>
        /// Returns an IEnumerable of n random elements from the given enumerable. If the withRepetitions flag is false it guarantees that 
        /// the returned IEnumerable won't contain duplicates if and only if the original enumerable doesn't contain duplicates.
        /// </summary>
        public static IEnumerable<T> RandomSample<T>(this IEnumerable<T> enumerable, int n, bool withRepetitions = false)
        {
            return InternalRandomSample(enumerable, n, withRepetitions);
        }

        /// <summery>
        /// Returns an IEnumerable of n random elements from the given enumerable. It will contain maxOccurrences occurrences or less
        /// of any single element if and only if the given element occurs only once in the original enumerable.
        /// </summary>
        public static IEnumerable<T> RandomSample<T>(this IEnumerable<T> enumerable, int n, int maxOccurences)
        {
            if (maxOccurences <= 0) 
                return InternalRandomSample(enumerable, n, true);
            var concat = enumerable;
            for (int i = 1; i < maxOccurences; i++)
                concat = concat.Concat(enumerable);
            return InternalRandomSample(concat, n, false);
        }

        /// <summary>
        /// Randomly reaorders the enumerable using pseudo-random generation. It's faster than FineShuffled. The resulting IEnumerable 
        /// is eager evaluated.
        /// </summary>
        public static IEnumerable<T> Shuffled<T>(this IEnumerable<T> enumerable) => InternalShuffle(enumerable, s_rng.Next);

        /// <summary>
        /// Returns true if the given enumerable doesn't contain any element, false otherwise.
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.GetEnumerator().MoveNext();

        /// <summary>
        /// Randomly reorders the enumerable using a crypto-random generator. It's slower than Shuffled, but 
        /// the resulting order of the elements is guaranteed to be true random. The resulting IEnumerable is eager
        /// evaluated.
        /// </summary>
        public static IEnumerable<T> FineShuffled<T>(this IEnumerable<T> enumerable)
        {
            return InternalShuffle(enumerable, v => 
            {
                byte[] bytes = new byte[sizeof(int)];
                s_crng.GetBytes(bytes);

                //The most significant bit must be 0 so that the generated number is positive (an index).
                bytes[bytes.Length - 1] &= 0b01111111; 
                
                return BitConverter.ToInt32(bytes, 0) % v;
            });
        }
        
        /// <summary>
        /// Pairs up elements from different IEnumerables on-to-one as Key-Value pairs. Given 2 enumerables of n elements 
        /// and m elements respectively with n < m, the result will be an enumerable of n pairs where the Key comes from en1 and
        /// the value comes from en2. 
        /// </summary>
        /// <param name="en1">Left enumerable, key.</param>
        /// <param name="en2">Right enumerable, value.</param>
        /// <typeparam name="TLeft">Key type</typeparam>
        /// <typeparam name="TRight">Value type</typeparam>
        /// <returns></returns>
        public static IEnumerable<(TLeft Left, TRight Right)> PairSequentially<TLeft, TRight>(IEnumerable<TLeft> en1, IEnumerable<TRight> en2)
        {
            var enum1 = en1.GetEnumerator();
            var enum2 = en2.GetEnumerator();
            while(enum1.MoveNext() && enum2.MoveNext())
                yield return (enum1.Current, enum2.Current);
        }

        /// <summary>
        /// Allows to add IComparable elements to a list in an ordered manner.
        /// </summary>
        public static void AddInOrder<T>(this List<T> list, T item) where T : IComparable<T> => 
            AddInOrder<T>(list, Comparer<T>.Default, item);

        /// <summary>
        /// Allows to add elements to a list in an ordered manner. T being generic,
        /// an IComparer instance is required to properly order the items.
        /// </summary>
        public static void AddInOrder<T>(this List<T> list, IComparer<T> comparer, T item)
        {
            if (list.Count == 0 || comparer.Compare(list[list.Count - 1], item) <= 0) 
                list.Add(item);
            else if (comparer.Compare(list[0], item) >= 0) 
                list.Insert(0, item);
            else
            {
                int index = list.BinarySearch(item, comparer);
                if (index < 0) index = ~index;
                list.Insert(index, item);
            }
        }

        /// <summary>
        /// Allows to add IComparable elements to a linked list in an ordered manner.
        /// </summary>
        public static void AddInOrder<T>(this LinkedList<T> list, T item) where T : IComparable<T> =>
            AddInOrder(list, Comparer<T>.Default, item);

        /// <summary>
        /// Allows to add elements to a linked list in an ordered manner. T being generic,
        /// an IComparer instance is required to properly order the items.
        /// </summary>
        public static void AddInOrder<T>(this LinkedList<T> list, IComparer<T> comparer, T item)
        {
            if (list.Count == 0 || comparer.Compare(list.Last.Value, item) <= 0) 
                list.AddLast(item);
            else if (comparer.Compare(list.First.Value, item) >= 0)
                list.AddFirst(item);
            else if (comparer.Compare(item, list.First.Value) <= comparer.Compare(list.Last.Value, item))
            {
                LinkedListNode<T> next = list.First.Next;
                while(comparer.Compare(item, next.Value) > 0) 
                    next = next.Next;
                list.AddBefore(next, item);
            }
            else 
            {
                LinkedListNode<T> previous = list.Last.Previous;
                while(comparer.Compare(item, previous.Value) < 0) 
                    previous = previous.Previous;
                list.AddAfter(previous, item);
            }
        }

        /// <summary>
        /// Swaps positions the elements at index i and j. After a call to Swap, 
        /// the element previously at index i will be placed at index j and the
        /// element previously at index j will be placed at index i.
        /// </summary>
        public static void Swap(this System.Collections.IList list, int i, int j)
        {
            if (i < 0 || i >= list.Count)
                throw new ArgumentOutOfRangeException(nameof(i));
            if (j < 0 || j >= list.Count)
                throw new ArgumentOutOfRangeException(nameof(j));
            object t = list[i];
            list[i] = list[j];
            list[j] = t;
        }
    }
}