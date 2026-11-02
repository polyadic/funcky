#pragma warning disable CA1715, SA1302 // lang-ext naming
namespace Funcky.Spike.Traits;

/// <summary><c>IEnumerable&lt;A&gt;</c> as a kind. The interface cannot be made to implement <c>K</c>, so a wrapper class
/// carries it; the wrapper is one allocation per sequence, which is nothing next to the enumeration itself.</summary>
public sealed record Seq<A>(IEnumerable<A> Items) : K<SeqKind, A>
    where A : notnull;

public sealed class SeqKind : Monad<SeqKind>, Traversable<SeqKind>
{
    public static K<SeqKind, A> Pure<A>(A value)
        where A : notnull
        => new Seq<A>([value]);

    public static K<SeqKind, B> Map<A, B>(K<SeqKind, A> fa, Func<A, B> f)
        where A : notnull
        where B : notnull
        => new Seq<B>(As(fa).Select(f));

    public static K<SeqKind, C> Map2<A, B, C>(K<SeqKind, A> fa, K<SeqKind, B> fb, Func<A, B, C> f)
        where A : notnull
        where B : notnull
        where C : notnull
        => new Seq<C>(As(fa).SelectMany(_ => As(fb), f));

    public static K<SeqKind, B> Bind<A, B>(K<SeqKind, A> ma, Func<A, K<SeqKind, B>> f)
        where A : notnull
        where B : notnull
        => new Seq<B>(As(ma).SelectMany(a => As(f(a))));

    /// <summary>The one Traverse that today exists as IEnumerable × {Option, Either, Result, Lazy, Task, ...}.
    /// Materializes: an applicative effect cannot be deferred without knowing the effect.</summary>
    public static K<F, K<SeqKind, B>> Traverse<F, A, B>(K<SeqKind, A> ta, Func<A, K<F, B>> f)
        where F : Applicative<F>
        where A : notnull
        where B : notnull
    {
        var result = F.Pure<List<B>>([]);
        foreach (var item in As(ta))
        {
            result = F.Map2(result, f(item), static (list, b) =>
            {
                list.Add(b);
                return list;
            });
        }

        return F.Map(result, static list => (K<SeqKind, B>)new Seq<B>(list));
    }

    public static IEnumerable<A> As<A>(K<SeqKind, A> k)
        where A : notnull
        => ((Seq<A>)k).Items;
}
