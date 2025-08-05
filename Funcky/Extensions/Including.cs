namespace Funcky.Extensions;

public class Including : IIntervalBoundary
{
    private Including(int number)
    {
        Value = number;
    }

    public int Value { get; }

    public static implicit operator Including(int number) => new(number);
}
