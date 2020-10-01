using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Funcky.Extensions
{
    public static partial class EnumerableExtensions
    {
        [Pure]
        public static IEnumerable<ValueWithFirst<TSource>> WithFirst<TSource>(this IEnumerable<TSource> source)
        {
            using var enumerator = source.GetEnumerator();

            if (!enumerator.MoveNext())
            {
                yield break;
            }

            yield return new ValueWithFirst<TSource>(enumerator.Current, true);

            while (enumerator.MoveNext())
            {
                yield return new ValueWithFirst<TSource>(enumerator.Current, false);
            }
        }

        public readonly struct ValueWithFirst<TValue>
        {
            public ValueWithFirst(TValue value, bool isFirst)
            {
                Value = value;
                IsFirst = isFirst;
            }

            public TValue Value { get; }

            public bool IsFirst { get; }

            public void Deconstruct(out TValue value, out bool isFirst)
            {
                value = Value;
                isFirst = IsFirst;
            }
        }
    }
}
