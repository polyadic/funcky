namespace Funcky.Spike.Unions;

/// <summary>
/// Shape 1b: the union lists the payload type itself as a case, <c>union BareOption&lt;T&gt;(T, None)</c>.
/// <para>RC3 accepts a type parameter as a case. <c>BareOption&lt;int&gt; o = 5;</c> is the generated conversion from the
/// case, and <c>o is T value</c> is the whole match. No <c>Some</c> wrapper exists, so <c>Some(None)</c> cannot be
/// expressed: for <c>BareOption&lt;BareOption&lt;int&gt;&gt;</c> the outer union's <c>Value</c> holds the inner union
/// boxed, and the inner None is a Some of the outer, which is correct, but <c>BareOption&lt;None&gt;</c> is ambiguous
/// by construction. The nesting question is what the <c>Some</c> wrapper in shape 1 buys.</para>
/// </summary>
public partial union BareOption<T>(T, None)
    where T : notnull
{
    public static BareOption<T> Empty => new None();

    public TResult Match<TResult>(Func<TResult> none, Func<T, TResult> some)
        => this switch
        {
            T value => some(value),
            _ => none(),
        };

    public BareOption<TResult> Select<TResult>(Func<T, TResult> selector)
        where TResult : notnull
        => this switch
        {
            T value => selector(value),
            _ => new None(),
        };

    public override string ToString()
        => this switch
        {
            T value => $"Some({value})",
            _ => "None",
        };
}
