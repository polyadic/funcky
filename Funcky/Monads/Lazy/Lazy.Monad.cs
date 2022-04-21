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
        => SelectMany(lazy, selector, static (a, b) => b);

    [Pure]
    public static Lazy<TResult> SelectMany<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TA, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TResult>(this Lazy<T> lazy, Func<T, Lazy<TA>> selector, Func<T, TA, TResult> resultSelector)
        => new(new LazySelectMany<T, TA, TResult>(lazy, selector, resultSelector).Apply);

    // This class is needed because the implicitly generated class that would be generated for a lambda
    // wouldn't have generic types annotated with DynamicallyAccessedMembers, which would result in a warning.
    private sealed class LazySelect<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TResult>
    {
        private readonly Lazy<T> _lazy;
        private readonly Func<T, TResult> _selector;

        public LazySelect(Lazy<T> source, Func<T, TResult> selector) => (_lazy, _selector) = (source, selector);

        public TResult Apply() => _selector(_lazy.Value);
    }

    // This class is needed because the implicitly generated class that would be generated for a lambda
    // wouldn't have generic types annotated with DynamicallyAccessedMembers, which would result in a warning.
    private sealed class LazySelectMany<[DynamicallyAccessedMembers(PublicParameterlessConstructor)] T, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TA, [DynamicallyAccessedMembers(PublicParameterlessConstructor)] TResult>
    {
        private readonly Lazy<T> _lazy;
        private readonly Func<T, Lazy<TA>> _selector;
        private readonly Func<T, TA, TResult> _resultSelector;

        public LazySelectMany(Lazy<T> lazy, Func<T, Lazy<TA>> selector, Func<T, TA, TResult> resultSelector) => (_lazy, _selector, _resultSelector) = (lazy, selector, resultSelector);

        public TResult Apply()
        {
            var first = _lazy.Value;
            var second = _selector(first).Value;
            return _resultSelector(first, second);
        }
    }
}
