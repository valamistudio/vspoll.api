using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

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
    }
}
