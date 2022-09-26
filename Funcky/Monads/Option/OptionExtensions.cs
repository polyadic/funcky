#pragma warning disable RS0026
namespace Funcky.Monads;

public static partial class OptionExtensions
{
    public static Either<TLeft, TRight> ToEither<TLeft, TRight>(this Option<TRight> option, TLeft left)
        where TLeft : notnull
        where TRight : notnull
        => option.Match(
            none: Either<TLeft, TRight>.Left(left),
            some: Either<TLeft, TRight>.Right);

    public static Either<TLeft, TRight> ToEither<TLeft, TRight>(this Option<TRight> option, Func<TLeft> left)
        where TLeft : notnull
        where TRight : notnull
    {
        var toLeft = Either<TLeft, TRight>.Left;

        return option.Match(
            none: toLeft.Compose(left),
            some: Either<TLeft, TRight>.Right);
    }

    public static TItem? ToNullable<TItem>(this Option<TItem> option, RequireStruct<TItem>? ω = null)
        where TItem : struct
        => option.Match(
            none: null as TItem?,
            some: item => item);

    public static TItem? ToNullable<TItem>(this Option<TItem> option, RequireClass<TItem>? ω = null)
        where TItem : class
        => option.Match(
            none: null as TItem,
            some: item => item);
}
