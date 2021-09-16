using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Xunit;
using Xunit.Sdk;

namespace Funcky.Xunit
{
    public static partial class FunctionalAssert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IsSome<TItem>(TItem expectedValue, Option<TItem> option)
            where TItem : notnull
        {
            try
            {
                Assert.Equal(Option.Some(expectedValue), option);
            }
            catch (EqualException)
            {
                throw new IsSomeWithExpectedValueException(expectedValue, option);
            }
        }

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
        public static TItem IsSome<TItem>(Option<TItem> option)
            where TItem : notnull
        {
            try
            {
                return option.Match(
                    some: Identity,
                    none: () => throw new IsSomeException());
            }
            catch (IsSomeException exception)
            {
                throw exception;
            }
        }
    }
}
