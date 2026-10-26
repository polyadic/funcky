using System.Reflection;

namespace Funcky.Extensions;

public static class AssemblyExtensions
{
    extension(Assembly assembly)
    {
        /// <summary>The entry point of the assembly, or <c>None</c> for an assembly without one, such as a class library,
        /// where <see cref="Assembly.EntryPoint"/> is <see langword="null"/>.</summary>
        public Option<MethodInfo> EntryPointOrNone
            => Option.FromNullable(assembly.EntryPoint);
    }
}
