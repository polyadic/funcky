using System.Collections.Generic;
using Funcky.Monads;

namespace Funcky.Xunit
{
    public class UnorderedSequenceEqualityComparer<TElement> : IEqualityComparer<IEnumerable<TElement>>
    {
        private Option<IEqualityComparer<TElement>> _equalityComparer;

        public UnorderedSequenceEqualityComparer()
        {
        }

        public UnorderedSequenceEqualityComparer(IEqualityComparer<TElement> equalityComparer)
        {
            _equalityComparer = Option.Some(equalityComparer);
        }

        public bool Equals(IEnumerable<TElement> left, IEnumerable<TElement> right)
        {
            var referenceSet = new HashSet<TElement>(left, _equalityComparer.GetOrElse(EqualityComparer<TElement>.Default));

            return referenceSet.SetEquals(right);
        }

        public int GetHashCode(IEnumerable<TElement> obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
