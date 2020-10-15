using System.Collections.Generic;

namespace Funcky.Monads
{
    // We could make this public if someone needs to build a custom option comparer.
    internal sealed class OptionComparer<TItem> : Comparer<Option<TItem>>, IOptionComparer<TItem>
        where TItem : notnull
    {
        private readonly IComparer<TItem> _comparer;

        private OptionComparer(IComparer<TItem> comparer) => _comparer = comparer;

        public static new IOptionComparer<TItem> Default => new OptionComparer<TItem>(Comparer<TItem>.Default);

        public override int Compare(Option<TItem> x, Option<TItem> y)
        {
            var leftHasValue = x.TryGetValue(out var left);
            var rightHasValue = y.TryGetValue(out var right);
            return leftHasValue && rightHasValue
                ? _comparer.Compare(left!, right!)
                : leftHasValue.CompareTo(rightHasValue);
        }
    }
}
