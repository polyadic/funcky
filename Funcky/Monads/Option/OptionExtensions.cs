#pragma warning disable SA1317 //  we use ω to denote parameters which are not used, we exempt them from latin-only-identifier convention

using System.ComponentModel;

namespace Funcky.Monads;

public static partial class OptionExtensions
{
    extension<T>(Option<Option<T>> option)
        where T : notnull
    {
        [Pure]
        public Option<T> Flatten()
#pragma warning disable λ1018 // This is the implementation of Flatten.
            => option.SelectMany(Identity);
    }

#pragma warning restore λ1018
    extension<TItem>(Option<TItem> option)
        where TItem : struct
    {
        /// <summary>Converts the option to a nullable value: <c>Some(item)</c> becomes the item, <c>None</c> becomes <c>null</c>.</summary>
        /// <remarks>Never pass <paramref name="ω"/>. It only exists so that the overloads for structs and classes, which would otherwise have the same signature, can coexist; <see cref="RequireStruct{T}"/> and <see cref="RequireClass{T}"/> have no other purpose.</remarks>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Pure]
        public TItem? ToNullable(RequireStruct<TItem>? ω = null)
            => option.Match(
                none: null as TItem?,
                some: item => item);
    }

    extension<TItem>(Option<TItem> option)
        where TItem : class
    {
        /// <summary>Converts the option to a nullable value: <c>Some(item)</c> becomes the item, <c>None</c> becomes <c>null</c>.</summary>
        /// <remarks>Never pass <paramref name="ω"/>. It only exists so that the overloads for structs and classes, which would otherwise have the same signature, can coexist; <see cref="RequireStruct{T}"/> and <see cref="RequireClass{T}"/> have no other purpose.</remarks>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [Pure]
        public TItem? ToNullable(RequireClass<TItem>? ω = null)
            => option.Match(
                none: null as TItem,
                some: Identity);
    }

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
}
