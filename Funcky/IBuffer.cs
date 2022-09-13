namespace Funcky;

/// <summary>
/// Represents a buffer of an underlying <see cref="IEnumerable{T}"/> resource and is <see cref="IDisposable"/> accordingly.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public interface IBuffer<out T> : IEnumerable<T>, IDisposable
{
}
