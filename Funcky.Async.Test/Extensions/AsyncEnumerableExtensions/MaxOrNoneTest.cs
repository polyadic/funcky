// ReSharper disable PossibleMultipleEnumeration
using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.Internal;
using Funcky.Test.Internal.Data;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class MaxOrNoneTest
{
    public MaxOrNoneTest()
    {
        Arb.Register<AsyncGenerator<int?>>();
        Arb.Register<AsyncGenerator<int>>();
        Arb.Register<AsyncGenerator<long?>>();
        Arb.Register<AsyncGenerator<long>>();
        Arb.Register<AsyncGenerator<float?>>();
        Arb.Register<AsyncGenerator<float>>();
        Arb.Register<AsyncGenerator<double?>>();
        Arb.Register<AsyncGenerator<double>>();
        Arb.Register<AsyncGenerator<decimal?>>();
        Arb.Register<AsyncGenerator<decimal>>();
    }

    // Int32/int Tests
    [Property]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxAsyncForInt32(IAsyncEnumerable<int> sequence)
        => CompareMaxAndHandleEmptyInt32Sequence(sequence).Result.ToProperty();

    [Property]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence)
        => (Option.FromNullable(sequence.MaxAsync().Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync().Result).ToProperty();

    [Property]
    public Property MaxOrNoneAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, Func<int?, int?> selector)
        => (Option.FromNullable(sequence.MaxAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MaxOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, Func<int?, ValueTask<int?>> selector)
        => (Option.FromNullable(sequence.MaxAwaitAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MaxAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, Func<int?, CancellationToken, ValueTask<int?>> selector)
        => (Option.FromNullable(sequence.MaxAwaitWithCancellationAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    // Int64/long Tests
    [Property]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxAsyncForInt64(IAsyncEnumerable<long> sequence)
        => CompareMaxAndHandleEmptyInt64Sequence(sequence).Result.ToProperty();

    [Property]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence)
        => (Option.FromNullable(sequence.MaxAsync().Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync().Result).ToProperty();

    [Property]
    public Property MaxOrNoneAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence, Func<long?, long?> selector)
        => (Option.FromNullable(sequence.MaxAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MaxOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence, Func<long?, ValueTask<long?>> selector)
        => (Option.FromNullable(sequence.MaxAwaitAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MaxAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence, Func<long?, CancellationToken, ValueTask<long?>> selector)
        => (Option.FromNullable(sequence.MaxAwaitWithCancellationAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    // Single/float Tests
    [Property]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxAsyncForSingle(IAsyncEnumerable<float> sequence)
        => CompareMaxAndHandleEmptySingleSequence(sequence).Result.ToProperty();

    [Property]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence)
        => (Option.FromNullable(sequence.MaxAsync().Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync().Result).ToProperty();

    [Property]
    public Property MaxOrNoneAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, Func<float?, float?> selector)
        => (Option.FromNullable(sequence.MaxAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MaxOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, Func<float?, ValueTask<float?>> selector)
        => (Option.FromNullable(sequence.MaxAwaitAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MaxAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, Func<float?, CancellationToken, ValueTask<float?>> selector)
        => (Option.FromNullable(sequence.MaxAwaitWithCancellationAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    // Double/double Tests
    [Property]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxAsyncForDouble(IAsyncEnumerable<double> sequence)
        => CompareMaxAndHandleEmptyDoubleSequence(sequence).Result.ToProperty();

    [Property]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence)
        => (Option.FromNullable(sequence.MaxAsync().Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync().Result).ToProperty();

    [Property]
    public Property MaxOrNoneAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, Func<double?, double?> selector)
        => (Option.FromNullable(sequence.MaxAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MaxOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, Func<double?, ValueTask<double?>> selector)
        => (Option.FromNullable(sequence.MaxAwaitAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MaxAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, Func<double?, CancellationToken, ValueTask<double?>> selector)
        => (Option.FromNullable(sequence.MaxAwaitWithCancellationAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    // Decimal/decimal Tests
    [Property]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxAsyncForDecimal(IAsyncEnumerable<decimal> sequence)
        => CompareMaxAndHandleEmptyDecimalSequence(sequence).Result.ToProperty();

    [Property]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence)
        => (Option.FromNullable(sequence.MaxAsync().Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync().Result).ToProperty();

    [Property]
    public Property MaxOrNoneAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, Func<decimal?, decimal?> selector)
        => (Option.FromNullable(sequence.MaxAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MaxOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, Func<decimal?, ValueTask<decimal?>> selector)
        => (Option.FromNullable(sequence.MaxAwaitAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MaxAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, Func<decimal?, CancellationToken, ValueTask<decimal?>> selector)
        => (Option.FromNullable(sequence.MaxAwaitWithCancellationAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MaxOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    // Generic TSource implementing IComparable Tests
    [Property]
    public Property MaxOrNoneGivesTheSameResultAsMaxForAnyIComparable(IAsyncEnumerable<int> sequence)
        => CompareMaxAndHandleEmptyPersonSequence(sequence.Select(Person.Create)).Result.ToProperty();

    [Property]
    public Property MaxOrNoneAsyncGivesTheSameResultAsMaxForNullableAsyncForAnyIComparable(IAsyncEnumerable<int?> sequence)
        => (Option.FromNullable(sequence.Select(Person.Create).MaxAsync().Result)
            == sequence.Select(Person.Create).Select(Option.FromNullable).MaxOrNoneAsync().Result).ToProperty();

    [Property]
    public Property MaxOrNoneAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForAnyIComparable(IAsyncEnumerable<int?> sequence, Func<int?, int?> selector)
        => (Option.FromNullable(sequence.Select(Person.Create).MaxAsync(SelectorTransformation.TransformPersonSelector(selector)).Result)
            == sequence.Select(Person.Create).Select(Option.FromNullable).MaxOrNoneAsync(SelectorTransformation.TransformOptionPersonSelector(selector)).Result).ToProperty();

    [Property]
    public Property MaxOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForAnyIComparable(IAsyncEnumerable<int?> sequence, Func<int?, ValueTask<int?>> selector)
        => (Option.FromNullable(sequence.Select(Person.Create).MaxAwaitAsync(SelectorTransformation.TransformPersonSelector(selector)).Result)
            == sequence.Select(Person.Create).Select(Option.FromNullable).MaxOrNoneAwaitAsync(SelectorTransformation.TransformOptionPersonSelector(selector)).Result).ToProperty();

    [Property]
    public Property MaxAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMaxForNullableAsyncForAnyIComparable(IAsyncEnumerable<int?> sequence, Func<int?, CancellationToken, ValueTask<int?>> selector)
        => (Option.FromNullable(sequence.Select(Person.Create).MaxAwaitWithCancellationAsync(SelectorTransformation.TransformPersonSelector(selector)).Result)
            == sequence.Select(Person.Create).Select(Option.FromNullable).MaxOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformOptionPersonSelector(selector)).Result).ToProperty();

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
