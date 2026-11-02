#pragma warning disable CA1715, SA1302 // lang-ext naming: F, M, T, A, B are the established spelling for this pattern
namespace Funcky.Spike.Traits;

public interface Applicative<F> : Functor<F>
    where F : Applicative<F>
{
    static abstract K<F, A> Pure<A>(A value)
        where A : notnull;

    /// <summary>The binary lift. <c>Apply</c> (<c>K&lt;F, Func&lt;A, B&gt;&gt;</c>) is equivalent; Map2 avoids allocating curried delegates.</summary>
    static abstract K<F, C> Map2<A, B, C>(K<F, A> fa, K<F, B> fb, Func<A, B, C> f)
        where A : notnull
        where B : notnull
        where C : notnull;
}
