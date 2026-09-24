// ReSharper disable PossibleMultipleEnumeration
using FsCheck;
using FsCheck.Fluent;
using Funcky.Test.Internal;
using Funcky.Test.Internal.Data;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class MaxOrNoneTest
{
    // Int32/int Tests
    [FunckyAsyncProperty]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxAsyncForInt32(IAsyncEnumerable<int> sequence)
        => CompareMaxAndHandleEmptyInt32Sequence(sequence).Result.ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence)
        => (Option.FromNullable(sequence.MaxAsync().Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync().Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, Func<int?, int?> selector)
        => (Option.FromNullable(sequence.MaxAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, AwaitSelector<int?> selector)
        => (Option.FromNullable(sequence.MaxAwaitAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, AwaitSelectorWithCancellation<int?> selector)
        => (Option.FromNullable(sequence.MaxAwaitWithCancellationAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    // Int64/long Tests
    [FunckyAsyncProperty]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxAsyncForInt64(IAsyncEnumerable<long> sequence)
        => CompareMaxAndHandleEmptyInt64Sequence(sequence).Result.ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence)
        => (Option.FromNullable(sequence.MaxAsync().Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync().Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence, Func<long?, long?> selector)
        => (Option.FromNullable(sequence.MaxAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence, AwaitSelector<long?> selector)
        => (Option.FromNullable(sequence.MaxAwaitAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence, AwaitSelectorWithCancellation<long?> selector)
        => (Option.FromNullable(sequence.MaxAwaitWithCancellationAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    // Single/float Tests
    [FunckyAsyncProperty]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxAsyncForSingle(IAsyncEnumerable<float> sequence)
        => CompareMaxAndHandleEmptySingleSequence(sequence).Result.ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence)
        => (Option.FromNullable(sequence.MaxAsync().Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync().Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, Func<float?, float?> selector)
        => (Option.FromNullable(sequence.MaxAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, AwaitSelector<float?> selector)
        => (Option.FromNullable(sequence.MaxAwaitAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, AwaitSelectorWithCancellation<float?> selector)
        => (Option.FromNullable(sequence.MaxAwaitWithCancellationAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    // Double/double Tests
    [FunckyAsyncProperty]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxAsyncForDouble(IAsyncEnumerable<double> sequence)
        => CompareMaxAndHandleEmptyDoubleSequence(sequence).Result.ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence)
        => (Option.FromNullable(sequence.MaxAsync().Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync().Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, Func<double?, double?> selector)
        => (Option.FromNullable(sequence.MaxAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, AwaitSelector<double?> selector)
        => (Option.FromNullable(sequence.MaxAwaitAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, AwaitSelectorWithCancellation<double?> selector)
        => (Option.FromNullable(sequence.MaxAwaitWithCancellationAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    // Decimal/decimal Tests
    [FunckyAsyncProperty]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxAsyncForDecimal(IAsyncEnumerable<decimal> sequence)
        => CompareMaxAndHandleEmptyDecimalSequence(sequence).Result.ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence)
        => (Option.FromNullable(sequence.MaxAsync().Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync().Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, Func<decimal?, decimal?> selector)
        => (Option.FromNullable(sequence.MaxAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, AwaitSelector<decimal?> selector)
        => (Option.FromNullable(sequence.MaxAwaitAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, AwaitSelectorWithCancellation<decimal?> selector)
        => (Option.FromNullable(sequence.MaxAwaitWithCancellationAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    // Generic TSource implementing IComparable Tests
    [FunckyAsyncProperty]
    public Property MaxOrNoneGivesTheSameResultAsMaxForAnyIComparable(IAsyncEnumerable<int> sequence)
        => CompareMaxAndHandleEmptyPersonSequence(sequence.Select(Person.Create)).Result.ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxForNullableAsyncForAnyIComparable(IAsyncEnumerable<int?> sequence)
        => (Option.FromNullable(sequence.Select(Person.Create).MaxAsync().Result)
            == sequence.Select(Person.Create).Select(Option.FromNullable).MaxOrNoneAsync().Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForAnyIComparable(IAsyncEnumerable<int?> sequence, Func<int?, int?> selector)
        => (Option.FromNullable(sequence.Select(Person.Create).MaxAsync(SelectorTransformation.TransformPersonSelector(selector)).Result)
            == sequence.Select(Person.Create).Select(Option.FromNullable).MaxOrNoneAsync(SelectorTransformation.TransformOptionPersonSelector(selector)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForAnyIComparable(IAsyncEnumerable<int?> sequence, AwaitSelector<int?> selector)
        => (Option.FromNullable(sequence.Select(Person.Create).MaxAwaitAsync(SelectorTransformation.TransformPersonSelector(selector.Get)).Result)
            == sequence.Select(Person.Create).Select(Option.FromNullable).MaxOrNoneAwaitAsync(SelectorTransformation.TransformOptionPersonSelector(selector.Get)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MaxAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForAnyIComparable(IAsyncEnumerable<int?> sequence, AwaitSelectorWithCancellation<int?> selector)
        => (Option.FromNullable(sequence.Select(Person.Create).MaxAwaitWithCancellationAsync(SelectorTransformation.TransformPersonSelector(selector.Get)).Result)
            == sequence.Select(Person.Create).Select(Option.FromNullable).MaxOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformOptionPersonSelector(selector.Get)).Result).ToProperty();

    [Fact]
    public void Failing()
    {
        var sequence = new List<int?> { -1, -1, 1 };

        var min = Option.FromNullable(sequence.Select(Person.Create).Max());
        var minOrNone = sequence.Select(Person.Create).Select(Option.FromNullable).MaxOrNone();

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

    private static async Task<bool> CompareMaxAndHandleEmptyInt32Sequence(IAsyncEnumerable<int> sequence)
        => await sequence.AnyAsync()
            ? await sequence.MaxAsync() == await sequence.MaxOrNoneAsync()
            : (await sequence.MaxOrNoneAsync()).Match(none: true, some: _ => false);

    private static async Task<bool> CompareMaxAndHandleEmptyInt64Sequence(IAsyncEnumerable<long> sequence)
        => await sequence.AnyAsync()
            ? await sequence.MaxAsync() == await sequence.MaxOrNoneAsync()
            : (await sequence.MaxOrNoneAsync()).Match(none: true, some: _ => false);

    private static async Task<bool> CompareMaxAndHandleEmptySingleSequence(IAsyncEnumerable<float> sequence)
        => await sequence.AnyAsync()
            ? await sequence.MaxAsync() == await sequence.MaxOrNoneAsync()
            : (await sequence.MaxOrNoneAsync()).Match(none: true, some: _ => false);

    private static async Task<bool> CompareMaxAndHandleEmptyDoubleSequence(IAsyncEnumerable<double> sequence)
        => await sequence.AnyAsync()
            ? await sequence.MaxAsync() == await sequence.MaxOrNoneAsync()
            : (await sequence.MaxOrNoneAsync()).Match(none: true, some: _ => false);

    private static async Task<bool> CompareMaxAndHandleEmptyDecimalSequence(IAsyncEnumerable<decimal> sequence)
        => await sequence.AnyAsync()
            ? await sequence.MaxAsync() == await sequence.MaxOrNoneAsync()
            : (await sequence.MaxOrNoneAsync()).Match(none: true, some: _ => false);

    private static async Task<bool> CompareMaxAndHandleEmptyPersonSequence(IAsyncEnumerable<Person> sequence)
        => await sequence.AnyAsync()
            ? (await sequence.MaxOrNoneAsync()).Match(none: false, some: p => p.CompareTo(sequence.MaxAsync().Result) == 0)
            : (await sequence.MaxOrNoneAsync()).Match(none: true, some: _ => false);
}
