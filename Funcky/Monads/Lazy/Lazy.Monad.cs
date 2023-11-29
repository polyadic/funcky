using System.Diagnostics.CodeAnalysis;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Funcky.Monads;

public static partial class LazyExtensions
{
    [Pure]
    public static Lazy<TResult> Select<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TResult>(this Lazy<T> lazy, Func<T, TResult> selector)
        => new(new LazySelect<T, TResult>(lazy, selector).Apply);

    [Pure]
    public static Lazy<TResult> SelectMany<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TResult>(this Lazy<T> lazy, Func<T, Lazy<TResult>> selector)
        => SelectMany(lazy, selector, static (_, b) => b);

    [Pure]
    public static Lazy<TResult> SelectMany<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TLazy, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TResult>(this Lazy<T> lazy, Func<T, Lazy<TLazy>> selector, Func<T, TLazy, TResult> resultSelector)
        => new(new LazySelectMany<T, TLazy, TResult>(lazy, selector, resultSelector).Apply);

    // This class is needed because the implicitly generated class that would be generated for a lambda
    // wouldn't have generic types annotated with DynamicallyAccessedMembers, which would result in a warning.
    private sealed class LazySelect<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TResult>
        (Lazy<T> source, Func<T, TResult> selector)
    {
        public TResult Apply() => selector(source.Value);
    }

    // This class is needed because the implicitly generated class that would be generated for a lambda
    // wouldn't have generic types annotated with DynamicallyAccessedMembers, which would result in a warning.
    private sealed class LazySelectMany<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TA, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TResult>
        (Lazy<T> lazy, Func<T, Lazy<TA>> selector, Func<T, TA, TResult> resultSelector)
    {
        public TResult Apply()
        {
            var first = lazy.Value;
            var second = selector(first).Value;
            return resultSelector(first, second);
        }
    }
}
