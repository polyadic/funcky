using System.Diagnostics.CodeAnalysis;

namespace Funcky.GenericConstraints
{
    [SuppressMessage("ApiDesign", "RS0041", Justification = "This is just a helper type")]
    [Obsolete("Create these types in your own project, if you need them")]
    public sealed class RequireClass<T>
        where T : class
    {
    }
}
