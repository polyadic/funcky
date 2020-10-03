using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        public static (IEnumerable<TItem> True, IEnumerable<TItem> False) Partition<TItem>(
            this IEnumerable<TItem> source,
            Func<TItem, bool> predicate)
            => source.Partition(predicate, ValueTuple.Create);

        public static TResult Partition<TItem, TResult>(
            this IEnumerable<TItem> source,
            Func<TItem, bool> predicate,
            Func<IEnumerable<TItem>, IEnumerable<TItem>, TResult> resultSelector)
            => source
                .Aggregate(new PartitionBuilder<TItem>(predicate), (builder, item) => builder.Add(item))
                .Build(resultSelector);

        private readonly struct PartitionBuilder<TItem>
        {
            private readonly Func<TItem, bool> _predicate;

            private readonly IImmutableList<TItem> _left;

            private readonly IImmutableList<TItem> _right;

            public PartitionBuilder(Func<TItem, bool> predicate, IImmutableList<TItem>? left = null, IImmutableList<TItem>? right = null)
            {
                _predicate = predicate;
                _left = left ?? ImmutableList<TItem>.Empty;
                _right = right ?? ImmutableList<TItem>.Empty;
            }

            public PartitionBuilder<TItem> Add(TItem item)
                => _predicate(item)
                    ? new PartitionBuilder<TItem>(_predicate, _left.Add(item), _right)
                    : new PartitionBuilder<TItem>(_predicate, _left, _right.Add(item));

            public TResult Build<TResult>(Func<IEnumerable<TItem>, IEnumerable<TItem>, TResult> selector)
                => selector(_left, _right);
        }
    }
}
