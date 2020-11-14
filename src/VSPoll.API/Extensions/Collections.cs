using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace VSPoll.API.Extensions
{
    public static class Collections
    {
        /// <summary>
        /// Checks if the number of elements in a collection is equal or greater than the expected value.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to count.</param>
        /// <param name="count">The expected number of items in <paramref name="source"/>.</param>
        /// <returns><see langword="true"/> if the <paramref name="source"/> count is equal to <paramref name="count"/>. Otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        [Pure]
        public static bool AtLeast<T>(this IEnumerable<T> source, int count)
            => source.count(count, true);

        /// <summary>
        /// Checks if the number of elements in a collection is equal to the expected value.
        /// </summary>
        /// <typeparam name="T">The type of the elements of <paramref name="source"/>.</typeparam>
        /// <param name="source">An <see cref="IEnumerable{T}"/> to count.</param>
        /// <param name="count">The expected number of items in <paramref name="source"/>.</param>
        /// <returns><see langword="true"/> if the <paramref name="source"/> count is equal to <paramref name="count"/>. Otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        [Pure]
        public static bool Count<T>(this IEnumerable<T> source, int count)
            => source.count(count, false);

        private static bool count<T>(this IEnumerable<T> source, int count, bool stop)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException($"{nameof(count)} cannot be negative");

            if (count == 0)
                return true;

            using var e = source.GetEnumerator();
            while (count-- > 0)
                if (!e.MoveNext())
                    return false;

            return stop || !e.MoveNext();
        }

        /// <summary>
        /// Repeats a <typeparamref name="T"/> for the number of times specified in <paramref name="count"/>.
        /// </summary>
        /// <typeparam name="T">The type of the elements to be returned.</typeparam>
        /// <param name="source">The source item to be repeated.</param>
        /// <param name="count">The number of times the repetition should occur.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> with the specified number of the same <paramref name="source"/> element.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        [Pure]
        public static IEnumerable<T> Repeat<T>(this T source, int count)
            => Enumerable.Repeat(source, count);

        /// <summary>
        /// Append all characters from a collection to a <see cref="string"/>.
        /// </summary>
        /// <param name="source">The <see cref="char"/> collection.</param>
        /// <returns>The <see cref="string"/> containing all characters from the <paramref name="source"/> collection.</returns>
        [Pure]
        public static string AppendAll(this IEnumerable<char> source)
        {
            StringBuilder sb = new();
            foreach (var item in source)
                sb.Append(item);

            return sb.ToString();
        }
    }
}
