namespace Funcky
{
    public static partial class Functional
    {
        [Pure]
        public static T Identity<T>(T value) => value;
    }
}
