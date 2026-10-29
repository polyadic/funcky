using System.Diagnostics.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace Funcky;

public static partial class FunctionalAssert
{
    /// <inheritdoc cref="AssertExtensions.extension(Assert).Error{TValidResult}(Result{TValidResult})"/>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static Exception Error<TValidResult>(Result<TValidResult> result)
        where TValidResult : notnull
    {
        try
        {
            return Assert.Error(result);
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }

    /// <inheritdoc cref="AssertExtensions.extension(Assert).Error{TException, TValidResult}(Result{TValidResult})"/>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static TException Error<TException, TValidResult>(Result<TValidResult> result)
        where TException : Exception
        where TValidResult : notnull
    {
        try
        {
            return Assert.Error<TException, TValidResult>(result);
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }
}
