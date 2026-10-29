using System.Diagnostics.CodeAnalysis;
using Funcky.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace Funcky;

public static partial class FunctionalAssert
{
    /// <inheritdoc cref="AssertExtensions.extension(Assert).Left{TLeft, TRight}(TLeft, Either{TLeft, TRight})"/>
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static void Left<TLeft, TRight>(TLeft expectedLeft, Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
    {
        try
        {
            Assert.Left(expectedLeft, either);
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }

    /// <inheritdoc cref="AssertExtensions.extension(Assert).Left{TLeft, TRight}(Either{TLeft, TRight})"/>
    [AssertMethodHasOverloadWithExpectedValue]
    #if STACK_TRACE_HIDDEN_SUPPORTED
    [System.Diagnostics.StackTraceHidden]
    #endif
    [SuppressMessage("Microsoft.Usage", "CA2200", Justification = "Stack trace erasure intentional.")]
    [SuppressMessage("ReSharper", "PossibleIntendedRethrow", Justification = "Stack trace erasure intentional.")]
    public static TLeft Left<TLeft, TRight>(Either<TLeft, TRight> either)
        where TLeft : notnull
        where TRight : notnull
    {
        try
        {
            return Assert.Left(either);
        }
        catch (XunitException exception)
        {
            throw exception;
        }
    }
}
