using System.Diagnostics.Contracts;

namespace Funcky
{
    public static partial class Functional
    {
        [Pure]
        public static bool True<T>(T dummy) => true;

        [Pure]
        public static bool False<T>(T dummy) => false;
    }
}
