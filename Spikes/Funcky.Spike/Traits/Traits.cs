#pragma warning disable CA1715, SA1302 // lang-ext naming: F, M, T, A, B are the established spelling for this pattern
namespace Funcky.Spike.Traits;

public interface Functor<F> : Kind<F>
    where F : Functor<F>
{
    static abstract K<F, B> Map<A, B>(K<F, A> fa, Func<A, B> f)
        where A : notnull
        where B : notnull;
}

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

public interface Monad<M> : Applicative<M>
    where M : Monad<M>
{
    static abstract K<M, B> Bind<A, B>(K<M, A> ma, Func<A, K<M, B>> f)
        where A : notnull
        where B : notnull;
}

/// <summary>A container that can be walked with an effect. One implementation per container replaces the
/// container × effect matrix of hand-written Traverse/Sequence overloads (483 lines in Funcky today, #1036).</summary>
public interface Traversable<T> : Functor<T>
    where T : Traversable<T>
{
    static abstract K<F, K<T, B>> Traverse<F, A, B>(K<T, A> ta, Func<A, K<F, B>> f)
        where F : Applicative<F>
        where A : notnull
        where B : notnull;
}

/// <summary>Empty and OrElse for the monads that have a failure case (Option, Either, Result).</summary>
public interface Alternative<F> : Applicative<F>
    where F : Alternative<F>
{
    static abstract K<F, A> Empty<A>()
        where A : notnull;

    static abstract K<F, A> OrElse<A>(K<F, A> fa, Func<K<F, A>> fallback)
        where A : notnull;
}
