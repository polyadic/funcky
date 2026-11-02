#pragma warning disable CA1715, SA1302 // lang-ext naming: F, M, T, A, B are the established spelling for this pattern
namespace Funcky.Spike.Traits;

public interface Monad<M> : Applicative<M>
    where M : Monad<M>
{
    static abstract K<M, B> Bind<A, B>(K<M, A> ma, Func<A, K<M, B>> f)
        where A : notnull
        where B : notnull;
}
