namespace Funcky.Spike.Unions;

/// <summary>
/// <c>Either&lt;L, R&gt;</c> as a union. The two cases need distinct wrapper types even when <c>L == R</c>: identical
/// case types are rejected (CS0111), and <c>union E&lt;L, R&gt;(L, R)</c> would make <c>E&lt;int, int&gt;</c>
/// indistinguishable at runtime. The invalid <c>default</c> state Funcky guards with <c>[NonDefaultable]</c> exists
/// here too: a <c>null</c> Value.
/// </summary>
public partial union UnionEither<TLeft, TRight>(Left<TLeft>, Right<TRight>)
    where TLeft : notnull
    where TRight : notnull
{
    public TResult Match<TResult>(Func<TLeft, TResult> left, Func<TRight, TResult> right)
        => this switch
        {
            Left<TLeft>(var value) => left(value),
            Right<TRight>(var value) => right(value),
            _ => throw new NotSupportedException("Either constructed via default is not supported."),
        };

    public UnionEither<TLeft, TResult> Select<TResult>(Func<TRight, TResult> selector)
        where TResult : notnull
        => this switch
        {
            Left<TLeft> left => new UnionEither<TLeft, TResult>(left),
            Right<TRight>(var value) => new Right<TResult>(selector(value)),
            _ => throw new NotSupportedException("Either constructed via default is not supported."),
        };

    public override string ToString()
        => this switch
        {
            Left<TLeft>(var value) => $"Left({value})",
            Right<TRight>(var value) => $"Right({value})",
            _ => "Uninitialized",
        };
}
