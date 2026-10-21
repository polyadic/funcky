using System.Diagnostics.CodeAnalysis;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Funcky.Monads;

public static partial class LazyExtensions
{
    extension<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>(Lazy<Lazy<T>> lazy)
    {
        [Pure]
        public Lazy<T> Flatten()
            => Lazy.FromFunc(() => lazy.Value.Value);
    }
}
