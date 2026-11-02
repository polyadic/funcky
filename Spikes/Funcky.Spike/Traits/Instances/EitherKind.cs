#pragma warning disable CA1715, SA1302 // lang-ext naming
using Funcky.Monads;

namespace Funcky.Spike.Traits;

/// <summary>Funcky's <c>Either&lt;L, R&gt;</c> as a kind in its right type: the left type is fixed in the marker
/// <c>EitherKind&lt;L&gt;</c>. This is how a two-parameter type joins a one-parameter trait, and the same trick gives
/// Validation (#802) an applicative that accumulates lefts.</summary>
public readonly record struct EitherK<L, A>(Either<L, A> Either) : K<EitherKind<L>, A>
    where L : notnull
    where A : notnull;

public sealed class EitherKind<L> : Monad<EitherKind<L>>, Traversable<EitherKind<L>>
    where L : notnull
{
    public static K<EitherKind<L>, A> Pure<A>(A value)
        where A : notnull
        => new EitherK<L, A>(Either<L>.Return(value));

    public static K<EitherKind<L>, B> Map<A, B>(K<EitherKind<L>, A> fa, Func<A, B> f)
        where A : notnull
        where B : notnull
        => new EitherK<L, B>(As(fa).Select(f));

    public static K<EitherKind<L>, C> Map2<A, B, C>(K<EitherKind<L>, A> fa, K<EitherKind<L>, B> fb, Func<A, B, C> f)
        where A : notnull
        where B : notnull
        where C : notnull
        => new EitherK<L, C>(As(fa).SelectMany(a => As(fb).Select(b => f(a, b))));

    public static K<EitherKind<L>, B> Bind<A, B>(K<EitherKind<L>, A> ma, Func<A, K<EitherKind<L>, B>> f)
        where A : notnull
        where B : notnull
        => new EitherK<L, B>(As(ma).SelectMany(a => As(f(a))));

    public static K<F, K<EitherKind<L>, B>> Traverse<F, A, B>(K<EitherKind<L>, A> ta, Func<A, K<F, B>> f)
        where F : Applicative<F>
        where A : notnull
        where B : notnull
        => As(ta).Match(
            left: left => F.Pure<K<EitherKind<L>, B>>(new EitherK<L, B>(Either<L, B>.Left(left))),
            right: a => F.Map(f(a), Pure));

    public static Either<L, A> As<A>(K<EitherKind<L>, A> k)
        where A : notnull
        => ((EitherK<L, A>)k).Either;
}
