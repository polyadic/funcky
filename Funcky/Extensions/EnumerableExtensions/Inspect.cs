using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    /// <typeparam name="TSource">the inner type of the enumerable.</typeparam>
    extension<TSource>(IEnumerable<TSource> source)
    {
        /// <summary>
        /// An IEnumerable that calls a function on each element before yielding it. It can be used to encode side effects without enumerating.
        /// The side effect will be executed when enumerating the result.
        /// </summary>
        /// <returns>returns an <see cref="IEnumerable{T}" /> with the side effect defined by action encoded in the enumerable.</returns>
        [Pure]
        public IEnumerable<TSource> Inspect(Action<TSource> inspector)
            => ArgumentValidator.NotNull(source).Select(InspectElement(ArgumentValidator.NotNull(inspector)));
    }

    private static Func<TSource, TSource> InspectElement<TSource>(Action<TSource> inspector)
        => element
            =>
            {
                inspector(element);
                return element;
            };
}
