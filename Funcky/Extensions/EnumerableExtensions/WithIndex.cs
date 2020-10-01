using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static IEnumerable<ValueWithIndex<TSource>> WithIndex<TSource>(this IEnumerable<TSource> source)
            => source.Select((value, index) => new ValueWithIndex<TSource>(value, index));

        public readonly struct ValueWithIndex<TValue>
        {
            public ValueWithIndex(TValue value, int index)
            {
                Value = value;
                Index = index;
            }

            public TValue Value { get; }

            public int Index { get; }

            public void Deconstruct(out TValue value, out int index)
            {
                value = Value;
                index = Index;
            }
        }
    }
}
