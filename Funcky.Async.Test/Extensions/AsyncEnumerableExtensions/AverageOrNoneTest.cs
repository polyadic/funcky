// ReSharper disable PossibleMultipleEnumeration
using FsCheck;
using FsCheck.Xunit;
using Funcky.Async.Extensions;
using Funcky.Monads;
using Funcky.Test.Internal;

namespace Funcky.Async.Test.Extensions.AsyncEnumerableExtensions;

public sealed class AverageOrNoneTest
{
    public AverageOrNoneTest()
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
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageAsyncForInt32(IAsyncEnumerable<int> sequence)
        => CompareAverageAndHandleEmptyInt32SequenceAsync(sequence).Result.ToProperty();

    [Property]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence)
        => (Option.FromNullable(sequence.AverageAsync().Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync().Result).ToProperty();

    [Property]
    public Property AverageOrNoneAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, Func<int?, int?> selector)
        => (Option.FromNullable(sequence.AverageAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property AverageOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, Func<int?, ValueTask<int?>> selector)
        => (Option.FromNullable(sequence.AverageAwaitAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property AverageAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, Func<int?, CancellationToken, ValueTask<int?>> selector)
        => (Option.FromNullable(sequence.AverageAwaitWithCancellationAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    // Int64/long Tests
    [Property]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageAsyncForInt64(IAsyncEnumerable<long> sequence)
        => CompareAverageAndHandleEmptyInt64SequenceAsync(sequence).Result.ToProperty();

    [Property]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence)
        => (Option.FromNullable(sequence.AverageAsync().Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync().Result).ToProperty();

    [Property]
    public Property AverageOrNoneAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence, Func<long?, long?> selector)
        => (Option.FromNullable(sequence.AverageAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property AverageOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence, Func<long?, ValueTask<long?>> selector)
        => (Option.FromNullable(sequence.AverageAwaitAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property AverageAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForInt64(IAsyncEnumerable<int?> sequence, Func<int?, CancellationToken, ValueTask<int?>> selector)
        => (Option.FromNullable(sequence.AverageAwaitWithCancellationAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    // Single/float Tests
    [Property]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageAsyncForSingle(IAsyncEnumerable<float> sequence)
        => CompareAverageAndHandleEmptySingleSequenceAsync(sequence).Result.ToProperty();

    [Property]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence)
        => (Option.FromNullable(sequence.AverageAsync().Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync().Result).ToProperty();

    [Property]
    public Property AverageOrNoneAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, Func<float?, float?> selector)
        => (Option.FromNullable(sequence.AverageAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property AverageOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, Func<float?, ValueTask<float?>> selector)
        => (Option.FromNullable(sequence.AverageAwaitAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property AverageAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, Func<float?, CancellationToken, ValueTask<float?>> selector)
        => (Option.FromNullable(sequence.AverageAwaitWithCancellationAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    // Double/double Tests
    [Property]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageAsyncForDouble(IAsyncEnumerable<double> sequence)
        => CompareAverageAndHandleEmptyDoubleSequenceAsync(sequence).Result.ToProperty();

    [Property]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence)
        => (Option.FromNullable(sequence.AverageAsync().Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync().Result).ToProperty();

    [Property]
    public Property AverageOrNoneAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, Func<double?, double?> selector)
        => (Option.FromNullable(sequence.AverageAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property AverageOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, Func<double?, ValueTask<double?>> selector)
        => (Option.FromNullable(sequence.AverageAwaitAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property AverageAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, Func<double?, CancellationToken, ValueTask<double?>> selector)
        => (Option.FromNullable(sequence.AverageAwaitWithCancellationAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    // Decimal/decimal Tests
    [Property]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageAsyncForDecimal(IAsyncEnumerable<decimal> sequence)
        => CompareAverageAndHandleEmptyDecimalSequenceAsync(sequence).Result.ToProperty();

    [Property]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence)
        => (Option.FromNullable(sequence.AverageAsync().Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync().Result).ToProperty();

    [Property]
    public Property AverageOrNoneAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, Func<decimal?, decimal?> selector)
        => (Option.FromNullable(sequence.AverageAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property AverageOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, Func<decimal?, ValueTask<decimal?>> selector)
        => (Option.FromNullable(sequence.AverageAwaitAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [Property]
    public Property AverageAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, Func<decimal?, CancellationToken, ValueTask<decimal?>> selector)
        => (Option.FromNullable(sequence.AverageAwaitWithCancellationAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    private static async Task<bool> CompareAverageAndHandleEmptyInt32SequenceAsync(IAsyncEnumerable<int> sequence)
        => await sequence.AnyAsync()
            ? await sequence.AverageAsync() == await sequence.AverageOrNoneAsync()
            : (await sequence.AverageOrNoneAsync()).Match(none: true, some: _ => false);

    private static async Task<bool> CompareAverageAndHandleEmptyInt64SequenceAsync(IAsyncEnumerable<long> sequence)
        => await sequence.AnyAsync()
            ? await sequence.AverageAsync() == await sequence.AverageOrNoneAsync()
            : (await sequence.AverageOrNoneAsync()).Match(none: true, some: _ => false);

    private static async Task<bool> CompareAverageAndHandleEmptySingleSequenceAsync(IAsyncEnumerable<float> sequence)
        => await sequence.AnyAsync()
            ? await sequence.AverageAsync() == await sequence.AverageOrNoneAsync()
            : (await sequence.AverageOrNoneAsync()).Match(none: true, some: _ => false);

    private static async Task<bool> CompareAverageAndHandleEmptyDoubleSequenceAsync(IAsyncEnumerable<double> sequence)
        => await sequence.AnyAsync()
            ? await sequence.AverageAsync() == await sequence.AverageOrNoneAsync()
            : (await sequence.AverageOrNoneAsync()).Match(none: true, some: _ => false);

    private static async Task<bool> CompareAverageAndHandleEmptyDecimalSequenceAsync(IAsyncEnumerable<decimal> sequence)
        => await sequence.AnyAsync()
            ? await sequence.AverageAsync() == await sequence.AverageOrNoneAsync()
            : (await sequence.AverageOrNoneAsync()).Match(none: true, some: _ => false);
}
