using System.Diagnostics.CodeAnalysis;
using Funcky.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace Funcky;

public static partial class FunctionalAssert
{
    /// <inheritdoc cref="AssertExtensions.extension(Assert).Ok{TValidResult}(TValidResult, Result{TValidResult})"/>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static void Ok<TValidResult>(TValidResult expectedResult, Result<TValidResult> result)
        where TValidResult : notnull
    {
        try
        {
            Assert.Ok(expectedResult, result);
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }

    /// <inheritdoc cref="AssertExtensions.extension(Assert).Ok{TValidResult}(Result{TValidResult})"/>
    [AssertMethodHasOverloadWithExpectedValue]
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static TValidResult Ok<TValidResult>(Result<TValidResult> result)
        where TValidResult : notnull
    {
        try
        {
            return Assert.Ok(result);
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }
}
