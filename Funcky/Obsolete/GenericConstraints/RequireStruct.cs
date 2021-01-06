using System;

namespace Funcky.GenericConstraints
{
    [Obsolete("Create these types in your own project, if you need them")]
    public sealed class RequireStruct<T>
        where T : struct
    {
    }
}
