namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    // The constructors of Funcky.Sequence as static extension members of System.Linq.Enumerable, next to
    // Enumerable.Range, Enumerable.Repeat and Enumerable.Empty.
    extension(Enumerable)
    {
        /// <inheritdoc cref="Funcky.Sequence.Return{TResult}(TResult)"/>
        [Pure]
        public static IReadOnlyList<TResult> Return<TResult>(TResult element)
            => Funcky.Sequence.Return(element);

        /// <inheritdoc cref="Funcky.Sequence.Return{TResult}(TResult[])"/>
        [Pure]
        public static IReadOnlyList<TResult> Return<TResult>(params TResult[] elements)
            => Funcky.Sequence.Return(elements);

        /// <inheritdoc cref="Funcky.Sequence.Concat{TSource}(IEnumerable{TSource}[])"/>
        /// <remarks>With exactly two sequences the call binds to <see cref="Enumerable.Concat{TSource}(IEnumerable{TSource}, IEnumerable{TSource})"/> of the BCL, which does the same.</remarks>
        [Pure]
        public static IEnumerable<TSource> Concat<TSource>(params IEnumerable<TSource>[] sources)
            => Funcky.Sequence.Concat(sources);

        /// <inheritdoc cref="Funcky.Sequence.Concat{TSource}(IEnumerable{IEnumerable{TSource}})"/>
        [Pure]
        public static IEnumerable<TSource> Concat<TSource>(IEnumerable<IEnumerable<TSource>> sources)
            => Funcky.Sequence.Concat(sources);

        /// <inheritdoc cref="Funcky.Sequence.FromNullable{TResult}(TResult?)"/>
        [Pure]
        public static IEnumerable<TResult> FromNullable<TResult>(TResult? element)
            where TResult : class
            => Funcky.Sequence.FromNullable(element);

        /// <inheritdoc cref="Funcky.Sequence.FromNullable{TResult}(TResult?)"/>
        [Pure]
        public static IEnumerable<TResult> FromNullable<TResult>(TResult? element)
            where TResult : struct
            => Funcky.Sequence.FromNullable(element);

        /// <inheritdoc cref="Funcky.Sequence.Successors{TResult}(Option{TResult}, Func{TResult, Option{TResult}})"/>
        [Pure]
        public static IEnumerable<TResult> Successors<TResult>(Option<TResult> first, Func<TResult, Option<TResult>> successor)
            where TResult : notnull
            => Funcky.Sequence.Successors(first, successor);

        /// <inheritdoc cref="Funcky.Sequence.Successors{TResult}(TResult, Func{TResult, Option{TResult}})"/>
        [Pure]
        public static IEnumerable<TResult> Successors<TResult>(TResult first, Func<TResult, Option<TResult>> successor)
            where TResult : notnull
            => Funcky.Sequence.Successors(first, successor);

        /// <inheritdoc cref="Funcky.Sequence.Successors{TResult}(Option{TResult}, Func{TResult, TResult})"/>
        [Pure]
        public static IEnumerable<TResult> Successors<TResult>(Option<TResult> first, Func<TResult, TResult> successor)
            where TResult : notnull
            => Funcky.Sequence.Successors(first, successor);

        /// <inheritdoc cref="Funcky.Sequence.Successors{TResult}(TResult, Func{TResult, TResult})"/>
        [Pure]
        public static IEnumerable<TResult> Successors<TResult>(TResult first, Func<TResult, TResult> successor)
            => Funcky.Sequence.Successors(first, successor);

        /// <inheritdoc cref="Funcky.Sequence.Cycle{TResult}(TResult)"/>
        [Pure]
        public static IEnumerable<TResult> Cycle<TResult>(TResult element)
            => Funcky.Sequence.Cycle(element);

        /// <inheritdoc cref="Funcky.Sequence.CycleRange{TSource}(IEnumerable{TSource})"/>
        [Pure]
        public static IBuffer<TSource> CycleRange<TSource>(IEnumerable<TSource> source)
            => Funcky.Sequence.CycleRange(source);

        /// <inheritdoc cref="Funcky.Sequence.CycleMaterialized{TSource}(IReadOnlyCollection{TSource})"/>
        [Pure]
        public static IEnumerable<TSource> CycleMaterialized<TSource>(IReadOnlyCollection<TSource> source)
            => Funcky.Sequence.CycleMaterialized(source);

        /// <inheritdoc cref="Funcky.Sequence.RepeatRange{TSource}(IEnumerable{TSource}, int)"/>
        [Pure]
        public static IBuffer<TSource> RepeatRange<TSource>(IEnumerable<TSource> source, int count)
            => Funcky.Sequence.RepeatRange(source, count);

        /// <inheritdoc cref="Funcky.Sequence.RepeatMaterialized{TSource}(IReadOnlyCollection{TSource}, int)"/>
        [Pure]
        public static IEnumerable<TSource> RepeatMaterialized<TSource>(IReadOnlyCollection<TSource> source, int count)
            => Funcky.Sequence.RepeatMaterialized(source, count);
    }
}
