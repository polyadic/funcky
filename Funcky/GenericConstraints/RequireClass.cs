using System.Diagnostics.CodeAnalysis;

namespace Funcky.GenericConstraints
{
    [SuppressMessage("ApiDesign", "RS0041", Justification = "This is just a helper type")]
    public sealed class RequireClass<T>
        where T : class
    {
    }
}
