using System.Collections.Immutable;
using FsCheck;
using FsCheck.Xunit;
using Funcky.FsCheck;
using Result = Funcky.Monads.Result;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class SequenceTest
{
    public SequenceTest()
        => FunckyGenerators.Register();

    [Property]
    public Property SequencingEitherReturnsLeftWhenOneOrMoreElementsAreLeft(IList<Either<int, int>> list)
        => IsLeft(list.Sequence())
            .ToProperty()
            .When(list.Any(IsLeft));

    [Property]
    public Property SequencingEitherReturnsFirstLeftValue(IList<int> list)
        => list.Select(Either<int, Unit>.Left).Sequence()
            .Match(right: False, left: result => result == list.FirstOrNone())
            .When(list.Count >= 1);

    [Property]
    public Property SequencingEitherReturnsRightWhenAllElementsAreRight(IList<int> list)
        => IsRight(list.Select(Either<Unit>.Return).Sequence()).ToProperty();

    [Property]
    public Property SequencingEitherPreservesValuesWhenAllElementsAreRight(IList<int> list)
        => list.Select(Either<Unit>.Return).Sequence()
            .Match(left: False, right: result => result.SequenceEqual(list))
            .ToProperty();

    [Property]
    public Property SequencingOptionReturnsNoneWhenOneOrMoreElementsAreNone(IList<Option<int>> list)
        => IsNone(list.Sequence())
            .When(list.Any(IsNone));

    [Property]
    public Property SequencingOptionReturnsSomeWhenAllElementsAreSome(IList<int> list)
        => IsSome(list.Select(Option.Some).Sequence()).ToProperty();

    [Property]
    public Property SequencingOptionPreservesValuesWhenAllElementsAreSome(IList<int> list)
        => list.Select(Option.Some).Sequence()
            .Match(none: false, some: result => result.SequenceEqual(list))
            .ToProperty();

    [Property]
    public Property SequencingResultReturnsErrorWhenOneOrMoreElementsAreError(IList<Result<int>> list)
        => IsError(list.Sequence())
            .ToProperty()
            .When(list.Any(IsError));

    [Property]
    public Property SequencingResultReturnsFirstError(IList<string> list)
    {
        var exceptions = list.Select(message => new EquatableException(message)).ToImmutableArray();
        return exceptions.Select(Result<Unit>.Error).Sequence()
            .Match(ok: False, error: exception => exception == exceptions.FirstOrNone().Select(x => x as Exception))
            .When(list.Count >= 1);
    }

    [Property]
    public Property SequencingResultReturnsOkWhenAllElementsAreOk(IList<int> list)
        => IsOk(list.Select(Result.Return).Sequence()).ToProperty();

    [Property]
    public Property SequencingResultPreservesValuesWhenAllElementsAreOk(IList<int> list)
        => list.Select(Result.Return).Sequence()
            .Match(error: False, ok: result => result.SequenceEqual(list))
            .ToProperty();

    private static bool IsLeft<TLeft, TRight>(Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
        => either.Match(left: True, right: False);

    private static bool IsRight<TLeft, TRight>(Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
        => either.Match(left: False, right: True);

    private static bool IsSome<TItem>(Option<TItem> option)
        where TItem : notnull
        => option.Match(none: false, some: True);

    private static bool IsNone<TItem>(Option<TItem> option)
        where TItem : notnull
        => option.Match(none: true, some: False);

    private static bool IsError<TValidResult>(Result<TValidResult> result)
        where TValidResult : notnull
        => result.Match(ok: False, error: True);

    private static bool IsOk<TValidResult>(Result<TValidResult> result)
        where TValidResult : notnull
        => result.Match(ok: True, error: False);
}
