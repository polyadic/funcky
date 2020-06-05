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
            => predicates.Any() ? AllInternal(predicates) : True;

        /// <summary>
        /// Returns a new predicate that is true when any of the given predicates are true.
        /// </summary>
        public static Predicate<T> Any<T>(params Predicate<T>[] predicates)
            => predicates.Any() ? AnyInternal(predicates) : True;

        /// <summary>
        /// Returns a new predicate that inverts the given predicate.
        /// </summary>
        public static Predicate<T> Not<T>(Predicate<T> predicate)
            => value => !predicate(value);

        private static Predicate<T> AllInternal<T>(Predicate<T>[] predicates)
            => value => predicates.Aggregate(
                true,
                (result, predicate) => result && predicate(value));

        private static Predicate<T> AnyInternal<T>(Predicate<T>[] predicates)
            => value => predicates.Aggregate(
                false,
                (result, predicate) => result || predicate(value));
    }
}
