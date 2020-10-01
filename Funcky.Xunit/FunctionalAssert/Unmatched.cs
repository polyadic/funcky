using System.Runtime.CompilerServices;

namespace Funcky.Xunit
{
    public static partial class FunctionalAssert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Unmatched()
            => throw new UnmatchedException();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Unmatched(string unmatchedCase)
            => throw new UnmatchedException(unmatchedCase);
    }
}
