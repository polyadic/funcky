using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Funcky.Internal;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    extension<TLeft, TSource>(IEnumerable<Either<TLeft, TSource>> source)
        where TLeft : notnull
        where TSource : notnull
    {
        [Pure]
        public Either<TLeft, IReadOnlyList<TSource>> Sequence()
            => source.Traverse(UnsafeEither.FromEither).ToEither();
    }

    extension<TSource>(IEnumerable<Option<TSource>> source)
        where TSource : notnull
    {
        [Pure]
        public Option<IReadOnlyList<TSource>> Sequence()
            => source.Traverse(UnsafeEither.FromOption).ToOption();
    }

    extension<TSource>(IEnumerable<Result<TSource>> source)
        where TSource : notnull
    {
        [Pure]
        public Result<IReadOnlyList<TSource>> Sequence()
            => source.Traverse(UnsafeEither.FromResult).ToResult();
    }

    extension<TEnvironment, TSource>(IEnumerable<Reader<TEnvironment, TSource>> sequence)
        where TEnvironment : notnull
        where TSource : notnull
    {
        [Pure]
        public Reader<TEnvironment, IEnumerable<TSource>> Sequence()
            => environment
                => sequence.Select(reader => reader(environment));
    }

    extension<TSource>(IEnumerable<TSource> source)
    {
        private UnsafeEither<TLeft, IReadOnlyList<TRight>> Traverse<TLeft, TRight>(
            Func<TSource, UnsafeEither<TLeft, TRight>> selector)
        {
            var builder = ImmutableArray.CreateBuilder<TRight>();

            foreach (var element in source)
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

    extension<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] TSource>(IEnumerable<Lazy<TSource>> sequence)
    {
        [Pure]
        public Lazy<IEnumerable<TSource>> Sequence()
            => Lazy.FromFunc(new SequenceLazy<TSource>(sequence).Invoke);
    }

    // Workaround for https://github.com/dotnet/linker/issues/1416
    private class SequenceLazy<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] TSource>(IEnumerable<Lazy<TSource>> source)
    {
        private readonly IEnumerable<Lazy<TSource>> _source = source;

        public IEnumerable<TSource> Invoke() => _source.Select(static lazy => lazy.Value);
    }
}
