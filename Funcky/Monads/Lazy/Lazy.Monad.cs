using System.Diagnostics.CodeAnalysis;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;

namespace Funcky.Monads;

public static partial class LazyExtensions
{
    extension<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T>(Lazy<T> lazy)
    {
        [Pure]
        public Lazy<TResult> Select<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] TResult>(Func<T, TResult> selector)
            => new(new LazySelect<T, TResult>(lazy, selector).Apply);

        [Pure]
        public Lazy<TResult> SelectMany<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] TResult>(Func<T, Lazy<TResult>> selector)
            => lazy.SelectMany(selector, static (_, b) => b);

        [Pure]
        public Lazy<TResult> SelectMany<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] TLazy, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TResult>(Func<T, Lazy<TLazy>> selector, Func<T, TLazy, TResult> resultSelector)
            => new(new LazySelectMany<T, TLazy, TResult>(lazy, selector, resultSelector).Apply);
    }

    // These classes are needed because the closure class the compiler generates for a lambda has no
    // DynamicallyAccessedMembers annotations on its type parameters, which the trimmer reports as IL2091.
    private sealed class LazySelect<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TResult>(Lazy<T> source, Func<T, TResult> selector)
    {
        public TResult Apply() => selector(source.Value);
    }

    private sealed class LazySelectMany<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TA, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TResult>(Lazy<T> lazy, Func<T, Lazy<TA>> selector, Func<T, TA, TResult> resultSelector)
    {
        public TResult Apply()
        {
            var first = lazy.Value;
            var second = selector(first).Value;
            return resultSelector(first, second);
        }
    }
}
