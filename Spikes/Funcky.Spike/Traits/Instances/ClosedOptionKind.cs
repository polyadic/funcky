#pragma warning disable CA1715, SA1302 // lang-ext naming
using Funcky.Spike.Unions;

namespace Funcky.Spike.Traits;

/// <summary>The closed-record Option as a kind. The record is a class, so it would implement <c>K</c> directly in a real
/// implementation (no wrapper, no boxing); the spike keeps the same wrapper shape as the other instances so that the
/// benchmark isolates the shape's own cost. <see cref="ClosedOptionDirectKind"/> is the direct variant.</summary>
public readonly record struct ClosedOptionK<A>(ClosedOption<A> Option) : K<ClosedOptionKind, A>
    where A : notnull;

public sealed class ClosedOptionKind : Monad<ClosedOptionKind>, Traversable<ClosedOptionKind>, Alternative<ClosedOptionKind>
{
    public static K<ClosedOptionKind, A> Pure<A>(A value)
        where A : notnull
        => new ClosedOptionK<A>(ClosedOption.Some(value));

    public static K<ClosedOptionKind, B> Map<A, B>(K<ClosedOptionKind, A> fa, Func<A, B> f)
        where A : notnull
        where B : notnull
        => new ClosedOptionK<B>(As(fa).Select(f));

    public static K<ClosedOptionKind, C> Map2<A, B, C>(K<ClosedOptionKind, A> fa, K<ClosedOptionKind, B> fb, Func<A, B, C> f)
        where A : notnull
        where B : notnull
        where C : notnull
        => new ClosedOptionK<C>(As(fa).SelectMany(a => As(fb).Select(b => f(a, b))));

    public static K<ClosedOptionKind, B> Bind<A, B>(K<ClosedOptionKind, A> ma, Func<A, K<ClosedOptionKind, B>> f)
        where A : notnull
        where B : notnull
        => new ClosedOptionK<B>(As(ma).SelectMany(a => As(f(a))));

    public static K<F, K<ClosedOptionKind, B>> Traverse<F, A, B>(K<ClosedOptionKind, A> ta, Func<A, K<F, B>> f)
        where F : Applicative<F>
        where A : notnull
        where B : notnull
        => As(ta).Match(
            none: () => F.Pure(Empty<B>()),
            some: a => F.Map(f(a), Pure));

    public static K<ClosedOptionKind, A> Empty<A>()
        where A : notnull
        => new ClosedOptionK<A>(ClosedOption.None<A>());

    public static K<ClosedOptionKind, A> OrElse<A>(K<ClosedOptionKind, A> fa, Func<K<ClosedOptionKind, A>> fallback)
        where A : notnull
        => As(fa).IsSome ? fa : fallback();

    public static ClosedOption<A> As<A>(K<ClosedOptionKind, A> k)
        where A : notnull
        => ((ClosedOptionK<A>)k).Option;
}

/// <summary>What a closed-record Option would really look like with traits: the cases implement <c>K</c> themselves.
/// Pure/Map/Bind allocate exactly the result object and nothing else.</summary>
public closed record DirectOption<A> : K<ClosedOptionDirectKind, A>
    where A : notnull
{
    private DirectOption()
    {
    }

    public sealed record Some(A Value) : DirectOption<A>;

    public sealed record None : DirectOption<A>
    {
        public static readonly None Instance = new();

        private None()
        {
        }
    }
}

public sealed class ClosedOptionDirectKind : Monad<ClosedOptionDirectKind>, Traversable<ClosedOptionDirectKind>, Alternative<ClosedOptionDirectKind>
{
    public static K<ClosedOptionDirectKind, A> Pure<A>(A value)
        where A : notnull
        => new DirectOption<A>.Some(value);

    public static K<ClosedOptionDirectKind, B> Map<A, B>(K<ClosedOptionDirectKind, A> fa, Func<A, B> f)
        where A : notnull
        where B : notnull
        => fa switch
        {
            DirectOption<A>.Some(var a) => new DirectOption<B>.Some(f(a)),
            _ => DirectOption<B>.None.Instance,
        };

    public static K<ClosedOptionDirectKind, C> Map2<A, B, C>(K<ClosedOptionDirectKind, A> fa, K<ClosedOptionDirectKind, B> fb, Func<A, B, C> f)
        where A : notnull
        where B : notnull
        where C : notnull
        => (fa, fb) switch
        {
            (DirectOption<A>.Some(var a), DirectOption<B>.Some(var b)) => new DirectOption<C>.Some(f(a, b)),
            _ => DirectOption<C>.None.Instance,
        };

    public static K<ClosedOptionDirectKind, B> Bind<A, B>(K<ClosedOptionDirectKind, A> ma, Func<A, K<ClosedOptionDirectKind, B>> f)
        where A : notnull
        where B : notnull
        => ma switch
        {
            DirectOption<A>.Some(var a) => f(a),
            _ => DirectOption<B>.None.Instance,
        };

    public static K<F, K<ClosedOptionDirectKind, B>> Traverse<F, A, B>(K<ClosedOptionDirectKind, A> ta, Func<A, K<F, B>> f)
        where F : Applicative<F>
        where A : notnull
        where B : notnull
        => ta switch
        {
            DirectOption<A>.Some(var a) => F.Map(f(a), Pure),
            _ => F.Pure(Empty<B>()),
        };

    public static K<ClosedOptionDirectKind, A> Empty<A>()
        where A : notnull
        => DirectOption<A>.None.Instance;

    public static K<ClosedOptionDirectKind, A> OrElse<A>(K<ClosedOptionDirectKind, A> fa, Func<K<ClosedOptionDirectKind, A>> fallback)
        where A : notnull
        => fa is DirectOption<A>.Some ? fa : fallback();

    public static DirectOption<A> As<A>(K<ClosedOptionDirectKind, A> k)
        where A : notnull
        => (DirectOption<A>)k;
}
