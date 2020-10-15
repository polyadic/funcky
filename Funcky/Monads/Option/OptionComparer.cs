using System;
using System.Collections;
using System.Collections.Generic;
using Funcky.Internal;

namespace Funcky.Monads
{
    // We could make this public if someone needs to build a custom option comparer.
    internal sealed class OptionComparer<TItem> : IComparer, IComparer<Option<TItem>>
        where TItem : notnull
    {
        private readonly IComparer<TItem> _comparer;

        private OptionComparer(IComparer<TItem> comparer) => _comparer = comparer;

        public static OptionComparer<TItem> Default => new OptionComparer<TItem>(Comparer<TItem>.Default);

        public int Compare(object? x, object? y)
            => (x, y) switch
            {
                (null, null) => ComparisonResult.Equal,
                (null, _) => ComparisonResult.LessThan,
                (_, null) => ComparisonResult.GreaterThan,
                (Option<TItem> left, Option<TItem> right) => Compare(left, right),
                _ => throw new ArgumentException("Objects must be of type Option<TItem>"),
            };

        public int Compare(Option<TItem> x, Option<TItem> y)
        {
            var leftHasValue = x.TryGetValue(out var left);
            var rightHasValue = y.TryGetValue(out var right);
            return leftHasValue && rightHasValue
                ? _comparer.Compare(left!, right!)
                : leftHasValue.CompareTo(rightHasValue);
        }
    }
}
