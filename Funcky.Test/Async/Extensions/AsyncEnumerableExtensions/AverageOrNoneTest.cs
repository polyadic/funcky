#if !NET48

// ReSharper disable PossibleMultipleEnumeration
using FsCheck;
using FsCheck.Fluent;
using Funcky.Test.Internal;

namespace Funcky.Test.Async.Extensions.AsyncEnumerableExtensions;

public sealed class AverageOrNoneTest
{
    // Int32/int Tests
    [FunckyAsyncProperty]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageAsyncForInt32(IAsyncEnumerable<int> sequence)
        => CompareAverageAndHandleEmptyInt32SequenceAsync(sequence).Result.ToProperty();

    [FunckyAsyncProperty]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence)
        => (Option.FromNullable(sequence.AverageAsync().Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync().Result).ToProperty();

    [FunckyAsyncProperty]
    public Property AverageOrNoneAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, Func<int?, int?> selector)
        => (Option.FromNullable(sequence.AverageAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property AverageOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, AwaitSelector<int?> selector)
        => (Option.FromNullable(sequence.AverageAwaitAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property AverageAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForInt32(IAsyncEnumerable<int?> sequence, AwaitSelectorWithCancellation<int?> selector)
        => (Option.FromNullable(sequence.AverageAwaitWithCancellationAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    // Int64/long Tests
    [FunckyAsyncProperty]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageAsyncForInt64(IAsyncEnumerable<long> sequence)
        => CompareAverageAndHandleEmptyInt64SequenceAsync(sequence).Result.ToProperty();

    [FunckyAsyncProperty]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence)
        => (Option.FromNullable(sequence.AverageAsync().Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync().Result).ToProperty();

    [FunckyAsyncProperty]
    public Property AverageOrNoneAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence, Func<long?, long?> selector)
        => (Option.FromNullable(sequence.AverageAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property AverageOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForInt64(IAsyncEnumerable<long?> sequence, AwaitSelector<long?> selector)
        => (Option.FromNullable(sequence.AverageAwaitAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property AverageAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForInt64(IAsyncEnumerable<int?> sequence, AwaitSelectorWithCancellation<int?> selector)
        => (Option.FromNullable(sequence.AverageAwaitWithCancellationAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    // Single/float Tests
    [FunckyAsyncProperty]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageAsyncForSingle(IAsyncEnumerable<float> sequence)
        => CompareAverageAndHandleEmptySingleSequenceAsync(sequence).Result.ToProperty();

    [FunckyAsyncProperty]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence)
        => (Option.FromNullable(sequence.AverageAsync().Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync().Result).ToProperty();

    [FunckyAsyncProperty]
    public Property AverageOrNoneAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, Func<float?, float?> selector)
        => (Option.FromNullable(sequence.AverageAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property AverageOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, AwaitSelector<float?> selector)
        => (Option.FromNullable(sequence.AverageAwaitAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property AverageAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForSingle(IAsyncEnumerable<float?> sequence, AwaitSelectorWithCancellation<float?> selector)
        => (Option.FromNullable(sequence.AverageAwaitWithCancellationAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    // Double/double Tests
    [FunckyAsyncProperty]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageAsyncForDouble(IAsyncEnumerable<double> sequence)
        => CompareAverageAndHandleEmptyDoubleSequenceAsync(sequence).Result.ToProperty();

    [FunckyAsyncProperty]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence)
        => (Option.FromNullable(sequence.AverageAsync().Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync().Result).ToProperty();

    [FunckyAsyncProperty]
    public Property AverageOrNoneAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, Func<double?, double?> selector)
        => (Option.FromNullable(sequence.AverageAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property AverageOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, AwaitSelector<double?> selector)
        => (Option.FromNullable(sequence.AverageAwaitAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property AverageAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForDouble(IAsyncEnumerable<double?> sequence, AwaitSelectorWithCancellation<double?> selector)
        => (Option.FromNullable(sequence.AverageAwaitWithCancellationAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    // Decimal/decimal Tests
    [FunckyAsyncProperty]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageAsyncForDecimal(IAsyncEnumerable<decimal> sequence)
        => CompareAverageAndHandleEmptyDecimalSequenceAsync(sequence).Result.ToProperty();

    [FunckyAsyncProperty]
    public Property AverageOrNoneAsyncGivesTheSameResultAsAverageForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence)
        => (Option.FromNullable(sequence.AverageAsync().Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync().Result).ToProperty();

    [FunckyAsyncProperty]
    public Property AverageOrNoneAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, Func<decimal?, decimal?> selector)
        => (Option.FromNullable(sequence.AverageAsync(selector).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAsync(SelectorTransformation.TransformNullableSelector(selector)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property AverageOrNoneAwaitAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, AwaitSelector<decimal?> selector)
        => (Option.FromNullable(sequence.AverageAwaitAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

    [FunckyAsyncProperty]
    public Property AverageAwaitWithCancellationAsyncWithSelectorGivesTheSameResultAsAverageForNullableAsyncForDecimal(IAsyncEnumerable<decimal?> sequence, AwaitSelectorWithCancellation<decimal?> selector)
        => (Option.FromNullable(sequence.AverageAwaitWithCancellationAsync(selector.Get).Result)
            == sequence.Select(Option.FromNullable).AverageOrNoneAwaitWithCancellationAsync(SelectorTransformation.TransformNullableSelector(selector.Get)).Result).ToProperty();

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

#endif
