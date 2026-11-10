using System.Runtime.CompilerServices;

namespace Funcky.Spike.Test;

public sealed class CustomUnionEitherResultTest
{
    [Fact]
    public void EitherCasesArePayloadTypes()
    {
        var left = CustomUnionEither<string, int>.Left("l");
        var right = CustomUnionEither<string, int>.Right(5);
        Assert.True(left is string);
        Assert.True(right is int);
        Assert.False(left is int);
        Assert.Equal("left l", Show(left));
        Assert.Equal("right 5", Show(right));
    }

    [Fact]
    public void EitherDefaultIsNullValueAndThrowsInSwitch()
    {
        CustomUnionEither<string, int> either = default;
        Assert.Null(either.Value);
        Assert.False(either is string);
        Assert.False(either is int);
#pragma warning disable CS8655
        Assert.Throws<SwitchExpressionException>(() => either switch
        {
            string => "left",
            int => "right",
        });
#pragma warning restore CS8655
        Assert.Throws<NotSupportedException>(() => either.Match(left: _ => 0, right: _ => 1));
    }

    [Fact]
    public void EitherWithIdenticalCaseTypesIsStillConstructibleButATypePatternOnlySeesTheFirstCase()
    {
        // Both cases are int. With the non-boxing access members the compiler picks ONE TryGetValue for `is int`
        // (the spec says "implementation-defined"; RC3 takes the first, out TLeft), so a Right is NOT `int`.
        // Without them (Value only) both sides matched. Either way the side is only recoverable through Match.
        var left = CustomUnionEither<int, int>.Left(1);
        var right = CustomUnionEither<int, int>.Right(2);
        Assert.True(left is int);
        Assert.False(right is int);
        Assert.Equal(0, left.Match(left: _ => 0, right: _ => 1));
        Assert.Equal(1, right.Match(left: _ => 0, right: _ => 1));
    }

    [Fact]
    public void EitherSizeAndAllocation()
    {
        Assert.Equal(Unsafe.SizeOf<Funcky.Monads.Either<string, int>>(), Unsafe.SizeOf<CustomUnionEither<string, int>>());

        Assert.Equal(0, Allocated(() =>
        {
            var sum = 0;
            for (var i = 0; i < 1000; i++)
            {
                sum += CustomUnionEither<string, int>.Right(i).Select(x => x + 1).Match(left: _ => 0, right: x => x);
            }
        }));

        Assert.Equal(0, Allocated(() =>
        {
            var sum = 0;
            for (var i = 0; i < 1000; i++)
            {
                sum += CustomUnionEither<string, int>.Right(i) is int value ? value : 0;
            }
        }));

        // Generic code: with the non-boxing access members the match goes through TryGetValue(out TRight), no box.
        Assert.Equal(0, Allocated(() =>
        {
            var sum = 0;
            for (var i = 0; i < 1000; i++)
            {
                sum += RightOrZero(CustomUnionEither<string, int>.Right(i));
            }
        }));
    }

    [Fact]
    public void ResultCasesAreTheValueAndException()
    {
        CustomUnionResult<int> ok = 5;
        var error = CustomUnionResult<int>.Error(new InvalidOperationException("x"));
        Assert.True(ok is int);
        Assert.True(error is Exception);
        Assert.True(error is InvalidOperationException { Message: "x" });
        Assert.Equal("ok 5", ShowResult(ok));
        Assert.Equal("error InvalidOperationException", ShowResult(error));
    }

    [Fact]
    public void ResultOfExceptionCollapsesTheCases()
    {
        // Result<Exception>: ok and error cases are both Exception. `is Exception` resolves to TryGetValue(out T),
        // so the ERROR case is not `Exception` any more: the idiomatic error check gives the wrong answer for this
        // one instantiation. Match still works. (Without the non-boxing members both were `Exception`.)
        var ok = CustomUnionResult<Exception>.Ok(new InvalidOperationException());
        var error = CustomUnionResult<Exception>.Error(new InvalidOperationException());
        Assert.True(ok is Exception);
        Assert.False(error is Exception);
        Assert.Equal("ok", ok.Match(error: _ => "error", ok: _ => "ok"));
        Assert.Equal("error", error.Match(error: _ => "error", ok: _ => "ok"));
    }

    [Fact]
    public void ResultDefaultIsNullValue()
    {
        CustomUnionResult<int> result = default;
        Assert.Null(((IUnion)result).Value); // Value is hidden behind the provider interface
        Assert.False(result is int);
        Assert.False(result is Exception);
    }

    [Fact]
    public void ResultSizeAndAllocation()
    {
        Assert.Equal(Unsafe.SizeOf<Funcky.Monads.Result<int>>(), Unsafe.SizeOf<CustomUnionResult<int>>());

        Assert.Equal(0, Allocated(() =>
        {
            var sum = 0;
            for (var i = 0; i < 1000; i++)
            {
                sum += CustomUnionResult<int>.Ok(i).Select(x => x + 1).Match(error: _ => 0, ok: x => x);
            }
        }));

        var exception = new InvalidOperationException();
        Assert.Equal(0, Allocated(() =>
        {
            var sum = 0;
            for (var i = 0; i < 1000; i++)
            {
                sum += CustomUnionResult<int>.Error(exception) is Exception ? 1 : 0;
            }
        }));

        Assert.Equal(0, Allocated(() =>
        {
            var sum = 0;
            for (var i = 0; i < 1000; i++)
            {
                sum += OkOrZero(CustomUnionResult<int>.Ok(i));
            }
        }));
    }

    private static int RightOrZero<TLeft, TRight>(CustomUnionEither<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
        => either is TRight right ? right.GetHashCode() : 0;

    private static int OkOrZero<T>(CustomUnionResult<T> result)
        where T : notnull
        => result is T value ? value.GetHashCode() : 0;

    private static long Allocated(Action body)
    {
        body();
        var before = GC.GetAllocatedBytesForCurrentThread();
        body();
        return GC.GetAllocatedBytesForCurrentThread() - before;
    }

    private static string Show<TLeft, TRight>(CustomUnionEither<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
        => either switch
        {
            TLeft left => $"left {left}",
            TRight right => $"right {right}",
        };

    private static string ShowResult<T>(CustomUnionResult<T> result)
        where T : notnull
        => result switch
        {
            T value => $"ok {value}",
            Exception exception => $"error {exception.GetType().Name}",
        };
}
