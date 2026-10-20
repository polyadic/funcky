using System.ComponentModel;

namespace Funcky.Monads;

public static partial class OptionExtensions
{
    [Pure]
    public static Option<T> Flatten<T>(this Option<Option<T>> option)
        where T : notnull
#pragma warning disable λ1018 // This is the implementation of Flatten.
        => option.SelectMany(Identity);
#pragma warning restore λ1018

    [Pure]
    public static Either<TLeft, TRight> ToEither<TLeft, TRight>(this Option<TRight> option, TLeft left)
        where TLeft : notnull
        where TRight : notnull
        => option.Match(
            none: Either<TLeft, TRight>.Left(left),
            some: Either<TLeft, TRight>.Right);

    [Pure]
    public static Either<TLeft, TRight> ToEither<TLeft, TRight>(this Option<TRight> option, Func<TLeft> left)
        where TLeft : notnull
        where TRight : notnull
    {
        var toLeft = Either<TLeft, TRight>.Left;

        return option.Match(
            none: toLeft.Compose(left),
            some: Either<TLeft, TRight>.Right);
    }

    /// <summary>Converts the option to a nullable value: <c>Some(item)</c> becomes the item, <c>None</c> becomes <c>null</c>.</summary>
    /// <remarks>Never pass <paramref name="ω"/>. It only exists so that the overloads for structs and classes, which would otherwise have the same signature, can coexist; <see cref="RequireStruct{T}"/> and <see cref="RequireClass{T}"/> have no other purpose.</remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Pure]
    public static TItem? ToNullable<TItem>(this Option<TItem> option, RequireStruct<TItem>? ω = null)
        where TItem : struct
        => option.Match(
            none: null as TItem?,
            some: item => item);

    /// <summary>Converts the option to a nullable value: <c>Some(item)</c> becomes the item, <c>None</c> becomes <c>null</c>.</summary>
    /// <remarks>Never pass <paramref name="ω"/>. It only exists so that the overloads for structs and classes, which would otherwise have the same signature, can coexist; <see cref="RequireStruct{T}"/> and <see cref="RequireClass{T}"/> have no other purpose.</remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [Pure]
    public static TItem? ToNullable<TItem>(this Option<TItem> option, RequireClass<TItem>? ω = null)
        where TItem : class
        => option.Match(
            none: null as TItem,
            some: Identity);
}
