using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Funcky.Internal;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    [Pure]
    public static Either<TLeft, IReadOnlyList<TSource>> Sequence<TLeft, TSource>(this IEnumerable<Either<TLeft, TSource>> source)
        where TLeft : notnull
        where TSource : notnull
        => source.Traverse(UnsafeEither.FromEither).ToEither();

    [Pure]
    public static Option<IReadOnlyList<TSource>> Sequence<TSource>(this IEnumerable<Option<TSource>> source)
        where TSource : notnull
        => source.Traverse(UnsafeEither.FromOption).ToOption();

    [Pure]
    public static Result<IReadOnlyList<TSource>> Sequence<TSource>(this IEnumerable<Result<TSource>> source)
        => source.Traverse(UnsafeEither.FromResult).ToResult();

    [Pure]
    public static Reader<TEnvironment, IEnumerable<TSource>> Sequence<TEnvironment, TSource>(this IEnumerable<Reader<TEnvironment, TSource>> sequence)
        where TEnvironment : notnull
        where TSource : notnull
        => environment
            => sequence.Select(reader => reader(environment));

    [Pure]
    public static Lazy<IEnumerable<TSource>> Sequence<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] TSource>(this IEnumerable<Lazy<TSource>> sequence)
        => Lazy.FromFunc(new SequenceLazy<TSource>(sequence).Invoke);

    private static UnsafeEither<TLeft, IReadOnlyList<TRight>> Traverse<TSource, TLeft, TRight>(
        this IEnumerable<TSource> source,
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

    // Workaround for https://github.com/dotnet/linker/issues/1416
    private class SequenceLazy<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] TSource>
    {
        private readonly IEnumerable<Lazy<TSource>> _source;

        public SequenceLazy(IEnumerable<Lazy<TSource>> source) => _source = source;

        public IEnumerable<TSource> Invoke() => _source.Select(static lazy => lazy.Value);
    }
}
