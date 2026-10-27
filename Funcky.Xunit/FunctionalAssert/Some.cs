using System.Diagnostics.CodeAnalysis;
using Funcky.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace Funcky;

public static partial class FunctionalAssert
{
    /// <inheritdoc cref="AssertExtensions.extension(Assert).Some{TItem}(TItem, Option{TItem})"/>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static void Some<TItem>(TItem expectedValue, Option<TItem> option)
        where TItem : notnull
    {
        try
        {
            Assert.Some(expectedValue, option);
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }

    /// <inheritdoc cref="AssertExtensions.extension(Assert).Some{TItem}(Option{TItem})"/>
    [AssertMethodHasOverloadWithExpectedValue]
    [Pure]
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static TItem Some<TItem>(Option<TItem> option)
        where TItem : notnull
    {
        try
        {
            return Assert.Some(option);
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }
}
