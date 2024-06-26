using System.ComponentModel;
using static System.ComponentModel.EditorBrowsableState;

namespace Funcky;

[EditorBrowsable(Never)]
public sealed class RequireClass<T>
    where T : class;
