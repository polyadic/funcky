using FsCheck;
using FsCheck.Fluent;
using Funcky.FsCheck;
using Funcky.Test.TestUtilities;

namespace Funcky.Test.Monads;

public sealed partial class OptionTest
{
    [FunckyProperty]
    public Property SequencingEitherIsReversible(Option<Either<int, int>> option)
    {
        return (option == option.Sequence().Sequence()).ToProperty();
    }

    [FunckyProperty]
    public Property SequencingResultIsReversible(Option<Result<int>> option)
    {
        return (option == option.Sequence().Sequence()).ToProperty();
    }

    [Fact]
    public void SequencingEnumerableDoesNotEnumerate()
    {
        _ = Option.Return(new FailOnEnumerationSequence<Unit>().AsEnumerable()).Sequence();
    }

    [FunckyProperty]
    public Property SequencingEnumerableIsReversible(Option<IList<int>> option)
    {
        var optionOfIEnumerable = option.Select(x => x.AsEnumerable());
        var reversed = optionOfIEnumerable.Sequence().Sequence();
        return Match(
            (optionOfIEnumerable, reversed),
            some: (x, y) => x.SequenceEqual(y),
            none: True,
            heterogeneous: False)
            .ToProperty();
    }

    [Fact]
    public void SequencingLazyDoesNotEvaluate()
    {
        _ = Option.Return(Lazy.FromFunc<Unit>(() => throw new InvalidOperationException())).Sequence();
    }

    [FunckyProperty]
    public Property SequencingLazyPreservesSide(Option<Lazy<int>> option)
        => Match(
            (option, option.Sequence().Value),
            some: (x, y) => x.Value == y,
            none: True,
            heterogeneous: False).ToProperty();

    [Fact]
    public void SequencingReaderDoesNotEvaluate()
    {
        _ = Option.Return(Reader<Unit>.FromFunc<Unit>(_ => throw new InvalidOperationException())).Sequence();
    }

    [FunckyProperty]
    public Property SequencingReaderPreservesSide(Option<Reader<int, int>> option, int environment)
        => Match(
            (option, option.Sequence()(environment)),
            some: (x, y) => x(environment) == y,
            none: True,
            heterogeneous: False).ToProperty();

    private static TResult Match<TItem1, TItem2, TResult>(
        (Option<TItem1> X, Option<TItem2> Y) input,
        Func<TItem1, TItem2, TResult> some,
        Func<TResult> none,
        Func<TResult> heterogeneous)
        where TItem1 : notnull
        where TItem2 : notnull
        => input.X.Match(
            some: x => input.Y.Match(some: y => some(x, y), none: heterogeneous),
            none: () => input.Y.Match(some: _ => heterogeneous(), none: none));
}
