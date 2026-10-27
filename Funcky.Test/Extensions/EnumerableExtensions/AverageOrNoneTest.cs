using FsCheck;
using FsCheck.Fluent;
using FsCheck.Xunit;
using Funcky.Test.Internal;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class AverageOrNoneTest
{
    // Int32/int Tests
    [Property]
    public Property AverageOrNoneGivesTheSameResultAsAverageForInt32(List<int> sequence)
        => CompareAverageAndHandleEmptyInt32Sequence(sequence).ToProperty();

    [Property]
    public Property AverageOrNoneGivesTheSameResultAsAverageForNullableInt32(List<int?> sequence)
        => (Option.FromNullable(sequence.Average())
                == sequence.Select(Option.FromNullable).AverageOrNone()).ToProperty();

    [Property]
    public Property AverageOrNoneWithSelectorGivesTheSameResultAsAverageForNullableInt32(List<int?> sequence, Func<int?, int?> selector)
        => (Option.FromNullable(sequence.Average(selector))
            == sequence.Select(Option.FromNullable).AverageOrNone(SelectorTransformation.TransformNullableSelector(selector))).ToProperty();

    // Int64/long Tests
    [Property]
    public Property AverageOrNoneGivesTheSameResultAsAverageForInt64(List<long> sequence)
        => CompareAverageAndHandleEmptyInt64Sequence(sequence).ToProperty();

    [Property]
    public Property AverageOrNoneGivesTheSameResultAsAverageForNullableInt64(List<long?> sequence)
        => (Option.FromNullable(sequence.Average())
            == sequence.Select(Option.FromNullable).AverageOrNone()).ToProperty();

    [Property]
    public Property AverageOrNoneWithSelectorGivesTheSameResultAsAverageForNullableInt64(List<long?> sequence, Func<long?, long?> selector)
        => (Option.FromNullable(sequence.Average(selector))
            == sequence.Select(Option.FromNullable).AverageOrNone(SelectorTransformation.TransformNullableSelector(selector))).ToProperty();

    // Single/float Tests
    [Property]
    public Property AverageOrNoneGivesTheSameResultAsAverageForSingle(List<float> sequence)
        => CompareAverageAndHandleEmptySingleSequence(sequence).ToProperty();

    [Property]
    public Property AverageOrNoneGivesTheSameResultAsAverageForNullableSingle(List<float?> sequence)
        => (Option.FromNullable(sequence.Average())
            == sequence.Select(Option.FromNullable).AverageOrNone()).ToProperty();

    [Property]
    public Property AverageOrNoneWithSelectorGivesTheSameResultAsAverageForNullableSingle(List<float?> sequence, Func<float?, float?> selector)
        => (Option.FromNullable(sequence.Average(selector))
            == sequence.Select(Option.FromNullable).AverageOrNone(SelectorTransformation.TransformNullableSelector(selector))).ToProperty();

    // Double/double Tests
    [Property]
    public Property AverageOrNoneGivesTheSameResultAsAverageForDouble(List<double> sequence)
        => CompareAverageAndHandleEmptyDoubleSequence(sequence).ToProperty();

    [Property]
    public Property AverageOrNoneGivesTheSameResultAsAverageForNullableDouble(List<double?> sequence)
        => (Option.FromNullable(sequence.Average())
            == sequence.Select(Option.FromNullable).AverageOrNone()).ToProperty();

    [Property]
    public Property AverageOrNoneWithSelectorGivesTheSameResultAsAverageForNullableDouble(List<double?> sequence, Func<double?, double?> selector)
        => (Option.FromNullable(sequence.Average(selector))
            == sequence.Select(Option.FromNullable).AverageOrNone(SelectorTransformation.TransformNullableSelector(selector))).ToProperty();

    // decimal
    [Property]
    public Property AverageOrNoneGivesTheSameResultAsAverageForDecimal(List<decimal> sequence)
        => CompareAverageAndHandleEmptyDecimalSequence(sequence).ToProperty();

    [Property]
    public Property AverageOrNoneGivesTheSameResultAsAverageForNullableDecimal(List<decimal?> sequence)
        => (Option.FromNullable(sequence.Average())
            == sequence.Select(Option.FromNullable).AverageOrNone()).ToProperty();

    [Property]
    public Property AverageOrNoneWithSelectorGivesTheSameResultAsAverageForNullableDecimal(List<decimal?> sequence, Func<decimal?, decimal?> selector)
        => (Option.FromNullable(sequence.Average(selector))
            == sequence.Select(Option.FromNullable).AverageOrNone(SelectorTransformation.TransformNullableSelector(selector))).ToProperty();

    [Fact]
    public void AverageOrNoneSumsInt64ValuesExactlyLikeAverage()
    {
        // Each term is not representable as a double, so summing in double drifts while an exact long sum does not.
        var sequence = Enumerable.Repeat((1L << 53) + 1, 3).ToList();

        Assert.Some(sequence.Average(), sequence.AverageOrNone());
    }

    [Fact]
    public void AverageOrNoneThrowsWhenTheInt64SumOverflowsLikeAverage()
    {
        var sequence = Enumerable.Return(long.MaxValue, 1L);

        Assert.Throws<OverflowException>(() => sequence.Average());
        Assert.Throws<OverflowException>(() => sequence.AverageOrNone());
    }

    private static bool CompareAverageAndHandleEmptyInt32Sequence(IReadOnlyCollection<int> sequence)
        => sequence.Count == 0
            ? sequence.AverageOrNone().Match(none: true, some: False)
            : sequence.Average() == sequence.AverageOrNone();

    private static bool CompareAverageAndHandleEmptyInt64Sequence(IReadOnlyCollection<long> sequence)
        => sequence.Count == 0
            ? sequence.AverageOrNone().Match(none: true, some: False)
            : sequence.Average() == sequence.AverageOrNone();

    private static bool CompareAverageAndHandleEmptySingleSequence(IReadOnlyCollection<float> sequence)
        => sequence.Count == 0
            ? sequence.AverageOrNone().Match(none: true, some: False)
            : sequence.Average() == sequence.AverageOrNone();

    private static bool CompareAverageAndHandleEmptyDoubleSequence(IReadOnlyCollection<double> sequence)
        => sequence.Count == 0
            ? sequence.AverageOrNone().Match(none: true, some: False)
            : sequence.Average() == sequence.AverageOrNone();

    private static bool CompareAverageAndHandleEmptyDecimalSequence(IReadOnlyCollection<decimal> sequence)
        => sequence.Count == 0
            ? sequence.AverageOrNone().Match(none: true, some: False)
            : sequence.Average() == sequence.AverageOrNone();
}
