using System.ComponentModel;
using static System.ComponentModel.EditorBrowsableState;

namespace Funcky;

[EditorBrowsable(Never)]
public sealed record RequireStruct<T>
    where T : struct;
