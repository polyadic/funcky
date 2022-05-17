using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;

namespace Funcky.Test.Monads;

public sealed partial class EitherTest
{
    [Property]
    public Property SequencingOptionIsReversible(Either<int, Option<int>> either)
    {
        return (either == either.Sequence().Sequence()).ToProperty();
    }

    [Property]
    public Property SequencingResultIsReversible(Either<int, Result<int>> either)
    {
        return (either == either.Sequence().Sequence()).ToProperty();
    }

    [Fact]
    public void SequencingEnumerableDoesNotEnumerate()
    {
        _ = Either<Unit>.Return(new FailOnEnumerationSequence<Unit>().AsEnumerable()).Sequence();
    }

    [Property]
    public Property SequencingEnumerableIsReversible(Either<int, IList<int>> either)
    {
        var eitherOfIEnumerable = either.Select(x => x.AsEnumerable());
        var reversed = eitherOfIEnumerable.Sequence().Sequence();
        return Match(
            (eitherOfIEnumerable, reversed),
            left: (x, y) => x == y,
            right: (x, y) => x.SequenceEqual(y),
            heterogeneous: False)
            .ToProperty();
    }

    [Fact]
    public void SequencingLazyDoesNotEvaluate()
    {
        _ = Either<Unit>.Return(Lazy.FromFunc<Unit>(() => throw new InvalidOperationException())).Sequence();
    }

    [Property]
    public Property SequencingLazyPreservesSide(Either<int, Lazy<int>> either)
        => Match(
            (either, either.Sequence().Value),
            left: (x, y) => x == y,
            right: (x, y) => x.Value == y,
            heterogeneous: False).ToProperty();

    [Fact]
    public void SequencingReaderDoesNotEvaluate()
    {
        _ = Either<Unit>.Return(Reader<Unit>.FromFunc<Unit>(_ => throw new InvalidOperationException())).Sequence();
    }

    [Property]
    public Property SequencingReaderPreservesSide(Either<int, Reader<int, int>> either, int environment)
        => Match(
            (either, either.Sequence()(environment)),
            left: (x, y) => x == y,
            right: (x, y) => x(environment) == y,
            heterogeneous: False).ToProperty();

    private static TResult Match<TLeft1, TRight1, TLeft2, TRight2, TResult>(
        (Either<TLeft1, TRight1> X, Either<TLeft2, TRight2> Y) input,
        Func<TLeft1, TLeft2, TResult> left,
        Func<TRight1, TRight2, TResult> right,
        Func<TResult> heterogeneous)
        => input.X.Match(
            left: leftX => input.Y.Match(left: leftY => left(leftX, leftY), right: _ => heterogeneous()),
            right: rightX => input.Y.Match(left: _ => heterogeneous(), right: rightY => right(rightX, rightY)));
}
