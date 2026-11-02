#pragma warning disable CA1715, SA1302 // lang-ext naming: F, M, T, A, B are the established spelling for this pattern
namespace Funcky.Spike.Traits;

/// <summary>Empty and OrElse for the monads that have a failure case (Option, Either, Result).</summary>
public interface Alternative<F> : Applicative<F>
    where F : Alternative<F>
{
    static abstract K<F, A> Empty<A>()
        where A : notnull;

    static abstract K<F, A> OrElse<A>(K<F, A> fa, Func<K<F, A>> fallback)
        where A : notnull;
}
