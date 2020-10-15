using System;
using Funcky.Monads;

namespace Funcky.Internal
{
    internal static class OptionTupleExtensions
    {
        public static TResult Match<TItem, TResult>(
            this (Option<TItem> Left, Option<TItem> Right) tuple,
            Func<TItem, TItem, TResult> both,
            Func<TItem, TResult> left,
            Func<TItem, TResult> right,
            Func<TResult> neither)
            where TItem : notnull
            => tuple.Left.Match(
                some: l => tuple.Right.Match(
                    some: r => both(l, r),
                    none: () => left(l)),
                none: () => tuple.Right.Match(
                    some: right,
                    none: neither));
    }
}
