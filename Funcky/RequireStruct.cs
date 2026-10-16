using System.ComponentModel;
using static System.ComponentModel.EditorBrowsableState;

namespace Funcky;

/// <summary>Marker type that lets two overloads differ by a <c>struct</c> constraint on their type parameter, as an optional parameter that is never passed. Not meant to be instantiated.</summary>
[EditorBrowsable(Never)]
public sealed class RequireStruct<T>
    where T : struct;
