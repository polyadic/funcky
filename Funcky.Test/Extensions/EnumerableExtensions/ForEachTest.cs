using Funcky.Test.TestUtils;
using Xunit.Sdk;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public class ForEachTest
{
    [Fact]
    public void ForEachIsEvaluatedEagerly()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        Assert.Throws<XunitException>(() => doNotEnumerate.ForEach(NoOperation));
        Assert.Throws<XunitException>(() => doNotEnumerate.ForEach(UnitNoOperation));
    }

    [Fact]
    public void ForEachOnAnEmptyListCallsNothing()
    {
        Enumerable
            .Empty<int>()
            .ForEach(ActionWithException);

        Enumerable
            .Empty<int>()
            .ForEach(UnitActionWithException);
    }

    [Fact]
    public void ForEachExecutesTheActionNTimesWithCorrectValues()
    {
        var state = new List<int>();

        Enumerable
            .Range(0, 42)
            .ForEach(state.Add);

        Assert.Equal(Enumerable.Range(0, 42), state);
    }

    [Fact]
    public void ForEachWithUnitActionExecutesTheActionNTimesWithCorrectValues()
    {
        var state = new List<int>();

        Enumerable
            .Range(0, 42)
            .ForEach(UnitAdd(state));

        Assert.Equal(Enumerable.Range(0, 42), state);
    }

    private static void ActionWithException(int i)
        => throw new XunitException("Should not execute");

    private static Unit UnitActionWithException(int i)
        => throw new XunitException("Should not execute");

    private static Func<int, Unit> UnitAdd(ICollection<int> state)
        => item
            =>
            {
                state.Add(item);
                return Unit.Value;
            };

    private static Unit UnitNoOperation<T>(T value)
        => Unit.Value;
}
