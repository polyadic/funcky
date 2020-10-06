using System.Collections.Generic;

namespace Funcky.Xunit
{
    public abstract class CollectionEquality<TElement>
    {
        public static IEqualityComparer<IEnumerable<TElement>> UnorderedSequenceEquality
            => new UnorderedSequenceEqualityComparer<TElement>();
    }
}
