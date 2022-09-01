using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Funcky.Internal;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    public static async ValueTask<Either<TLeft, IReadOnlyList<TSource>>> SequenceAsync<TLeft, TSource>(
        this IAsyncEnumerable<Either<TLeft, TSource>> source,
        CancellationToken cancellationToken = default)
        where TLeft : notnull
        where TSource : notnull
        => (await source.TraverseAsync(UnsafeEither.FromEither, cancellationToken).ConfigureAwait(false)).ToEither();

    public static async ValueTask<Option<IReadOnlyList<TSource>>> SequenceAsync<TSource>(
        this IAsyncEnumerable<Option<TSource>> source,
        CancellationToken cancellationToken = default)
        where TSource : notnull
        => (await source.TraverseAsync(UnsafeEither.FromOption, cancellationToken).ConfigureAwait(false)).ToOption();

    public static async ValueTask<Result<IReadOnlyList<TSource>>> SequenceAsync<TSource>(
        this IAsyncEnumerable<Result<TSource>> source,
        CancellationToken cancellationToken = default)
        where TSource : notnull
        => (await source.TraverseAsync(UnsafeEither.FromResult, cancellationToken).ConfigureAwait(false)).ToResult();

    [Pure]
    public static Reader<TEnvironment, IAsyncEnumerable<TSource>> Sequence<TEnvironment, TSource>(this IAsyncEnumerable<Reader<TEnvironment, TSource>> source)
        where TEnvironment : notnull
        where TSource : notnull
        => environment
            => source.Select(reader => reader(environment));

    [Pure]
    public static Lazy<IAsyncEnumerable<TSource>> Sequence<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] TSource>(this IAsyncEnumerable<Lazy<TSource>> source)
        => Lazy.FromFunc(new SequenceLazyInternal<TSource>(source).Invoke);

    private static async ValueTask<UnsafeEither<TLeft, IReadOnlyList<TRight>>> TraverseAsync<TSource, TLeft, TRight>(
        this IAsyncEnumerable<TSource> source,
        Func<TSource, UnsafeEither<TLeft, TRight>> selector,
        CancellationToken cancellationToken)
    {
        var builder = ImmutableArray.CreateBuilder<TRight>();

        await foreach (var element in source.ConfigureAwait(false).WithCancellation(cancellationToken))
        {
            var either = selector(element);

            if (!either.IsRight)
            {
                return UnsafeEither<TLeft, IReadOnlyList<TRight>>.Left(either.LeftValue);
            }

            builder.Add(either.RightValue);
        }

        return UnsafeEither<TLeft, IReadOnlyList<TRight>>.Right(builder.ToImmutable());
    }

    private sealed class SequenceLazyInternal<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] TSource>
    {
        private readonly IAsyncEnumerable<Lazy<TSource>> _source;

        public SequenceLazyInternal(IAsyncEnumerable<Lazy<TSource>> source) => _source = source;

        // Workaround for https://github.com/dotnet/linker/issues/1416
        public IAsyncEnumerable<TSource> Invoke() => _source.Select(lazy => lazy.Value);
    }
}
