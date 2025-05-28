using System.Diagnostics.CodeAnalysis;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Funcky.Monads;

public static partial class LazyExtensions
{
    public static Lazy<T> Flatten<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>(this Lazy<Lazy<T>> lazy)
        => lazy.SelectMany(Identity);
}
