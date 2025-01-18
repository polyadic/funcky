using FsCheck;
using FsCheck.Fluent;
using Funcky.FsCheck;
using Funcky.Test.TestUtils;
using Result = Funcky.Monads.Result;

namespace Funcky.Test.Monads;

public sealed partial class ResultTest
{
    [FunckyProperty]
    public Property SequencingOptionIsReversible(Result<Option<int>> result)
    {
        return (result == result.Sequence().Sequence()).ToProperty();
    }

    [FunckyProperty]
    public Property SequencingEitherIsReversible(Result<Either<int, int>> result)
    {
        return (result == result.Sequence().Sequence()).ToProperty();
    }

    [Fact]
    public void SequencingEnumerableDoesNotEnumerate()
    {
        _ = Result.Return(new FailOnEnumerationSequence<Unit>().AsEnumerable()).Sequence();
    }

    [FunckyProperty]
    public Property SequencingEnumerableIsReversible(Result<IList<int>> result)
    {
        var resultOfIEnumerable = result.Select(x => x.AsEnumerable());
        var reversed = resultOfIEnumerable.Sequence().Sequence();
        return Match(
            (resultOfIEnumerable, reversed),
            ok: (x, y) => x.SequenceEqual(y),
            error: (x, y) => x == y,
            heterogeneous: False)
            .ToProperty();
    }

    [Fact]
    public void SequencingLazyDoesNotEvaluate()
    {
        _ = Result.Return(Lazy.FromFunc<Unit>(() => throw new InvalidOperationException())).Sequence();
    }

    [FunckyProperty]
    public Property SequencingLazyPreservesSide(Result<Lazy<int>> result)
        => Match(
            (result, result.Sequence().Value),
            ok: (x, y) => x.Value == y,
            error: (x, y) => x == y,
            heterogeneous: False).ToProperty();

    [Fact]
    public void SequencingReaderDoesNotEvaluate()
    {
        _ = Result.Return(Reader<Unit>.FromFunc<Unit>(_ => throw new InvalidOperationException())).Sequence();
    }

    [FunckyProperty]
    public Property SequencingReaderPreservesSide(Result<Reader<int, int>> result, int environment)
        => Match(
            (result, result.Sequence()(environment)),
            ok: (x, y) => x(environment) == y,
            error: (x, y) => x == y,
            heterogeneous: False).ToProperty();

    private static TResult Match<TValidResult1, TValidResult2, TResult>(
        (Result<TValidResult1> X, Result<TValidResult2> Y) input,
        Func<TValidResult1, TValidResult2, TResult> ok,
        Func<Exception, Exception, TResult> error,
        Func<TResult> heterogeneous)
        where TValidResult1 : notnull
        where TValidResult2 : notnull
        => input.X.Match(
            ok: x => input.Y.Match(ok: y => ok(x, y), error: _ => heterogeneous()),
            error: x => input.Y.Match(ok: _ => heterogeneous(), error: y => error(x, y)));
}
