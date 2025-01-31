using System.ComponentModel;

namespace Funcky.Monads;

/// <summary>Marker interface implemented for <see cref="Option{TItem}"/>.</summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface IOption
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal void InternalImplementationOnly();
}
