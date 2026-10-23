using Funcky.Internal;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    extension<TSource>(IEnumerable<TSource> source)
        where TSource : notnull
    {
        /// <summary>Returns a sequence mapping each element together with its predecessor.</summary>
        /// <exception cref="ArgumentNullException">Thrown when any value in <paramref name="source"/> is <see langword="null"/>.</exception>
        [Pure]
        public IEnumerable<ValueWithPrevious<TSource>> WithPrevious()
            => ArgumentValidator.NotNull(source) switch
            {
                IList<TSource> list => ListWithSelector.Create(list, ValueWithPrevious),
                _ => source.WithPreviousImplementation(),
            };

        private IEnumerable<ValueWithPrevious<TSource>> WithPreviousImplementation()
        {
            var previous = Option<TSource>.None;

            foreach (var value in source)
            {
                yield return new ValueWithPrevious<TSource>(value, previous);
                previous = value;
            }
        }
    }

    private static Func<TSource, int, ValueWithPrevious<TSource>> ValueWithPrevious<TSource>(IList<TSource> list)
        where TSource : notnull
#if OPTIMIZED_ELEMENT_AT
        => (value, index)
            => new(value, list.ElementAtOrNone(IndexOfPrevious(index)));
#else
        => (value, index)
            => new(value, IndexOfPrevious(index) < 0 ? Option<TSource>.None : list[IndexOfPrevious(index)]);
#endif

    private static int IndexOfPrevious(int index)
        => index - 1;
}
