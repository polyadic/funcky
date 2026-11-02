#pragma warning disable CA1715, SA1302 // lang-ext naming
using Funcky.Monads;

namespace Funcky.Spike.Traits;

/// <summary>Today's struct <c>Option&lt;T&gt;</c> as a kind, through a wrapper struct. In a real implementation the struct
/// would implement <c>K&lt;OptionKind, A&gt;</c> itself; the box happens either way when the value is passed as
/// <c>K</c>. This is the cost #862 has to accept for a struct-based library.</summary>
public readonly record struct OptionK<A>(Option<A> Option) : K<OptionKind, A>
    where A : notnull;

public sealed class OptionKind : Monad<OptionKind>, Traversable<OptionKind>, Alternative<OptionKind>
{
    public static K<OptionKind, A> Pure<A>(A value)
        where A : notnull
        => new OptionK<A>(Option.Some(value));

    public static K<OptionKind, B> Map<A, B>(K<OptionKind, A> fa, Func<A, B> f)
        where A : notnull
        where B : notnull
        => new OptionK<B>(As(fa).Select(f));

    public static K<OptionKind, C> Map2<A, B, C>(K<OptionKind, A> fa, K<OptionKind, B> fb, Func<A, B, C> f)
        where A : notnull
        where B : notnull
        where C : notnull
        => new OptionK<C>(As(fa).SelectMany(a => As(fb).Select(b => f(a, b))));

    public static K<OptionKind, B> Bind<A, B>(K<OptionKind, A> ma, Func<A, K<OptionKind, B>> f)
        where A : notnull
        where B : notnull
        => new OptionK<B>(As(ma).SelectMany(a => As(f(a))));

    public static K<F, K<OptionKind, B>> Traverse<F, A, B>(K<OptionKind, A> ta, Func<A, K<F, B>> f)
        where F : Applicative<F>
        where A : notnull
        where B : notnull
        => As(ta).Match(
            none: () => F.Pure(Empty<B>()),
            some: a => F.Map(f(a), Pure));

    public static K<OptionKind, A> Empty<A>()
        where A : notnull
        => new OptionK<A>(Option<A>.None);

    public static K<OptionKind, A> OrElse<A>(K<OptionKind, A> fa, Func<K<OptionKind, A>> fallback)
        where A : notnull
        => As(fa).Match(none: fallback, some: _ => fa);

    public static Option<A> As<A>(K<OptionKind, A> k)
        where A : notnull
        => ((OptionK<A>)k).Option;
}
