namespace Funcky.Spike.Unions;

/// <summary>
/// Shape 2: <c>Option&lt;T&gt;</c> as a C# 15 <c>closed</c> record hierarchy.
/// <para>A closed type is implicitly abstract; all derived types must live in the same assembly, so the compiler treats a
/// switch over <c>Some</c> and <c>None</c> as exhaustive (CS8509 for a missing case, no catch-all needed). The cases are
/// nested types, which makes <c>ClosedOption&lt;T&gt;.None</c> a type: today's <c>Option&lt;T&gt;.None</c> property has
/// to move (here: <see cref="ClosedOption.None{T}"/>). References are nullable, so <c>default</c> is <c>null</c> and the
/// nullable flow analysis, not a runtime exception, reports its misuse.</para>
/// <para>Costs: one allocation per <c>Some</c> (a 24-byte object for <c>int</c>), <c>None</c> is a cached singleton,
/// every value is a reference, so a <c>K&lt;F, A&gt;</c> trait interface is free (see Traits).</para>
/// </summary>
public closed record ClosedOption<T>
    where T : notnull
{
    private ClosedOption()
    {
    }

    public static implicit operator ClosedOption<T>(T value) => new Some(value);

    public bool IsSome => this is Some;

    public TResult Match<TResult>(TResult none, Func<T, TResult> some)
        => this switch
        {
            Some(var value) => some(value),
            None => none,
        };

    public TResult Match<TResult>(Func<TResult> none, Func<T, TResult> some)
        => this switch
        {
            Some(var value) => some(value),
            None => none(),
        };

    public ClosedOption<TResult> Select<TResult>(Func<T, TResult> selector)
        where TResult : notnull
        => this switch
        {
            Some(var value) => new ClosedOption<TResult>.Some(selector(value)),
            None => ClosedOption<TResult>.None.Instance,
        };

    public ClosedOption<TResult> SelectMany<TResult>(Func<T, ClosedOption<TResult>> selector)
        where TResult : notnull
        => this switch
        {
            Some(var value) => selector(value),
            None => ClosedOption<TResult>.None.Instance,
        };

    public T GetOrElse(T fallback)
        => this is Some(var value) ? value : fallback;

    public sealed override string ToString()
        => this switch
        {
            Some(var value) => $"Some({value})",
            None => "None",
        };

    public sealed record Some(T Value) : ClosedOption<T>;

    public sealed record None : ClosedOption<T>
    {
        public static readonly None Instance = new();

        private None()
        {
        }
    }
}

public static class ClosedOption
{
    public static ClosedOption<T> Some<T>(T value)
        where T : notnull
        => new ClosedOption<T>.Some(value);

    public static ClosedOption<T> None<T>()
        where T : notnull
        => ClosedOption<T>.None.Instance;
}
