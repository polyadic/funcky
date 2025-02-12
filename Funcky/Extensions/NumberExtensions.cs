namespace Funcky.Extensions;

public static class NumberExtensions
{
    public static bool IsBetween<TFrom, TTo>(this int number, TFrom from, TTo to)
        where TFrom : IIntervalBoundary
        where TTo : IIntervalBoundary
        => from.Value < to.Value
            ? IsBetweenForward(number, from, to)
            : IsBetweenBackward(number, from, to);

    private static bool IsBetweenForward<TFrom, TTo>(int number, TFrom from, TTo to)
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

    private static bool IsBetweenBackward<TFrom, TTo>(int number, TFrom from, TTo to)
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
