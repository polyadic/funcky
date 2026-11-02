#pragma warning disable CA1715, SA1302 // lang-ext naming
namespace Funcky.Spike.Traits;

/// <summary>Algorithms written once against the traits. Each of these replaces a family of overloads in Funcky.</summary>
public static class Generic
{
    /// <summary>Today: Traverse exists per (container, effect) pair. Here: one method.</summary>
    public static K<F, K<T, B>> Traverse<T, F, A, B>(this K<T, A> ta, Func<A, K<F, B>> f)
        where T : Traversable<T>
        where F : Applicative<F>
        where A : notnull
        where B : notnull
        => T.Traverse(ta, f);

    public static K<F, K<T, A>> Sequence<T, F, A>(this K<T, K<F, A>> ta)
        where T : Traversable<T>
        where F : Applicative<F>
        where A : notnull
        => T.Traverse(ta, static fa => fa);

    public static K<F, B> Select<F, A, B>(this K<F, A> fa, Func<A, B> f)
        where F : Functor<F>
        where A : notnull
        where B : notnull
        => F.Map(fa, f);

    public static K<M, B> SelectMany<M, A, B>(this K<M, A> ma, Func<A, K<M, B>> f)
        where M : Monad<M>
        where A : notnull
        where B : notnull
        => M.Bind(ma, f);

    public static K<M, C> SelectMany<M, A, B, C>(this K<M, A> ma, Func<A, K<M, B>> f, Func<A, B, C> g)
        where M : Monad<M>
        where A : notnull
        where B : notnull
        where C : notnull
        => M.Bind(ma, a => M.Map(f(a), b => g(a, b)));

    /// <summary>The applicative lift over arity 2; #802 (Validation) is one more instance of <see cref="Applicative{F}"/>,
    /// not one more copy of this method.</summary>
    public static K<F, C> Lift<F, A, B, C>(Func<A, B, C> f, K<F, A> fa, K<F, B> fb)
        where F : Applicative<F>
        where A : notnull
        where B : notnull
        where C : notnull
        => F.Map2(fa, fb, f);

    public static K<F, D> Lift<F, A, B, C, D>(Func<A, B, C, D> f, K<F, A> fa, K<F, B> fb, K<F, C> fc)
        where F : Applicative<F>
        where A : notnull
        where B : notnull
        where C : notnull
        where D : notnull
        => F.Map2(F.Map2(fa, fb, static (a, b) => (a, b)), fc, (ab, c) => f(ab.a, ab.b, c));

    /// <summary>Kleisli composition for any monad, the thing #864 (Task as monad) wants for Task and ValueTask.</summary>
    public static Func<A, K<M, C>> Compose<M, A, B, C>(this Func<A, K<M, B>> f, Func<B, K<M, C>> g)
        where M : Monad<M>
        where A : notnull
        where B : notnull
        where C : notnull
        => a => M.Bind(f(a), g);
}
