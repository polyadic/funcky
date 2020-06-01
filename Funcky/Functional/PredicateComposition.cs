using System;
using System.Linq;

namespace Funcky
{
    public static partial class Functional
    {
        /// <summary>
        /// Returns a new predicate that is true when all given predicates are true.
        /// </summary>
        public static Func<T, bool> All<T>(params Func<T, bool>[] predicates)
            => value => predicates.All(predicate => predicate(value));

        /// <summary>
        /// Returns a new predicate that is true when any of the given predicates are true.
        /// </summary>
        public static Func<T, bool> Any<T>(params Func<T, bool>[] predicates)
            => value => predicates.Any(predicate => predicate(value));

        /// <summary>
        /// Returns a new predicate that inverts the given predicate.
        /// </summary>
        public static Func<T, bool> Not<T>(Func<T, bool> predicate)
            => value => !predicate(value);

        private static bool True<T>(T value) => true;

        private static bool False<T>(T value) => false;
    }
}
