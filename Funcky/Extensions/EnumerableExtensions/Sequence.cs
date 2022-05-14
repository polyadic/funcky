using System.Collections.Immutable;

namespace Funcky.Extensions;

public static partial class EnumerableExtensions
{
    [Pure]
    public static Either<TLeft, IReadOnlyList<TSource>> Sequence<TLeft, TSource>(this IEnumerable<Either<TLeft, TSource>> source)
    {
        var builder = ImmutableArray.CreateBuilder<TSource>();

        foreach (var either in source)
        {
            if (!either.TryGetRight(out var right, out var left))
            {
                return Either<TLeft, IReadOnlyList<TSource>>.Left(left);
            }

            builder.Add(right);
        }

        return builder.ToImmutable();
    }

    [Pure]
    public static Option<IReadOnlyList<TSource>> Sequence<TSource>(this IEnumerable<Option<TSource>> source)
        where TSource : notnull
        => source.Traverse(ToEither).RightOrNone();

    [Pure]
    public static Result<IReadOnlyList<TSource>> Sequence<TSource>(this IEnumerable<Result<TSource>> source)
        => source.Traverse(ToEither).ToResult();

    [Pure]
    public static Reader<TEnvironment, IEnumerable<TSource>> Sequence<TEnvironment, TSource>(this IEnumerable<Reader<TEnvironment, TSource>> sequence)
        => environment
            => sequence.Select(reader => reader(environment));

    [Pure]
    public static Lazy<IEnumerable<TSource>> Sequence<TSource>(this IEnumerable<Lazy<TSource>> sequence)
        => Lazy.FromFunc(() => sequence.Select(lazy => lazy.Value));

    private static Either<Exception, TValidResult> ToEither<TValidResult>(Result<TValidResult> result)
        => result.Match(ok: Either<Exception>.Return, error: Either<Exception, TValidResult>.Left);

    private static Either<Unit, TItem> ToEither<TItem>(Option<TItem> option)
        where TItem : notnull
        => option.ToEither(Unit.Value);

    private static Result<TRight> ToResult<TRight>(this Either<Exception, TRight> either)
        => either.Match(left: Result<TRight>.Error, right: Result.Return);
}
