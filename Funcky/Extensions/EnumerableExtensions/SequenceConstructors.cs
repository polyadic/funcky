#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
using System.Collections;
using Funcky.Internal.Validators;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    // The constructors of IEnumerable as static extension members of System.Linq.Enumerable, next to
    // Enumerable.Range, Enumerable.Repeat and Enumerable.Empty. Funcky.Sequence offers the same constructors
    // as plain static methods for compilers older than C# 14.
    extension(Enumerable)
    {
        /// <summary>Creates a sequence that contains exactly one element.</summary>
        /// <remarks>This is the monadic <c>Return</c> for <see cref="IEnumerable{T}"/>, the counterpart of <c>Option.Return</c> and <c>Result.Return</c>.</remarks>
        /// <typeparam name="TResult">Type of the element.</typeparam>
        /// <param name="element">The single element of the sequence.</param>
        /// <returns>A materialized sequence with exactly one element.</returns>
        [Pure]
        public static IReadOnlyList<TResult> Return<TResult>(TResult element)
            => Enumerable.Return(elements: element);

        /// <summary>Creates a sequence that contains exactly the given elements, in order.</summary>
        /// <remarks>
        /// <para>The result is a read-only view over <paramref name="elements"/>: the array is not copied, so later changes to an
        /// array passed in show through, but the view cannot be cast back to a mutable array or list.</para>
        /// <para>Passing an array binds to this overload, so <c>Enumerable.Return(array)</c> is a sequence of the array's elements,
        /// not a sequence containing the array. Use <c>Enumerable.Return&lt;T[]&gt;(array)</c> for the latter.</para>
        /// </remarks>
        /// <typeparam name="TResult">Type of the elements.</typeparam>
        /// <param name="elements">The elements of the sequence.</param>
        /// <returns>A materialized, read-only sequence of the given elements.</returns>
        [Pure]
        public static IReadOnlyList<TResult> Return<TResult>(params TResult[] elements)
            => Array.AsReadOnly(elements);

        /// <summary>Concatenates multiple sequences together.</summary>
        /// <remarks>With exactly two sequences the call binds to <see cref="Enumerable.Concat{TSource}(IEnumerable{TSource}, IEnumerable{TSource})"/> of the BCL, which does the same.</remarks>
        [Pure]
        public static IEnumerable<TSource> Concat<TSource>(params IEnumerable<TSource>[] sources)
            => Enumerable.Concat(sources.AsEnumerable());

        /// <summary>Concatenates multiple sequences together.</summary>
        [Pure]
        public static IEnumerable<TSource> Concat<TSource>(IEnumerable<IEnumerable<TSource>> sources)
            => sources.Flatten();

        /// <returns>An <see cref="IEnumerable{T}" /> consisting of a single item or zero items.</returns>
        [Pure]
        public static IEnumerable<TResult> FromNullable<TResult>(TResult? element)
            where TResult : class
            => element is null ? [] : [element];

        /// <returns>An <see cref="IEnumerable{T}" /> consisting of a single item or zero items.</returns>
        [Pure]
        public static IEnumerable<TResult> FromNullable<TResult>(TResult? element)
            where TResult : struct
            => element.HasValue ? [element.Value] : [];

        /// <summary>
        /// Generates a sequence based on a <paramref name="successor"/> function stopping at the first <see cref="Option{TItem}.None"/> value.
        /// This is essentially the inverse operation of an <see cref="Enumerable.Aggregate{T}"/>.
        /// </summary>
        /// <example>
        /// The sequence of all non-negative integers.
        /// <code>
        /// Enumerable.Successors(0, n => n + 1); // [0, 1, 2, ...]
        /// </code>
        /// </example>
        /// <example>
        /// The fibonacci sequence.
        /// <code>
        /// Enumerable.Successors((0, 1), n => (n.Item2, n.Item1 + n.Item2)).Select(n => n.Item1); // [0, 1, 1, 2, 3, 5, 8, ...]
        /// </code>
        /// </example>
        /// <example>
        /// The sequence of all inner exceptions.
        /// <code>
        /// Exception exception = ...;
        /// Enumerable.Successors(exception, e => Option.FromNullable(e.InnerException)); // [exception, exception.InnerException, exception.InnerException.InnerException, ...]
        /// </code>
        /// </example>
        /// <param name="first">The first element of the sequence.</param>
        /// <param name="successor">Generates the next element of the sequence or <see cref="Option{TItem}.None"/> based on the previous item.</param>
        /// <remarks>Use <see cref="Enumerable.Skip{TSource}(IEnumerable{TSource}, int)"/> on the result if you don't want the first item to be included.</remarks>
        [Pure]
        public static IEnumerable<TResult> Successors<TResult>(Option<TResult> first, Func<TResult, Option<TResult>> successor)
            where TResult : notnull
            => SuccessorsIterator(first, ArgumentValidator.NotNull(successor));

        /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).Successors{TResult}(Option{TResult}, Func{TResult, Option{TResult}})" />
        [Pure]
        public static IEnumerable<TResult> Successors<TResult>(TResult first, Func<TResult, Option<TResult>> successor)
            where TResult : notnull
            => Enumerable.Successors(Option.Some(first), successor);

        /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).Successors{TResult}(Option{TResult}, Func{TResult, Option{TResult}})" />
        [Pure]
        public static IEnumerable<TResult> Successors<TResult>(Option<TResult> first, Func<TResult, TResult> successor)
            where TResult : notnull
            => Enumerable.Successors(first, ToOptionSuccessor(ArgumentValidator.NotNull(successor)));

        /// <inheritdoc cref="EnumerableExtensions.extension(Enumerable).Successors{TResult}(Option{TResult}, Func{TResult, Option{TResult}})" />
        /// <remarks>Unlike the other overloads, this one never wraps the elements in an <see cref="Option{T}"/>, so <typeparamref name="TResult"/> is not constrained to <c>notnull</c>: it can produce <c>null</c> elements.</remarks>
        [Pure]
        public static IEnumerable<TResult> Successors<TResult>(TResult first, Func<TResult, TResult> successor)
            => SuccessorsIterator(first, ArgumentValidator.NotNull(successor));

        /// <summary>
        /// Cycles the same element over and over again as an endless generator.
        /// </summary>
        /// <typeparam name="TResult">Type of the element to be cycled.</typeparam>
        /// <param name="element">The element to be cycled.</param>
        /// <returns>Returns an infinite IEnumerable cycling through the same elements.</returns>
        [Pure]
        public static IEnumerable<TResult> Cycle<TResult>(TResult element)
            => Enumerable.Successors(element, Identity);

        /// <summary>
        /// Generates a sequence that contains the same sequence of elements over and over again as an endless generator.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements to be cycled.</typeparam>
        /// <param name="source">The sequence of elements which are cycled. Throws an exception if the sequence is empty.</param>
        /// <returns>Returns an infinite IEnumerable repeating the same sequence of elements.</returns>
        /// <remarks>
        /// Use <see cref="EnumerableExtensions.extension(Enumerable).CycleMaterialized{TSource}(IReadOnlyCollection{TSource})"/> if you need to cycle an already materialized sequence.
        /// The source is not enumerated before the first element is requested, and it is enumerated only once.
        /// </remarks>
        [Pure]
        public static IBuffer<TSource> CycleRange<TSource>(IEnumerable<TSource> source)
            => CycleBuffer.Create(source);

        /// <summary>
        /// Generates a sequence that contains the same sequence of elements over and over again as an endless generator.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements to be cycled.</typeparam>
        /// <param name="source">The sequence of elements which are cycled. Throws an exception if the sequence is empty.</param>
        /// <returns>Returns an infinite IEnumerable repeating the same sequence of elements.</returns>
        /// <remarks>Use <see cref="EnumerableExtensions.extension(Enumerable).CycleRange{TSource}(IEnumerable{TSource})"/> if you need to cycle a lazy sequence.</remarks>
        [Pure]
        public static IEnumerable<TSource> CycleMaterialized<TSource>(IReadOnlyCollection<TSource> source)
            => source.Count > 0
                ? Enumerable.Cycle(source).Flatten()
                : throw new InvalidOperationException("you cannot cycle an empty enumerable");

        /// <summary>
        /// Generates a sequence that contains the same sequence of elements the given number of times.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements to be repeated.</typeparam>
        /// <param name="source">The sequence of elements to be repeated.</param>
        /// <param name="count">The number of times to repeat the sequence in the generated sequence; 0 yields an empty sequence.</param>
        /// <returns>Returns a finite sequence containing the elements of <paramref name="source"/> <paramref name="count"/> times.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        /// <remarks>Use <see cref="EnumerableExtensions.extension(Enumerable).RepeatMaterialized{TSource}(IReadOnlyCollection{TSource}, int)"/> if you need to cycle an already materialized sequence.</remarks>
        [Pure]
        public static IBuffer<TSource> RepeatRange<TSource>(IEnumerable<TSource> source, int count)
            => CycleBuffer.Create(source, RepeatCountValidator.Validate(count));

        /// <summary>
        /// Generates a sequence that contains the same sequence of elements the given number of times.
        /// </summary>
        /// <typeparam name="TSource">Type of the elements to be repeated.</typeparam>
        /// <param name="source">The sequence of elements to be repeated.</param>
        /// <param name="count">The number of times to repeat the sequence in the generated sequence; 0 yields an empty sequence.</param>
        /// <returns>Returns a finite sequence containing the elements of <paramref name="source"/> <paramref name="count"/> times.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
        /// <remarks>Use <see cref="EnumerableExtensions.extension(Enumerable).RepeatRange{TSource}(IEnumerable{TSource}, int)"/> if you need to cycle a lazy sequence.</remarks>
        [Pure]
        public static IEnumerable<TSource> RepeatMaterialized<TSource>(IReadOnlyCollection<TSource> source, int count)
            => Enumerable.Repeat(source, count).Flatten();
    }

    private static IEnumerable<TResult> SuccessorsIterator<TResult>(TResult first, Func<TResult, TResult> successor)
    {
        var item = first;
        while (true)
        {
            yield return item;
            item = successor(item);
        }
    }

    private static Func<TResult, Option<TResult>> ToOptionSuccessor<TResult>(Func<TResult, TResult> successor)
        where TResult : notnull
        => previous => Option.Some(successor(previous));

    private static IEnumerable<TResult> SuccessorsIterator<TResult>(Option<TResult> first, Func<TResult, Option<TResult>> successor)
        where TResult : notnull
    {
        var item = first;
        while (item.TryGetValue(out var itemValue))
        {
            yield return itemValue;
            item = successor(itemValue);
        }
    }

    private static class CycleBuffer
    {
        public static CycleBuffer<TSource> Create<TSource>(IEnumerable<TSource> source, Option<int> maxCycles = default)
            => new(source, maxCycles);
    }

    private sealed class CycleBuffer<T>(IEnumerable<T> source, Option<int> maxCycles = default) : IBuffer<T>
    {
        private readonly List<T> _buffer = [];

        private IEnumerator<T>? _source;
        private bool _sourceExhausted;
        private bool _disposed;

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                DisposeSource();
                _buffer.Clear();
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            ThrowIfDisposed();

            return GetEnumeratorInternal();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            ThrowIfDisposed();

            return GetEnumeratorInternal();
        }

        private IEnumerator<T> GetEnumeratorInternal()
        {
            if (HasNoCycles())
            {
                yield break;
            }

            for (var index = 0; true; ++index)
            {
                ThrowIfDisposed();

                if (index == _buffer.Count)
                {
                    if (TryFetchNext())
                    {
                        _buffer.Add(_source!.Current);
                    }
                    else
                    {
                        break;
                    }
                }

                yield return _buffer[index];
            }

            if (_buffer.Count is 0)
            {
                if (maxCycles.Match(none: true, some: False))
                {
                    throw new InvalidOperationException("you cannot cycle an empty enumerable");
                }
                else
                {
                    yield break;
                }
            }

            // this can change on Dispose!
            var bufferCount = _buffer.Count;

            for (int cycle = 1; IsCycling(cycle); ++cycle)
            {
                for (var index = 0; index < bufferCount; ++index)
                {
                    ThrowIfDisposed();

                    yield return _buffer[index];
                }
            }
        }

        private bool TryFetchNext()
        {
            if (_sourceExhausted)
            {
                return false;
            }

            _source ??= source.GetEnumerator();

            if (_source.MoveNext())
            {
                return true;
            }

            _sourceExhausted = true;
            DisposeSource();
            return false;
        }

        private void DisposeSource()
        {
            _source?.Dispose();
            _source = null;
        }

        private bool HasNoCycles()
            => maxCycles.Match(none: false, some: maxCycles => maxCycles is 0);

        private bool IsCycling(int cycle)
            => maxCycles.Match(
                none: true,
                some: maxCycles => cycle < maxCycles);

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(CycleBuffer));
            }
        }
    }
}
