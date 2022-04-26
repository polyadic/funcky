namespace Funcky;

/// <summary>
/// Represents a buffer of an underlying <see cref="IEnumerable{TItem}"/> resource and is <see cref="IDisposable"/> accordingly.
/// </summary>
/// <typeparam name="TSource">Element type.</typeparam>
public interface IBuffer<out TSource> : IEnumerable<TSource>, IDisposable
{
}
