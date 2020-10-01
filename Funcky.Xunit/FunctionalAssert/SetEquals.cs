using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xunit.Sdk;

namespace Funcky.Xunit
{
    public static partial class FunctionalAssert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsSetEquals<TITem>(IEnumerable<TITem> expected, IEnumerable<TITem> actual)
        {
            try
            {
                var referenceSet = new HashSet<TITem>(expected, EqualityComparer<TITem>.Default);
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
