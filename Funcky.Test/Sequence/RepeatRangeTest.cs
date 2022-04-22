using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.TestUtils;

namespace Funcky.Test;

public sealed class RepeatRangeTest
{
    [Fact]
    public void RepeatRangeIsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerationSequence<object>();

        using var repeatRange = Sequence.RepeatRange(doNotEnumerate, 2);
    }

    [Fact]
    public void RepeatRangeThrowsWhenAlreadyDisposed()
    {
        var repeatRange = Sequence.RepeatRange(Sequence.Return(1337), 5);

#pragma warning disable IDISP016 // we test behaviour after Dispose
#pragma warning disable IDISP017 // we test behaviour after Dispose
        repeatRange.Dispose();
#pragma warning restore IDISP016
#pragma warning restore IDISP017

        Assert.Throws<ObjectDisposedException>(() => repeatRange.ForEach(NoOperation));
    }

    [Fact]
    public void RepeatRangeThrowsWhenAlreadyDisposedEvenIfYouDisposeBetweenMoveNext()
    {
        var list = new List<int> { 1337, 2, 5 };

        var repeats = 5;

        foreach (var i in Enumerable.Range(0, list.Count * repeats))
        {
            var repeatRange = Sequence.RepeatRange(list, repeats);
            using var enumerator = repeatRange.GetEnumerator();

            Assert.True(Enumerable.Range(0, i).All(_ => enumerator.MoveNext()));

#pragma warning disable IDISP016 // we test behaviour after Dispose
#pragma warning disable IDISP017 // we test behaviour after Dispose
            repeatRange.Dispose();
#pragma warning restore IDISP016
#pragma warning restore IDISP017

            Assert.ThrowsAny<ObjectDisposedException>(() => enumerator.MoveNext());
        }
    }

    [Property]
    public Property TheLengthOfTheGeneratedRepeatRangeIsCorrect(List<int> list, NonNegativeInt count)
    {
        using var repeatRange = Sequence.RepeatRange(list, count.Get);

        var materialized = repeatRange.ToList();

        var shouldBeTrue = materialized.Count == list.Count * count.Get;

        return (materialized.Count == list.Count * count.Get).ToProperty();
    }

    [Property]
    public Property TheSequenceRepeatsTheGivenNumberOfTimes(List<int> list, NonNegativeInt count)
    {
        using var repeatRange = Sequence.RepeatRange(list, count.Get);

        return repeatRange
            .IsSequenceRepeating(list)
            .NTimes(count.Get);
    }

    [Property]
    public void RepeatRangeEnumeratesUnderlyingEnumerableOnlyOnce(NonEmptySet<int> sequence)
    {
        var enumerateOnce = new EnumerateOnce<int>(sequence.Get);

        using var repeatRange = Sequence.RepeatRange(enumerateOnce, 3);

        repeatRange.ForEach(NoOperation);
    }
}
