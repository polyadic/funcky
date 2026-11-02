using System.Runtime.CompilerServices;
using Funcky.Monads;

namespace Funcky.Spike.Test;

/// <summary>The measurable facts that go into the #1034 write-up, as assertions so they stay true while the spike evolves.</summary>
public sealed class ShapeFactsTest
{
    [Fact]
    public void SizesInBytes()
    {
        // Struct Option<int>: bool + int, padded. Union: one object reference. Closed record: one object reference.
        Assert.Equal(8, Unsafe.SizeOf<Option<int>>());
        Assert.Equal(8, Unsafe.SizeOf<UnionOption<int>>());
        Assert.Equal(8, Unsafe.SizeOf<ClosedOption<int>>());

        // The struct grows with its payload, the union and the record do not (they point to a heap object instead).
        Assert.Equal(20, Unsafe.SizeOf<Option<Guid>>());
        Assert.Equal(8, Unsafe.SizeOf<UnionOption<Guid>>());
        Assert.Equal(16, Unsafe.SizeOf<Option<string>>());
        Assert.Equal(8, Unsafe.SizeOf<UnionOption<string>>());
    }

    [Fact]
    public void TheUnionIsAStructWithOneObjectFieldAndTheIUnionInterface()
    {
        var type = typeof(UnionOption<int>);
        Assert.True(type.IsValueType);
        Assert.Contains(type.GetInterfaces(), i => i.FullName == "System.Runtime.CompilerServices.IUnion");
        Assert.Equal(typeof(object), type.GetProperty("Value")!.PropertyType);
        Assert.Contains(type.CustomAttributes, a => a.AttributeType.Name == "UnionAttribute");
    }

    [Fact]
    public void TheStructOptionDoesNotAllocateTheOthersDo()
    {
        static long Allocated(Action body)
        {
            var before = GC.GetAllocatedBytesForCurrentThread();
            body();
            return GC.GetAllocatedBytesForCurrentThread() - before;
        }

        // Construct + pattern match, no delegates involved.
        Assert.Equal(0, Allocated(() =>
        {
            var sum = 0;
            for (var i = 0; i < 1000; i++)
            {
                sum += Option.Some(i) is [var value] ? value : 0;
            }
        }));

        Assert.InRange(Allocated(() =>
        {
            var sum = 0;
            for (var i = 0; i < 1000; i++)
            {
                sum += ClosedOption.Some(i) is ClosedOption<int>.Some(var value) ? value : 0;
            }
        }), 24 * 1000, 24 * 1000 + 1024);

        Assert.InRange(Allocated(() =>
        {
            var sum = 0;
            for (var i = 0; i < 1000; i++)
            {
                sum += UnionOption.Some(i) is Some<int>(var value) ? value : 0;
            }
        }), 24 * 1000, 24 * 1000 + 1024);
    }

    /// <summary>Side finding: today's <c>Option.Select</c> is implemented via <c>Match</c> with a lambda that captures the
    /// selector, so every Select allocates a closure and a delegate (80 bytes in Release) although the option itself is
    /// a struct. A branch-based Select (as the spike shapes use) would make Select allocation-free. Worth an issue of its own.
    /// (Measured against the Release build of Funcky; the Debug build additionally boxes in the constructor's null check.)</summary>
    [Fact]
    public void TodaysStructOptionSelectAllocatesAClosurePerCall()
    {
        static long Allocated(Action body)
        {
            body();
            var before = GC.GetAllocatedBytesForCurrentThread();
            body();
            return GC.GetAllocatedBytesForCurrentThread() - before;
        }

        var structSelect = Allocated(() =>
        {
            for (var i = 0; i < 1000; i++)
            {
                _ = Option.Some(i).Select(x => x + 1);
            }
        });

        var closedSelect = Allocated(() =>
        {
            for (var i = 0; i < 1000; i++)
            {
                _ = ClosedOption.Some(i).Select(x => x + 1);
            }
        });

        var unionSelect = Allocated(() =>
        {
            for (var i = 0; i < 1000; i++)
            {
                _ = UnionOption.Some(i).Select(x => x + 1);
            }
        });

        Assert.InRange(structSelect, 80 * 1000, 80 * 1000 + 1024);
        Assert.InRange(closedSelect, 48 * 1000, 48 * 1000 + 1024);
        Assert.InRange(unionSelect, 48 * 1000, 48 * 1000 + 1024);
    }
}
