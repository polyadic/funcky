using System.Diagnostics.CodeAnalysis;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Funcky.Monads;

public static class Lazy
{
    [Pure]
    public static Lazy<TItem> FromFunc<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] TItem>(Func<TItem> valueFactory)
        => new(valueFactory);

#if LAZY_RETURN_CONSTRUCTOR
    [Pure]
    public static Lazy<TItem> Return<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] TItem>(TItem value)
        => new(value);
#else
    [Pure]
    public static Lazy<TItem> Return<TItem>(TItem value)
        => new(() => value);
#endif
}
