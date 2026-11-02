using System.Diagnostics.CodeAnalysis;

namespace Funcky.Spike.Unions;

/// <summary>
/// Shape 1: <c>Option&lt;T&gt;</c> as a C# 15 nominal union over two case structs.
/// <para>What the compiler generates (RC3): a <c>readonly struct</c> with one <c>object Value</c> field, implicit
/// conversions from each case type, <c>System.Runtime.CompilerServices.IUnion</c> and <c>[Union]</c>. Patterns on the
/// union are patterns on <c>Value</c>, so <c>o is Some&lt;T&gt;(var v)</c> works and a switch over all cases is exhaustive.</para>
/// <para>Consequences measured by the spike: every <c>Some</c> boxes its case struct (one 24-byte allocation for an
/// <c>int</c>), <c>default(UnionOption&lt;T&gt;)</c> has a <c>null</c> Value and matches no case (a switch without
/// <c>_</c> throws <see cref="System.Runtime.CompilerServices.SwitchExpressionException"/>), and the case types live
/// outside the union, so <c>None</c> as a type and <c>None</c> as a member cannot coexist on the union.</para>
/// </summary>
public partial union UnionOption<T>(Some<T>, None) : IEquatable<UnionOption<T>>
    where T : notnull
{
    /// <summary>Mirrors <c>Option&lt;T&gt;.None</c>. Named <c>Empty</c> because the case type <c>None</c> is in scope.</summary>
    public static UnionOption<T> Empty => new None();

    public static implicit operator UnionOption<T>(T value) => new Some<T>(value);

    public bool IsSome => Value is Some<T>;

    public TResult Match<TResult>(TResult none, Func<T, TResult> some)
        => this switch
        {
            Some<T>(var value) => some(value),
            // A `None` arm would be exhaustive for the compiler, but `default(UnionOption<T>)` (Value == null) would
            // then throw at runtime. The spike treats the null union as None; the catch-all also hides a missing case.
            _ => none,
        };

    public TResult Match<TResult>(Func<TResult> none, Func<T, TResult> some)
        => this switch
        {
            Some<T>(var value) => some(value),
            _ => none(),
        };

    public void Switch(Action none, Action<T> some)
    {
        if (this is Some<T>(var value))
        {
            some(value);
        }
        else
        {
            none();
        }
    }

    public UnionOption<TResult> Select<TResult>(Func<T, TResult> selector)
        where TResult : notnull
        => this switch
        {
            Some<T>(var value) => new Some<TResult>(selector(value)),
            _ => new None(),
        };

    public UnionOption<TResult> SelectMany<TResult>(Func<T, UnionOption<TResult>> selector)
        where TResult : notnull
        => this switch
        {
            Some<T>(var value) => selector(value),
            _ => new None(),
        };

    public UnionOption<TResult> SelectMany<TOption, TResult>(Func<T, UnionOption<TOption>> selector, Func<T, TOption, TResult> resultSelector)
        where TOption : notnull
        where TResult : notnull
        => SelectMany(value => selector(value).Select(option => resultSelector(value, option)));

    public T GetOrElse(T fallback)
        => this is Some<T>(var value) ? value : fallback;

    public UnionOption<T> OrElse(UnionOption<T> fallback)
        => IsSome ? this : fallback;

    public bool TryGetValue([NotNullWhen(true)] out T? value)
    {
        if (this is Some<T>(var some))
        {
            value = some;
            return true;
        }

        value = default;
        return false;
    }

    public bool Equals(UnionOption<T> other) => Equals(Value, other.Value);

    public override bool Equals(object? obj) => obj is UnionOption<T> other && Equals(other);

    public override int GetHashCode() => Value?.GetHashCode() ?? 0;

    public override string ToString()
        => this switch
        {
            Some<T>(var value) => $"Some({value})",
            _ => "None",
        };
}

public static class UnionOption
{
    public static UnionOption<T> Some<T>(T value)
        where T : notnull
        => new Some<T>(value);

    public static UnionOption<T> Return<T>(T value)
        where T : notnull
        => new Some<T>(value);
}
