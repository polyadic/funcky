#if ASYNC_SUPPORTED
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Funcky.Internal;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Funcky.Extensions;

public static partial class AsyncEnumerableExtensions
{
    extension<TLeft, TSource>(IAsyncEnumerable<Either<TLeft, TSource>> source)
        where TLeft : notnull
        where TSource : notnull
    {
        public async ValueTask<Either<TLeft, IReadOnlyList<TSource>>> SequenceAsync(
            CancellationToken cancellationToken = default)
            => (await source.TraverseAsync(UnsafeEither.FromEither, cancellationToken).ConfigureAwait(false)).ToEither();
    }

    extension<TSource>(IAsyncEnumerable<Option<TSource>> source)
        where TSource : notnull
    {
        public async ValueTask<Option<IReadOnlyList<TSource>>> SequenceAsync(
            CancellationToken cancellationToken = default)
            => (await source.TraverseAsync(UnsafeEither.FromOption, cancellationToken).ConfigureAwait(false)).ToOption();
    }

    extension<TSource>(IAsyncEnumerable<Result<TSource>> source)
        where TSource : notnull
    {
        public async ValueTask<Result<IReadOnlyList<TSource>>> SequenceAsync(
            CancellationToken cancellationToken = default)
            => (await source.TraverseAsync(UnsafeEither.FromResult, cancellationToken).ConfigureAwait(false)).ToResult();
    }

    extension<TEnvironment, TSource>(IAsyncEnumerable<Reader<TEnvironment, TSource>> source)
        where TEnvironment : notnull
        where TSource : notnull
    {
        [Pure]
        public Reader<TEnvironment, IAsyncEnumerable<TSource>> Sequence()
            => environment
                => source.Select(reader => reader(environment));
    }

    extension<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] TSource>(IAsyncEnumerable<Lazy<TSource>> source)
    {
        [Pure]
        public Lazy<IAsyncEnumerable<TSource>> Sequence()
            => Lazy.FromFunc(new SequenceLazyInternal<TSource>(source).Invoke);
    }

    extension<TSource>(IAsyncEnumerable<TSource> source)
    {
        private async ValueTask<UnsafeEither<TLeft, IReadOnlyList<TRight>>> TraverseAsync<TLeft, TRight>(
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
    }

    private sealed class SequenceLazyInternal<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] TSource>(
        IAsyncEnumerable<Lazy<TSource>> source)
    {
        // Workaround for https://github.com/dotnet/linker/issues/1416
        public IAsyncEnumerable<TSource> Invoke() => source.Select(lazy => lazy.Value);
    }
}
#endif
