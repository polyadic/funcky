using System.Diagnostics.Contracts;

namespace Funcky
{
    public static partial class Functional
    {
        [Pure]
        public static bool True<T>(T ω) => true;

        [Pure]
        public static bool False<T>(T ω) => false;
    }
}
