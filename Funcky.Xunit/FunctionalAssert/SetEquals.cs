using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Funcky.Monads;
using Xunit.Sdk;

namespace Funcky.Xunit
{
    public static partial class FunctionalAssert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsSetEquals<TITem>(IEnumerable<TITem> expected, IEnumerable<TITem> actual, IEqualityComparer<TITem>? equalityComparer = null)
        {
            try
            {
                var referenceSet = new HashSet<TITem>(expected, Option.FromNullable(equalityComparer).GetOrElse(EqualityComparer<TITem>.Default));
                if (!referenceSet.SetEquals(actual))
                {
                    throw new NotEqualException(expected.ToString(), actual.ToString());
                }
            }
            catch (IsNoneException exception)
            {
                throw exception;
            }
        }
    }
}
