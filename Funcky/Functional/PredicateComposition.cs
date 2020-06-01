using System;
using System.Linq;

namespace Funcky
{
    public static partial class Functional
    {
        /// <summary>
        /// Returns a new predicate that is true when all given predicates are true.
        /// </summary>
        public static Predicate<T> All<T>(params Predicate<T>[] predicates)
            => value => predicates.All(predicate => predicate(value));

        /// <summary>
        /// Returns a new predicate that is true when any of the given predicates are true.
        /// </summary>
        public static Predicate<T> Any<T>(params Predicate<T>[] predicates)
            => value => predicates.Any(predicate => predicate(value));

        /// <summary>
        /// Returns a new predicate that inverts the given predicate.
        /// </summary>
        public static Predicate<T> Not<T>(Predicate<T> predicate)
            => value => !predicate(value);

        private static bool True<T>(T value) => true;

        private static bool False<T>(T value) => false;
    }
}
