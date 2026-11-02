namespace Funcky.Spike.Traits;

/// <summary>
/// Higher-kinded types the lang-ext way (#862): <c>K&lt;F, A&gt;</c> stands for "the type constructor <c>F</c> applied to
/// <c>A</c>". A concrete type <c>Option&lt;A&gt;</c> implements <c>K&lt;OptionKind, A&gt;</c>; the trait implementations
/// (<see cref="Functor{F}"/>, <see cref="Monad{M}"/>, ...) are static abstract members on the marker type
/// <c>OptionKind</c>, found through the constraint <c>where F : Monad&lt;F&gt;</c>.
/// <para>The whole scheme works since C# 11. The question for Funcky is the cost: <c>K&lt;F, A&gt;</c> is an interface,
/// so a <em>struct</em> Option is boxed on every entry into a trait-generic algorithm and unboxed (type test) on exit.
/// lang-ext pays nothing because all its types are classes. This is the link to #1034: a closed-record Option is a class
/// and gets traits for free; the struct Option and the union Option (which is itself a struct) pay one box per step.</para>
/// <para>Constraints: an implementation of a static abstract member must repeat the interface's constraints exactly, so
/// Funcky's <c>where T : notnull</c> has to be on the trait. The spike puts it on <c>K</c> and every trait method, which
/// fits Funcky (all its monads demand notnull) but means <c>IEnumerable&lt;string?&gt;</c> has no kind. Attribute-style
/// requirements (Lazy's <c>[DynamicallyAccessedMembers]</c>) cannot be expressed on the trait at all.</para>
/// </summary>
/// <typeparam name="F">The marker type of the type constructor (e.g. <c>OptionKind</c>).</typeparam>
/// <typeparam name="A">The type argument.</typeparam>
#pragma warning disable SA1302, CA1715 // lang-ext naming: K, F, A are the established spelling for this pattern
public interface K<F, out A>
    where F : Kind<F>
    where A : notnull;

/// <summary>Marker for a type constructor. Self-constrained so that trait interfaces can refer to their implementer.</summary>
public interface Kind<F>
    where F : Kind<F>;
#pragma warning restore SA1302, CA1715
