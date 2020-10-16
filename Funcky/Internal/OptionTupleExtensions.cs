using System;
using Funcky.Monads;

namespace Funcky.Internal
{
    internal static class OptionTupleExtensions
    {
        public static TResult Match<TItem, TResult>(
            this (Option<TItem> Left, Option<TItem> Right) tuple,
            Func<TItem, TResult> left,
            Func<TItem, TResult> right,
            Func<TItem, TItem, TResult> leftAndRight,
            Func<TResult> none)
            where TItem : notnull
            => tuple.Left.Match(
                some: leftItem => tuple.Right.Match(
                    some: rightItem => leftAndRight(leftItem, rightItem),
                    none: () => left(leftItem)),
                none: tuple.Right.Match(
                    some: right,
                    none: none));
    }
}
