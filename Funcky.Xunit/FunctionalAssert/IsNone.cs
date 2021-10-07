using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Xunit.Sdk;

namespace Funcky.Xunit
{
    public static partial class FunctionalAssert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
        public static void IsNone<TItem>(Option<TItem> option)
            where TItem : notnull
        {
            try
            {
                option.Switch(
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
