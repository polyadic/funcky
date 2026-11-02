#pragma warning disable CA1715, SA1302 // lang-ext naming: F, M, T, A, B are the established spelling for this pattern
namespace Funcky.Spike.Traits;

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
