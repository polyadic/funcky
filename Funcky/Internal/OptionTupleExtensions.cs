namespace Funcky.Internal
{
    internal static class OptionTupleExtensions
    {
        public static TResult Match<TLeft, TRight, TResult>(
            this (Option<TLeft> Left, Option<TRight> Right) tuple,
            Func<TLeft, TResult> left,
            Func<TRight, TResult> right,
            Func<TLeft, TRight, TResult> leftAndRight,
            Func<TResult> none)
            where TLeft : notnull
            where TRight : notnull
            => tuple.Left.Match(
                some: leftItem => tuple.Right.Match(
                    some: rightItem => leftAndRight(leftItem, rightItem),
                    none: () => left(leftItem)),
                none: tuple.Right.Match(
                    some: right,
                    none: none));
    }
}
