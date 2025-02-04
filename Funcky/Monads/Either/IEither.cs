using System.ComponentModel;

namespace Funcky.Monads;

/// <summary>Marker interface implemented for <see cref="Either{TLeft,TRight}"/>.</summary>
[EditorBrowsable(EditorBrowsableState.Advanced)]
public interface IEither
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal void InternalImplementationOnly();
}
