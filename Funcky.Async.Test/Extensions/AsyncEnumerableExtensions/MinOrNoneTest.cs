// ReSharper disable PossibleMultipleEnumeration
using FsCheck;
using FsCheck.Fluent;
using Funcky.Test.Internal;
using Funcky.Test.Internal.Data;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class MinOrNoneTest
{
    // Int32/int Tests
    [FunckyAsyncProperty]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinAsyncForInt32(IAsyncEnumerable<int> sequence)
        => CompareMinAndHandleEmptyInt32Sequence(sequence).Result.ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence)
        => (Option.FromNullable(sequence.MinAsync().Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync().Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, Func<int?, int?> selector)
        => (Option.FromNullable(sequence.MinAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, AwaitSelector<int?> selector)
        => (Option.FromNullable(sequence.MinAwaitAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, AwaitSelectorWithCancellation<int?> selector)
        => (Option.FromNullable(sequence.MinAwaitWithCancellationAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    // Int64/long Tests
    [FunckyAsyncProperty]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinAsyncForInt64(IAsyncEnumerable<long> sequence)
        => CompareMinAndHandleEmptyInt64Sequence(sequence).Result.ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence)
        => (Option.FromNullable(sequence.MinAsync().Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync().Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence, Func<long?, long?> selector)
        => (Option.FromNullable(sequence.MinAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence, AwaitSelector<long?> selector)
        => (Option.FromNullable(sequence.MinAwaitAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence, AwaitSelectorWithCancellation<long?> selector)
        => (Option.FromNullable(sequence.MinAwaitWithCancellationAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    // Single/float Tests
    [FunckyAsyncProperty]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinAsyncForSingle(IAsyncEnumerable<float> sequence)
        => CompareMinAndHandleEmptySingleSequence(sequence).Result.ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence)
        => (Option.FromNullable(sequence.MinAsync().Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync().Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, Func<float?, float?> selector)
        => (Option.FromNullable(sequence.MinAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, AwaitSelector<float?> selector)
        => (Option.FromNullable(sequence.MinAwaitAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, AwaitSelectorWithCancellation<float?> selector)
        => (Option.FromNullable(sequence.MinAwaitWithCancellationAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    // Double/double Tests
    [FunckyAsyncProperty]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinAsyncForDouble(IAsyncEnumerable<double> sequence)
        => CompareMinAndHandleEmptyDoubleSequence(sequence).Result.ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence)
        => (Option.FromNullable(sequence.MinAsync().Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync().Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, Func<double?, double?> selector)
        => (Option.FromNullable(sequence.MinAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, AwaitSelector<double?> selector)
        => (Option.FromNullable(sequence.MinAwaitAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, AwaitSelectorWithCancellation<double?> selector)
        => (Option.FromNullable(sequence.MinAwaitWithCancellationAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    // Decimal/decimal Tests
    [FunckyAsyncProperty]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinAsyncForDecimal(IAsyncEnumerable<decimal> sequence)
        => CompareMinAndHandleEmptyDecimalSequence(sequence).Result.ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence)
        => (Option.FromNullable(sequence.MinAsync().Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync().Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, Func<decimal?, decimal?> selector)
        => (Option.FromNullable(sequence.MinAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, AwaitSelector<decimal?> selector)
        => (Option.FromNullable(sequence.MinAwaitAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, AwaitSelectorWithCancellation<decimal?> selector)
        => (Option.FromNullable(sequence.MinAwaitWithCancellationAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    // Generic TSource implementing IComparable Tests
    [FunckyAsyncProperty]
    public Property MinOrNoneGivesTheSameResultAsMinForAnyIComparable(IAsyncEnumerable<int> sequence)
        => CompareMinAndHandleEmptyPersonSequence(sequence.Select(Person.Create)).Result.ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinForNullableAsyncForAnyIComparable(IAsyncEnumerable<int?> sequence)
        => (Option.FromNullable(sequence.Select(Person.Create).MinAsync().Result)
            == sequence.Select(Person.Create).Select(Option.FromNullable).MinOrNoneAsync().Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForAnyIComparable(IAsyncEnumerable<int?> sequence, Func<int?, int?> selector)
        => (Option.FromNullable(sequence.Select(Person.Create).MinAsync(SelectorTransformation.TransformPersonSelector(selector)).Result)
            == sequence.Select(Person.Create).Select(Option.FromNullable).MinOrNoneAsync(SelectorTransformation.TransformOptionPersonSelector(selector)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForAnyIComparable(IAsyncEnumerable<int?> sequence, AwaitSelector<int?> selector)
        => (Option.FromNullable(sequence.Select(Person.Create).MinAwaitAsync(SelectorTransformation.TransformPersonSelector(selector.Get)).Result)
            == sequence.Select(Person.Create).Select(Option.FromNullable).MinOrNoneAwaitAsync(SelectorTransformation.TransformOptionPersonSelector(selector.Get)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property MinAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForAnyIComparable(IAsyncEnumerable<int?> sequence, AwaitSelectorWithCancellation<int?> selector)
        => (Option.FromNullable(sequence.Select(Person.Create).MinAwaitWithCancellationAsync(SelectorTransformation.TransformPersonSelector(selector.Get)).Result)
            == sequence.Select(Person.Create).Select(Option.FromNullable).MinOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformOptionPersonSelector(selector.Get)).Result).ToProperty();

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

    private static async Task<bool> CompareMinAndHandleEmptyInt32Sequence(IAsyncEnumerable<int> sequence)
        => await sequence.AnyAsync()
            ? await sequence.MinAsync() == await sequence.MinOrNoneAsync()
            : (await sequence.MinOrNoneAsync()).Match(none: true, some: _ => false);

    private static async Task<bool> CompareMinAndHandleEmptyInt64Sequence(IAsyncEnumerable<long> sequence)
        => await sequence.AnyAsync()
            ? await sequence.MinAsync() == await sequence.MinOrNoneAsync()
            : (await sequence.MinOrNoneAsync()).Match(none: true, some: _ => false);

    private static async Task<bool> CompareMinAndHandleEmptySingleSequence(IAsyncEnumerable<float> sequence)
        => await sequence.AnyAsync()
            ? await sequence.MinAsync() == await sequence.MinOrNoneAsync()
            : (await sequence.MinOrNoneAsync()).Match(none: true, some: _ => false);

    private static async Task<bool> CompareMinAndHandleEmptyDoubleSequence(IAsyncEnumerable<double> sequence)
        => await sequence.AnyAsync()
            ? await sequence.MinAsync() == await sequence.MinOrNoneAsync()
            : (await sequence.MinOrNoneAsync()).Match(none: true, some: _ => false);

    private static async Task<bool> CompareMinAndHandleEmptyDecimalSequence(IAsyncEnumerable<decimal> sequence)
        => await sequence.AnyAsync()
            ? await sequence.MinAsync() == await sequence.MinOrNoneAsync()
            : (await sequence.MinOrNoneAsync()).Match(none: true, some: _ => false);

    private static async Task<bool> CompareMinAndHandleEmptyPersonSequence(IAsyncEnumerable<Person> sequence)
        => await sequence.AnyAsync()
            ? (await sequence.MinOrNoneAsync()).Match(none: false, some: p => p.CompareTo(sequence.MinAsync().Result) == 0)
            : (await sequence.MinOrNoneAsync()).Match(none: true, some: _ => false);
}
