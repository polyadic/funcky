using System.Diagnostics.CodeAnalysis;

namespace Funcky;

[SuppressMessage("ApiDesign", "RS0041", Justification = "This is just a helper type")]
public sealed record RequireClass<T>
    where T : class;
