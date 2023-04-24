namespace Funcky.Internal;

internal static class ValueMapper
{
    private const int NotFoundValue = -1;

    [Pure]
    public static Option<int> MapNotFoundToNone(int index)
        => index is NotFoundValue
            ? Option<int>.None
            : index;
}
