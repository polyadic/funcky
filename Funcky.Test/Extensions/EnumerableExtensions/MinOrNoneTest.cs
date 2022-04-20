using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.Internal;
using Funcky.Test.Internal.Data;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class MinOrNoneTest
{
    // Int32/int Tests
    [Property]
    public Property MinOrNoneGivesTheSameResultAsMinForInt32(List<int> sequence)
        => CompareMinAndHandleEmptyInt32Sequence(sequence).ToProperty();

    [Property]
    public Property MinOrNoneGivesTheSameResultAsMinForNullableInt32(List<int?> sequence)
        => (Option.FromNullable(sequence.Min())
            == sequence.Select(Option.FromNullable).MinOrNone()).ToProperty();

    [Property]
    public Property MinOrNoneWithSelectorGivesTheSameResultAsMinForNullableInt32(List<int?> sequence, Func<int?, int?> selector)
        => (Option.FromNullable(sequence.Min(selector))
            == sequence.Select(Option.FromNullable)
                .MinOrNone(SelectorTransformation.TransformNullableSelector(selector))).ToProperty();

    // Int64/long Tests
    [Property]
    public Property MinOrNoneGivesTheSameResultAsMinForInt64(List<long> sequence)
        => CompareMinAndHandleEmptyInt64Sequence(sequence).ToProperty();

    [Property]
    public Property MinOrNoneGivesTheSameResultAsMinForNullableInt64(List<long?> sequence)
        => (Option.FromNullable(sequence.Min())
            == sequence.Select(Option.FromNullable).MinOrNone()).ToProperty();

    [Property]
    public Property MinOrNoneWithSelectorGivesTheSameResultAsMinForNullableInt64(List<long?> sequence, Func<long?, long?> selector)
        => (Option.FromNullable(sequence.Min(selector))
            == sequence.Select(Option.FromNullable)
                .MinOrNone(SelectorTransformation.TransformNullableSelector(selector))).ToProperty();

    // Single/float Tests
    [Property]
    public Property MinOrNoneGivesTheSameResultAsMinForSingle(List<float> sequence)
        => CompareMinAndHandleEmptySingleSequence(sequence).ToProperty();

    [Property]
    public Property MinOrNoneGivesTheSameResultAsMinForNullableSingle(List<float?> sequence)
        => (Option.FromNullable(sequence.Min())
            == sequence.Select(Option.FromNullable).MinOrNone()).ToProperty();

    [Property]
    public Property MinOrNoneWithSelectorGivesTheSameResultAsMinForNullableSingle(List<float?> sequence, Func<float?, float?> selector)
        => (Option.FromNullable(sequence.Min(selector))
            == sequence.Select(Option.FromNullable)
                .MinOrNone(SelectorTransformation.TransformNullableSelector(selector))).ToProperty();

    // Double/double Tests
    [Property]
    public Property MinOrNoneGivesTheSameResultAsMinForDouble(List<double> sequence)
        => CompareMinAndHandleEmptyDoubleSequence(sequence).ToProperty();

    [Property]
    public Property MinOrNoneGivesTheSameResultAsMinForNullableDouble(List<double?> sequence)
        => (Option.FromNullable(sequence.Min())
            == sequence.Select(Option.FromNullable).MinOrNone()).ToProperty();

    [Property]
    public Property MinOrNoneWithSelectorGivesTheSameResultAsMinForNullableDouble(List<double?> sequence, Func<double?, double?> selector)
        => (Option.FromNullable(sequence.Min(selector))
            == sequence.Select(Option.FromNullable)
                .MinOrNone(SelectorTransformation.TransformNullableSelector(selector))).ToProperty();

    // Decimal/decimal Tests
    [Property]
    public Property MinOrNoneGivesTheSameResultAsMinForDecimal(List<decimal> sequence)
        => CompareMinAndHandleEmptyDecimalSequence(sequence).ToProperty();

    [Property]
    public Property MinOrNoneGivesTheSameResultAsMinForNullableDecimal(List<decimal?> sequence)
        => (Option.FromNullable(sequence.Min())
            == sequence.Select(Option.FromNullable).MinOrNone()).ToProperty();

    [Property]
    public Property MinOrNoneWithSelectorGivesTheSameResultAsMinForNullableDecimal(List<decimal?> sequence, Func<decimal?, decimal?> selector)
        => (Option.FromNullable(sequence.Min(selector))
            == sequence.Select(Option.FromNullable)
                .MinOrNone(SelectorTransformation.TransformNullableSelector(selector))).ToProperty();

    // Generic TSource implementing IComparable Tests
    [Property]
    public Property MinOrNoneGivesTheSameResultAsMinForAnyIComparable(List<int> sequence)
        => CompareMinAndHandleEmptyPersonSequence(sequence.Select(Person.Create).ToList()).ToProperty();

    [Property]
    public Property MinOrNoneGivesTheSameResultAsMinForAnyNullableIComparable(List<int?> sequence)
        => (Option.FromNullable(sequence.Select(Person.Create).Min())
            == sequence.Select(Person.Create).Select(Option.FromNullable).MinOrNone()).ToProperty();

    [Property]
    public Property MinOrNoneWithSelectorGivesTheSameResultAsMinForAnyIComparable(List<int?> sequence, Func<int?, int?> selector)
        => (Option.FromNullable(sequence.Select(Person.Create)
                .Min(SelectorTransformation.TransformPersonSelector(selector)))
            == sequence.Select(Person.Create).Select(Option.FromNullable)
                .MinOrNone(SelectorTransformation.TransformOptionPersonSelector(selector))).ToProperty();

    [Fact]
    public void Failing()
    {
        var sequence = new List<int?> { -1, -1, 1 };

        var min = Option.FromNullable(sequence.Select(Person.Create).Min());
        var minOrNone = sequence.Select(Person.Create).Select(Option.FromNullable).MinOrNone();

        Assert.True(min == minOrNone);
        Assert.Equal(0, min.CompareTo(minOrNone));
        Assert.Equal(min, minOrNone);
    }

    [Fact]
    public void Confused()
    {
        Person personA = new(42);
        Person personB = new(42);

        Assert.Equal(personA, personB);
        Assert.Equal(Option.FromNullable(personA), Option.FromNullable(personB));
    }

    private static bool CompareMinAndHandleEmptyInt32Sequence(IReadOnlyCollection<int> sequence)
        => sequence.Count == 0
            ? sequence.MinOrNone().Match(none: true, some: _ => false)
            : sequence.Min() == sequence.MinOrNone();

    private static bool CompareMinAndHandleEmptyInt64Sequence(IReadOnlyCollection<long> sequence)
        => sequence.Count == 0
            ? sequence.MinOrNone().Match(none: true, some: _ => false)
            : sequence.Min() == sequence.MinOrNone();

    private static bool CompareMinAndHandleEmptySingleSequence(IReadOnlyCollection<float> sequence)
        => sequence.Count == 0
            ? sequence.MinOrNone().Match(none: true, some: _ => false)
            : sequence.Min() == sequence.MinOrNone();

    private static bool CompareMinAndHandleEmptyDoubleSequence(IReadOnlyCollection<double> sequence)
        => sequence.Count == 0
            ? sequence.MinOrNone().Match(none: true, some: _ => false)
            : sequence.Min() == sequence.MinOrNone();

    private static bool CompareMinAndHandleEmptyDecimalSequence(IReadOnlyCollection<decimal> sequence)
        => sequence.Count == 0
            ? sequence.MinOrNone().Match(none: true, some: _ => false)
            : sequence.Min() == sequence.MinOrNone();

    private static bool CompareMinAndHandleEmptyPersonSequence(IReadOnlyCollection<Person> sequence)
        => sequence.Count == 0
            ? sequence.MinOrNone().Match(none: true, some: _ => false)
            : sequence.MinOrNone().Match(none: false, some: p => p.CompareTo(sequence.Min()) == 0);
}
