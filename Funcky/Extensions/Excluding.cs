namespace Funcky.Extensions;

public class Excluding : IIntervalBoundary
{
    private Excluding(int number)
    {
        Value = number;
    }

    public int Value { get; }

    public static implicit operator Excluding(int number) => new(number);
}
