#pragma warning disable CA1715, SA1302 // lang-ext naming
using Funcky.Spike.Unions;

namespace Funcky.Spike.Traits;

/// <summary>The union Option as a kind. A union may list interfaces, but it is a struct: passing it as <c>K</c> boxes it.
/// The spike wraps it in a struct that implements <c>K</c>, so every trait call boxes the wrapper on entry and unboxes
/// with a cast on exit, on top of the union's own boxing of its case.</summary>
public readonly record struct UnionOptionK<A>(UnionOption<A> Option) : K<UnionOptionKind, A>
    where A : notnull;

public sealed class UnionOptionKind : Monad<UnionOptionKind>, Traversable<UnionOptionKind>, Alternative<UnionOptionKind>
{
    public static K<UnionOptionKind, A> Pure<A>(A value)
        where A : notnull
        => new UnionOptionK<A>(UnionOption.Some(value));

    public static K<UnionOptionKind, B> Map<A, B>(K<UnionOptionKind, A> fa, Func<A, B> f)
        where A : notnull
        where B : notnull
        => new UnionOptionK<B>(As(fa).Select(f));

    public static K<UnionOptionKind, C> Map2<A, B, C>(K<UnionOptionKind, A> fa, K<UnionOptionKind, B> fb, Func<A, B, C> f)
        where A : notnull
        where B : notnull
        where C : notnull
        => new UnionOptionK<C>(As(fa).SelectMany(a => As(fb).Select(b => f(a, b))));

    public static K<UnionOptionKind, B> Bind<A, B>(K<UnionOptionKind, A> ma, Func<A, K<UnionOptionKind, B>> f)
        where A : notnull
        where B : notnull
        => new UnionOptionK<B>(As(ma).SelectMany(a => As(f(a))));

    public static K<F, K<UnionOptionKind, B>> Traverse<F, A, B>(K<UnionOptionKind, A> ta, Func<A, K<F, B>> f)
        where F : Applicative<F>
        where A : notnull
        where B : notnull
        => As(ta).Match(
            none: () => F.Pure(Empty<B>()),
            some: a => F.Map(f(a), Pure));

    public static K<UnionOptionKind, A> Empty<A>()
        where A : notnull
        => new UnionOptionK<A>(UnionOption<A>.Empty);

    public static K<UnionOptionKind, A> OrElse<A>(K<UnionOptionKind, A> fa, Func<K<UnionOptionKind, A>> fallback)
        where A : notnull
        => As(fa).IsSome ? fa : fallback();

    public static UnionOption<A> As<A>(K<UnionOptionKind, A> k)
        where A : notnull
        => ((UnionOptionK<A>)k).Option;
}
