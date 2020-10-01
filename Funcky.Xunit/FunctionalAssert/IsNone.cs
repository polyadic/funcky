using System.Runtime.CompilerServices;
using Funcky.Monads;
using Xunit.Sdk;
using static Funcky.Functional;

namespace Funcky.Xunit
{
    public static partial class FunctionalAssert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsNone<TItem>(Option<TItem> option)
            where TItem : notnull
        {
            try
            {
                option.Match(
                    none: NoOperation,
                    some: value => throw new IsNoneException(value));
            }
            catch (IsNoneException exception)
            {
                throw exception;
            }
        }
    }
}
