using System.ComponentModel;
using static System.ComponentModel.EditorBrowsableState;

namespace Funcky;

[EditorBrowsable(Never)]
public sealed class RequireStruct<T>
    where T : struct
{
}
