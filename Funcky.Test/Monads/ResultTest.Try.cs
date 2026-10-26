namespace Funcky.Test.Monads;

public sealed partial class ResultTest
{
    [Fact]
    public void TryReturnsOkWhenTheFunctionReturns()
    {
        FunctionalAssert.Ok(42, Result.Try(() => 42));
    }

    [Fact]
    public void TryReturnsErrorWithTheThrownExceptionWhenTheFunctionThrows()
    {
        var expected = new InvalidOperationException("boom");

        var error = FunctionalAssert.Error<InvalidOperationException, int>(Result.Try<int>(() => throw expected));

        Assert.Same(expected, error);
        Assert.NotNull(error.StackTrace);
    }

    [Theory]
    [MemberData(nameof(FatalExceptions))]
    public void TryDoesNotCaptureFatalExceptions(Exception fatal)
    {
        var thrown = Assert.ThrowsAny<Exception>(() => Result.Try<int>(() => throw fatal));
        Assert.Same(fatal, thrown);
    }

    [Fact]
    public void TryCapturesAnOperationCanceledException()
    {
        _ = FunctionalAssert.Error<OperationCanceledException, int>(Result.Try<int>(() => throw new OperationCanceledException()));
    }

    [Fact]
    public async Task TryAsyncReturnsOkWhenTheTaskCompletes()
    {
        FunctionalAssert.Ok(42, await Result.TryAsync(() => Task.FromResult(42)));
        var awaited = await Result.TryAsync(async () =>
        {
            await Task.Yield();
            return 42;
        });
        FunctionalAssert.Ok(42, awaited);
    }

    [Fact]
    public async Task TryAsyncReturnsErrorWhenTheFunctionThrowsBeforeReturningATask()
    {
        var expected = new InvalidOperationException("boom");

        var error = FunctionalAssert.Error<InvalidOperationException, int>(await Result.TryAsync<int>(() => throw expected));

        Assert.Same(expected, error);
    }

    [Fact]
    public async Task TryAsyncReturnsErrorWhenTheTaskFaults()
    {
        var expected = new InvalidOperationException("boom");

        var error = FunctionalAssert.Error<InvalidOperationException, int>(await Result.TryAsync<int>(async () =>
        {
            await Task.Yield();
            throw expected;
        }));

        Assert.Same(expected, error);
    }

    [Fact]
    public async Task TryAsyncDoesNotCaptureCancellation()
    {
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Result.TryAsync(() => Task.FromCanceled<int>(cancellation.Token)));
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => Result.TryAsync<int>(() => throw new OperationCanceledException(cancellation.Token)));
    }

    [Theory]
    [MemberData(nameof(FatalExceptions))]
    public async Task TryAsyncDoesNotCaptureFatalExceptions(Exception fatal)
    {
        var thrown = await Assert.ThrowsAnyAsync<Exception>(() => Result.TryAsync<int>(() => throw fatal));
        Assert.Same(fatal, thrown);

        thrown = await Assert.ThrowsAnyAsync<Exception>(() => Result.TryAsync<int>(() => Task.FromException<int>(fatal)));
        Assert.Same(fatal, thrown);
    }

    public static TheoryData<Exception> FatalExceptions()
        => new((IEnumerable<Exception>)[
            new OutOfMemoryException(),
            new StackOverflowException(),
            new AccessViolationException(),
        ]);
}
