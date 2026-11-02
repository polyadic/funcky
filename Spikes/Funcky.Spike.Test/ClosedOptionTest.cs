namespace Funcky.Spike.Test;

public sealed class ClosedOptionTest
{
    [Fact]
    public void CasesAreNestedTypesAndPatternsUseThem()
    {
        ClosedOption<int> option = 5;
        Assert.True(option is ClosedOption<int>.Some);
        Assert.True(option is ClosedOption<int>.Some(5));
        Assert.True(option is ClosedOption<int>.Some(var value) && value == 5);
        Assert.True(ClosedOption.None<int>() is ClosedOption<int>.None);
    }

    [Fact]
    public void ASwitchOverAllCasesNeedsNoCatchAll()
    {
        // No CS8509 here: the compiler knows the hierarchy is closed. A missing arm is a warning (an error in the spike).
        static string Show(ClosedOption<int> option) => option switch
        {
            ClosedOption<int>.Some(var value) => $"some {value}",
            ClosedOption<int>.None => "none",
        };

        Assert.Equal("some 5", Show(5));
        Assert.Equal("none", Show(ClosedOption.None<int>()));
    }

    [Fact]
    public void NoneIsASingletonAndSomeAllocatesOnce()
    {
        var before = GC.GetAllocatedBytesForCurrentThread();
        ClosedOption<int> option = ClosedOption.None<int>();
        for (var i = 0; i < 1000; i++)
        {
            option = i;
        }

        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;
        Assert.InRange(allocated, 24 * 1000, 24 * 1000 + 1024);
        Assert.True(option.IsSome);
        Assert.Same(ClosedOption.None<int>(), ClosedOption.None<string>().Select(_ => 1));
    }

    [Fact]
    public void DefaultIsNullAndTheNullableAnalysisOwnsIt()
    {
        ClosedOption<int>? option = default;
        Assert.Null(option);
        Assert.Throws<NullReferenceException>(() => option!.IsSome);
    }

    [Fact]
    public void RecordEqualityIsStructural()
    {
        ClosedOption<int> a = 5;
        ClosedOption<int> b = 5;
        Assert.Equal(a, b);
        Assert.NotSame(a, b);
        Assert.Equal(ClosedOption.None<int>(), ClosedOption.None<int>());
    }
}
