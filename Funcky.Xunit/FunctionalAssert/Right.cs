using System.Diagnostics.CodeAnalysis;
using Funcky.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace Funcky;

public static partial class FunctionalAssert
{
    /// <inheritdoc cref="AssertExtensions.extension(Assert).Right{TLeft, TRight}(TRight, Either{TLeft, TRight})"/>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static void Right<TLeft, TRight>(TRight expectedRight, Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
    {
        try
        {
            Assert.Right(expectedRight, either);
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }

    /// <inheritdoc cref="AssertExtensions.extension(Assert).Right{TLeft, TRight}(Either{TLeft, TRight})"/>
    [AssertMethodHasOverloadWithExpectedValue]
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static TRight Right<TLeft, TRight>(Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
    {
        try
        {
            return Assert.Right(either);
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }
}
