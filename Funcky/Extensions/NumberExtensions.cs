namespace Funcky.Extensions;

public static class NumberExtensions
{
    public static bool InRange<TFrom, TTo>(this int number, TFrom from, TTo to)
        where TFrom : IIntervalBoundary
        where TTo : IIntervalBoundary
        => InRange(from, to)
            .Invoke(number, from, to);

    public static Func<int, TFrom, TTo, bool> InRange<TFrom, TTo>(TFrom from, TTo to)
        where TFrom : IIntervalBoundary
        where TTo : IIntervalBoundary
        => from.Value < to.Value
            ? InRangeForward
            : InRangeBackward;

    private static bool InRangeForward<TFrom, TTo>(int number, TFrom from, TTo to)
        where TFrom : IIntervalBoundary
        where TTo : IIntervalBoundary
        => (from, to) switch
        {
            (Including, Including) => from.Value <= number && number <= to.Value,
            (Including, Excluding) => from.Value <= number && number < to.Value,
            (Excluding, Including) => from.Value < number && number <= to.Value,
            (Excluding, Excluding) => from.Value < number && number < to.Value,
            _ => false,
        };

    private static bool InRangeBackward<TFrom, TTo>(int number, TFrom from, TTo to)
        where TFrom : IIntervalBoundary
        where TTo : IIntervalBoundary
        => (from, to) switch
        {
            (Including, Including) => from.Value >= number && number >= to.Value,
            (Including, Excluding) => from.Value >= number && number > to.Value,
            (Excluding, Including) => from.Value > number && number >= to.Value,
            (Excluding, Excluding) => from.Value > number && number > to.Value,
            _ => false,
        };
}
