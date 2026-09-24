using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Funcky.Test.TestUtilities;

namespace Funcky.Test;

public sealed class RepeatMaterializedTest
{
    [Fact]
    public void IsEnumeratedLazily()
    {
        var doNotEnumerate = new FailOnEnumerateReadOnlyCollection<object>(Count: 0);
        _ = Sequence.RepeatMaterialized(doNotEnumerate, 2);
    }

    [Property]
    public Property ARepeatedEmptySequenceIsStillEmpty(NonNegativeInt count)
    {
        var repeated = Sequence.RepeatMaterialized(Array.Empty<object>(), count.Get);
        return (!repeated.Any()).ToProperty();
    }

    [Property]
    public Property TheLengthOfTheGeneratedSequenceIsCorrect(List<int> list, NonNegativeInt count)
    {
        var repeatRange = Sequence.RepeatMaterialized(list, count.Get);

        var materialized = repeatRange.ToList();

        return (materialized.Count == list.Count * count.Get).ToProperty();
    }

    [Property]
    public Property TheSequenceRepeatsTheGivenNumberOfTimes(List<int> list, NonNegativeInt count)
    {
        var repeatRange = Sequence.RepeatMaterialized(list, count.Get);

        return repeatRange
            .IsSequenceRepeating(list)
            .NTimes(count.Get)
            .ToProperty();
    }
}
