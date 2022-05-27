using System.Runtime.CompilerServices;

namespace Funcky;

public static partial class Functional
{
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Identity<T>(T value) => value;
}
