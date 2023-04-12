namespace Funcky.Test.TestUtils;

internal class EverythingIsEqual<T> : IEqualityComparer<T>
{
    public bool Equals(T? x, T? y) => true;

    public int GetHashCode(T obj) => 0;
}
