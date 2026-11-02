using System.Runtime.CompilerServices;

namespace Funcky.Spike.Test;

public sealed class CustomUnionOptionTest
{
    [Fact]
    public void CasePatternsWorkOnTodaysStructLayout()
    {
        CustomUnionOption<int> option = 5;
        Assert.True(option is int);
        Assert.True(option is 5);
        Assert.True(option is int value && value == 5);
        Assert.False(option is None);
        Assert.True(CustomUnionOption<int>.None is None);
        Assert.Equal("some 5", Show(option));
        Assert.Equal("none", Show(CustomUnionOption<int>.None));
    }

    [Fact]
    public void DefaultIsNoneNotAThirdState()
    {
        CustomUnionOption<int> option = default;
        Assert.True(option is None);
        Assert.Equal("none", Show(option));
        Assert.Equal(CustomUnionOption<int>.None, option);
    }

    [Fact]
    public void ConstructionMatchAndSelectDoNotAllocateAMatchOnAValueTypePayloadBoxesOnce()
    {
        static long Allocated(Action body)
        {
            body();
            var before = GC.GetAllocatedBytesForCurrentThread();
            body();
            return GC.GetAllocatedBytesForCurrentThread() - before;
        }

        Assert.Equal(0, Allocated(() =>
        {
            var sum = 0;
            for (var i = 0; i < 1000; i++)
            {
                CustomUnionOption<int> option = i;
                sum += option.Select(x => x + 1).Match(none: 0, some: x => x);
            }
        }));

        // A concrete pattern (`is int value`) at a non-generic call site: the JIT elides the box of Value entirely.
        Assert.Equal(0, Allocated(() =>
        {
            var sum = 0;
            for (var i = 0; i < 1000; i++)
            {
                sum += CustomUnionOption.Some(i) is int value ? value : 0;
            }
        }));

        // A generic pattern (`is T value` in a method generic over T): shared generic code cannot elide it, one box per match.
        Assert.InRange(Allocated(() =>
        {
            var sum = 0;
            for (var i = 0; i < 1000; i++)
            {
                sum += GetOrZero(CustomUnionOption.Some(i));
            }
        }), 24 * 1000, 24 * 1000 + 1024);

        Assert.Equal(0, Allocated(() =>
        {
            var sum = 0;
            for (var i = 0; i < 1000; i++)
            {
                sum += CustomUnionOption.Some("x") is string value ? value.Length : 0;
            }
        }));

        Assert.Equal(0, Allocated(() =>
        {
            var sum = 0;
            for (var i = 0; i < 1000; i++)
            {
                sum += CustomUnionOption<int>.None is None ? 1 : 0;
            }
        }));
    }

    [Fact]
    public void SizeGrowsWithThePayloadLikeToday()
    {
        Assert.Equal(8, Unsafe.SizeOf<CustomUnionOption<int>>());
        Assert.Equal(20, Unsafe.SizeOf<CustomUnionOption<Guid>>());
        Assert.Equal(16, Unsafe.SizeOf<CustomUnionOption<string>>());
    }

    private static int GetOrZero<T>(CustomUnionOption<T> option)
        where T : notnull
        => option is T value ? Convert.ToInt32(value) : 0;

    private static string Show<T>(CustomUnionOption<T> option)
        where T : notnull
        => option switch
        {
            T value => $"some {value}",
            None => "none",
        };
}
