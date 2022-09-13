using System.Diagnostics.CodeAnalysis;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Funcky.Monads;

public static class Lazy
{
    [Pure]
    public static Lazy<T> FromFunc<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>(Func<T> valueFactory)
        => new(valueFactory);

#if LAZY_RETURN_CONSTRUCTOR
    [Pure]
    public static Lazy<T> Return<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>(T value)
        => new(value);
#else
    [Pure]
    public static Lazy<T> Return<T>(T value)
        => new(() => value);
#endif
}
