#pragma warning disable CA1715, SA1302 // lang-ext naming: F, M, T, A, B are the established spelling for this pattern
namespace Funcky.Spike.Traits;

public interface Functor<F> : Kind<F>
    where F : Functor<F>
{
    static abstract K<F, B> Map<A, B>(K<F, A> fa, Func<A, B> f)
        where A : notnull
        where B : notnull;
}
