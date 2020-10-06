using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Funcky.Monads;
using Xunit.Sdk;

namespace Funcky.Xunit
{
    public static partial class FunctionalAssert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsSetEqual<TITem>(IEnumerable<TITem> expected, IEnumerable<TITem> actual, Option<IEqualityComparer<TITem>> equalityComparer = default)
        {
            try
            {
                var referenceSet = new HashSet<TITem>(expected, equalityComparer.GetOrElse(EqualityComparer<TITem>.Default));
                if (!referenceSet.SetEquals(actual))
                {
                    throw new NotEqualException(string.Join(", ", expected), string.Join(", ", actual));
                }
            }
            catch (IsNoneException exception)
            {
                throw exception;
            }
        }
    }
}
