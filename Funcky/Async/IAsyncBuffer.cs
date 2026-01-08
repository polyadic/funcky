namespace Funcky;

/// <summary>
/// Represents a buffer of an underlying <see cref="IEnumerable{TItem}"/> resource and is <see cref="IDisposable"/> accordingly.
/// </summary>
/// <typeparam name="T">Element type.</typeparam>
public interface IAsyncBuffer<out T> : IAsyncEnumerable<T>, IAsyncDisposable;
