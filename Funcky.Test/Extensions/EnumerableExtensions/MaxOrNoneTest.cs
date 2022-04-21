using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.Internal;
using Funcky.Test.Internal.Data;

namespace Funcky.Test.Extensions.EnumerableExtensions;

public sealed class MaxOrNoneTest
{
    // Int32/int Tests
    [Property]
    public Property MaxOrNoneGivesTheSameResultAsMaxForInt32(List<int> sequence)
        => CompareMaxAndHandleEmptyInt32Sequence(sequence).ToProperty();

    [Property]
    public Property MaxOrNoneGivesTheSameResultAsMaxForNullableInt32(List<int?> sequence)
        => (Option.FromNullable(sequence.Max())
            == sequence.Select(Option.FromNullable).MaxOrNone()).ToProperty();

    [Property]
    public Property MaxOrNoneWithSelectorGivesTheSameResultAsMaxForNullableInt32(List<int?> sequence, Func<int?, int?> selector)
        => (Option.FromNullable(sequence.Max(selector))
            == sequence.Select(Option.FromNullable).MaxOrNone(SelectorTransformation.TransformNullableSelector(selector))).ToProperty();

    // Int64/long Tests
    [Property]
    public Property MaxOrNoneGivesTheSameResultAsMaxForInt64(List<long> sequence)
        => CompareMaxAndHandleEmptyInt64Sequence(sequence).ToProperty();

    [Property]
    public Property MaxOrNoneGivesTheSameResultAsMaxForNullableInt64(List<long?> sequence)
        => (Option.FromNullable(sequence.Max())
            == sequence.Select(Option.FromNullable).MaxOrNone()).ToProperty();

    [Property]
    public Property MaxOrNoneWithSelectorGivesTheSameResultAsMaxForNullableInt64(List<long?> sequence, Func<long?, long?> selector)
        => (Option.FromNullable(sequence.Max(selector))
            == sequence.Select(Option.FromNullable).MaxOrNone(SelectorTransformation.TransformNullableSelector(selector))).ToProperty();

    // Single/float Tests
    [Property]
    public Property MaxOrNoneGivesTheSameResultAsMaxForSingle(List<float> sequence)
        => CompareMaxAndHandleEmptySingleSequence(sequence).ToProperty();

    [Property]
    public Property MaxOrNoneGivesTheSameResultAsMaxForNullableSingle(List<float?> sequence)
        => (Option.FromNullable(sequence.Max())
            == sequence.Select(Option.FromNullable).MaxOrNone()).ToProperty();

    [Property]
    public Property MaxOrNoneWithSelectorGivesTheSameResultAsMaxForNullableSingle(List<float?> sequence, Func<float?, float?> selector)
        => (Option.FromNullable(sequence.Max(selector))
            == sequence.Select(Option.FromNullable).MaxOrNone(SelectorTransformation.TransformNullableSelector(selector))).ToProperty();

    // Double/double Tests
    [Property]
    public Property MaxOrNoneGivesTheSameResultAsMaxForDouble(List<double> sequence)
        => CompareMaxAndHandleEmptyDoubleSequence(sequence).ToProperty();

    [Property]
    public Property MaxOrNoneGivesTheSameResultAsMaxForNullableDouble(List<double?> sequence)
        => (Option.FromNullable(sequence.Max())
            == sequence.Select(Option.FromNullable).MaxOrNone()).ToProperty();

    [Property]
    public Property MaxOrNoneWithSelectorGivesTheSameResultAsMaxForNullableDouble(List<double?> sequence, Func<double?, double?> selector)
        => (Option.FromNullable(sequence.Max(selector))
            == sequence.Select(Option.FromNullable).MaxOrNone(SelectorTransformation.TransformNullableSelector(selector))).ToProperty();

    // Decimal/decimal Tests
    [Property]
    public Property MaxOrNoneGivesTheSameResultAsMaxForDecimal(List<decimal> sequence)
        => CompareMaxAndHandleEmptyDecimalSequence(sequence).ToProperty();

    [Property]
    public Property MaxOrNoneGivesTheSameResultAsMaxForNullableDecimal(List<decimal?> sequence)
        => (Option.FromNullable(sequence.Max())
            == sequence.Select(Option.FromNullable).MaxOrNone()).ToProperty();

    [Property]
    public Property MaxOrNoneWithSelectorGivesTheSameResultAsMaxForNullableDecimal(List<decimal?> sequence, Func<decimal?, decimal?> selector)
        => (Option.FromNullable(sequence.Max(selector))
            == sequence.Select(Option.FromNullable).MaxOrNone(SelectorTransformation.TransformNullableSelector(selector))).ToProperty();

    // Generic TSource implementing IComparable Tests
    [Property]
    public Property MaxOrNoneGivesTheSameResultAsMaxForAnyIComparable(List<int> sequence)
        => CompareMaxAndHandleEmptyPersonSequence(sequence.Select(Person.Create).ToList()).ToProperty();

    [Property]
    public Property MaxOrNoneGivesTheSameResultAsMaxForAnyNullableIComparable(List<int?> sequence)
        => (Option.FromNullable(sequence.Select(Person.Create).Max())
            == sequence.Select(Person.Create).Select(Option.FromNullable).MaxOrNone()).ToProperty();

    [Property]
    public Property MaxOrNoneWithSelectorGivesTheSameResultAsMaxForAnyIComparable(List<int?> sequence, Func<int?, int?> selector)
        => (Option.FromNullable(sequence.Select(Person.Create).Max(SelectorTransformation.TransformPersonSelector(selector)))
            == sequence.Select(Person.Create).Select(Option.FromNullable).MaxOrNone(SelectorTransformation.TransformOptionPersonSelector(selector))).ToProperty();

    private static bool CompareMaxAndHandleEmptyInt32Sequence(IReadOnlyCollection<int> sequence)
        => sequence.Count == 0
            ? sequence.MaxOrNone().Match(none: true, some: _ => false)
            : sequence.Max() == sequence.MaxOrNone();

    private static bool CompareMaxAndHandleEmptyInt64Sequence(IReadOnlyCollection<long> sequence)
        => sequence.Count == 0
            ? sequence.MaxOrNone().Match(none: true, some: _ => false)
            : sequence.Max() == sequence.MaxOrNone();

    private static bool CompareMaxAndHandleEmptySingleSequence(IReadOnlyCollection<float> sequence)
        => sequence.Count == 0
            ? sequence.MaxOrNone().Match(none: true, some: _ => false)
            : sequence.Max() == sequence.MaxOrNone();

    private static bool CompareMaxAndHandleEmptyDoubleSequence(IReadOnlyCollection<double> sequence)
        => sequence.Count == 0
            ? sequence.MaxOrNone().Match(none: true, some: _ => false)
            : sequence.Max() == sequence.MaxOrNone();

    private static bool CompareMaxAndHandleEmptyDecimalSequence(IReadOnlyCollection<decimal> sequence)
        => sequence.Count == 0
            ? sequence.MaxOrNone().Match(none: true, some: _ => false)
            : sequence.Max() == sequence.MaxOrNone();

    private static bool CompareMaxAndHandleEmptyPersonSequence(IReadOnlyCollection<Person> sequence)
        => sequence.Count == 0
            ? sequence.MaxOrNone().Match(none: true, some: _ => false)
            : sequence.MaxOrNone().Match(none: false, some: p => p.CompareTo(sequence.Max()) == 0);
}
