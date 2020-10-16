using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        /// <summary>
        /// Partitions the items in an <see cref="IEnumerable{T}"/> by the given <paramref name="predicate"/>.
        /// </summary>
        /// <example>
        /// <code><![CDATA[
        /// using System.Linq;
        /// using Funcky.Extensions;
        ///
        /// var (evens, odds) = Enumerable.Range(0, 6).Partition(n => n % 2 == 0);
        /// ]]></code>
        /// </example>
        /// <remarks>This method causes the items in <paramref name="source"/> to be materialized.</remarks>
        /// <returns>A tuple with the items for which the predicate holds, and for those for which it doesn't.</returns>
        public static (IEnumerable<TItem> True, IEnumerable<TItem> False) Partition<TItem>(
            this IEnumerable<TItem> source,
            Func<TItem, bool> predicate)
            => source.Partition(predicate, ValueTuple.Create);

        /// <summary>
        /// Partitions the items in an <see cref="IEnumerable{T}"/> by the given <paramref name="predicate"/>.
        /// The <paramref name="resultSelector"/> receives the items for which the predicate holds and the items
        /// for which it doesn't as separate parameters.
        /// </summary>
        /// <remarks>This method causes the items in <paramref name="source"/> to be materialized.</remarks>
        public static TResult Partition<TItem, TResult>(
            this IEnumerable<TItem> source,
            Func<TItem, bool> predicate,
            Func<IEnumerable<TItem>, IEnumerable<TItem>, TResult> resultSelector)
            => source
                .Aggregate(new PartitionBuilder<TItem>(predicate), PartitionBuilder<TItem>.Add)
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

            public static PartitionBuilder<TItem> Add(PartitionBuilder<TItem> builder, TItem item) => builder.Add(item);

            public TResult Build<TResult>(Func<IEnumerable<TItem>, IEnumerable<TItem>, TResult> selector) => selector(_left, _right);

            private PartitionBuilder<TItem> Add(TItem item)
                => _predicate(item)
                    ? With(left: _left.Add(item))
                    : With(right: _right.Add(item));

            private PartitionBuilder<TItem> With(IImmutableList<TItem>? left = null, IImmutableList<TItem>? right = null)
                => new PartitionBuilder<TItem>(_predicate, left ?? _left, right ?? _right);
        }
    }
}
