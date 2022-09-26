using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using static System.ComponentModel.EditorBrowsableState;

namespace Funcky;

[SuppressMessage("ApiDesign", "RS0041", Justification = "This is just a helper type")]
[EditorBrowsable(Never)]
public sealed record RequireClass<T>
    where T : class;
