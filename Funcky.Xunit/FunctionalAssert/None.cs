using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace Funcky;

public static partial class FunctionalAssert
{
    /// <inheritdoc cref="AssertExtensions.extension(Assert).None{TItem}(Option{TItem})"/>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static void None<TItem>(Option<TItem> option)
        where TItem : notnull
    {
        try
        {
            Assert.None(option);
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }
}
