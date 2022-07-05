// ReSharper disable PossibleMultipleEnumeration
using FsCheck;
using FsCheck.Xunit;
using Funcky.Test.Internal;
using Funcky.Test.Internal.Data;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class MinOrNoneTest
{
    public MinOrNoneTest()
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
    public Property MinOrNoneAsyncGivesTheSameResultAsMinAsyncForInt32(IAsyncEnumerable<int> sequence)
        => CompareMinAndHandleEmptyInt32Sequence(sequence).Result.ToProperty();

    [Property]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence)
        => (Option.FromNullable(sequence.MinAsync().Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync().Result).ToProperty();

    [Property]
    public Property MinOrNoneAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, Func<int?, int?> selector)
        => (Option.FromNullable(sequence.MinAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MinOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, Func<int?, ValueTask<int?>> selector)
        => (Option.FromNullable(sequence.MinAwaitAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MinAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, Func<int?, CancellationToken, ValueTask<int?>> selector)
        => (Option.FromNullable(sequence.MinAwaitWithCancellationAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    // Int64/long Tests
    [Property]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinAsyncForInt64(IAsyncEnumerable<long> sequence)
        => CompareMinAndHandleEmptyInt64Sequence(sequence).Result.ToProperty();

    [Property]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence)
        => (Option.FromNullable(sequence.MinAsync().Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync().Result).ToProperty();

    [Property]
    public Property MinOrNoneAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence, Func<long?, long?> selector)
        => (Option.FromNullable(sequence.MinAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MinOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence, Func<long?, ValueTask<long?>> selector)
        => (Option.FromNullable(sequence.MinAwaitAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MinAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence, Func<long?, CancellationToken, ValueTask<long?>> selector)
        => (Option.FromNullable(sequence.MinAwaitWithCancellationAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    // Single/float Tests
    [Property]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinAsyncForSingle(IAsyncEnumerable<float> sequence)
        => CompareMinAndHandleEmptySingleSequence(sequence).Result.ToProperty();

    [Property]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence)
        => (Option.FromNullable(sequence.MinAsync().Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync().Result).ToProperty();

    [Property]
    public Property MinOrNoneAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, Func<float?, float?> selector)
        => (Option.FromNullable(sequence.MinAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MinOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, Func<float?, ValueTask<float?>> selector)
        => (Option.FromNullable(sequence.MinAwaitAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MinAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, Func<float?, CancellationToken, ValueTask<float?>> selector)
        => (Option.FromNullable(sequence.MinAwaitWithCancellationAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    // Double/double Tests
    [Property]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinAsyncForDouble(IAsyncEnumerable<double> sequence)
        => CompareMinAndHandleEmptyDoubleSequence(sequence).Result.ToProperty();

    [Property]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence)
        => (Option.FromNullable(sequence.MinAsync().Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync().Result).ToProperty();

    [Property]
    public Property MinOrNoneAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, Func<double?, double?> selector)
        => (Option.FromNullable(sequence.MinAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MinOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, Func<double?, ValueTask<double?>> selector)
        => (Option.FromNullable(sequence.MinAwaitAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MinAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, Func<double?, CancellationToken, ValueTask<double?>> selector)
        => (Option.FromNullable(sequence.MinAwaitWithCancellationAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    // Decimal/decimal Tests
    [Property]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinAsyncForDecimal(IAsyncEnumerable<decimal> sequence)
        => CompareMinAndHandleEmptyDecimalSequence(sequence).Result.ToProperty();

    [Property]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence)
        => (Option.FromNullable(sequence.MinAsync().Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync().Result).ToProperty();

    [Property]
    public Property MinOrNoneAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, Func<decimal?, decimal?> selector)
        => (Option.FromNullable(sequence.MinAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MinOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, Func<decimal?, ValueTask<decimal?>> selector)
        => (Option.FromNullable(sequence.MinAwaitAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property MinAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, Func<decimal?, CancellationToken, ValueTask<decimal?>> selector)
        => (Option.FromNullable(sequence.MinAwaitWithCancellationAsync(selector).Result)
            == sequence.Select(Option.FromNullable).MinOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    // Generic TSource implementing IComparable Tests
    [Property]
    public Property MinOrNoneGivesTheSameResultAsMinForAnyIComparable(IAsyncEnumerable<int> sequence)
        => CompareMinAndHandleEmptyPersonSequence(sequence.Select(Person.Create)).Result.ToProperty();

    [Property]
    public Property MinOrNoneAsyncGivesTheSameResultAsMinForNullableAsyncForAnyIComparable(IAsyncEnumerable<int?> sequence)
        => (Option.FromNullable(sequence.Select(Person.Create).MinAsync().Result)
            == sequence.Select(Person.Create).Select(Option.FromNullable).MinOrNoneAsync().Result).ToProperty();

    [Property]
    public Property MinOrNoneAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForAnyIComparable(IAsyncEnumerable<int?> sequence, Func<int?, int?> selector)
        => (Option.FromNullable(sequence.Select(Person.Create).MinAsync(SelectorTransformation.TransformPersonSelector(selector)).Result)
            == sequence.Select(Person.Create).Select(Option.FromNullable).MinOrNoneAsync(SelectorTransformation.TransformOptionPersonSelector(selector)).Result).ToProperty();

    [Property]
    public Property MinOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForAnyIComparable(IAsyncEnumerable<int?> sequence, Func<int?, ValueTask<int?>> selector)
        => (Option.FromNullable(sequence.Select(Person.Create).MinAwaitAsync(SelectorTransformation.TransformPersonSelector(selector)).Result)
            == sequence.Select(Person.Create).Select(Option.FromNullable).MinOrNoneAwaitAsync(SelectorTransformation.TransformOptionPersonSelector(selector)).Result).ToProperty();

    [Property]
    public Property MinAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsMinForNullableAsyncForAnyIComparable(IAsyncEnumerable<int?> sequence, Func<int?, CancellationToken, ValueTask<int?>> selector)
        => (Option.FromNullable(sequence.Select(Person.Create).MinAwaitWithCancellationAsync(SelectorTransformation.TransformPersonSelector(selector)).Result)
            == sequence.Select(Person.Create).Select(Option.FromNullable).MinOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformOptionPersonSelector(selector)).Result).ToProperty();

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
