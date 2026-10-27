using System.Diagnostics.CodeAnalysis;
using Funcky.CodeAnalysis;
using Xunit;
using Xunit.Sdk;

namespace Funcky.Extensions;

/// <summary>The assertions of <see cref="FunctionalAssert"/> as static extension members of xunit's <see cref="Assert"/>,
/// so they are found where the other assertions live: <c>Assert.Some(option)</c>.</summary>
public static class AssertExtensions
{
    extension(Assert)
    {
        /// <inheritdoc cref="FunctionalAssert.Some{TItem}(TItem, Option{TItem})"/>
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
                FunctionalAssert.Some(expectedValue, option);
            }
            catch (XunitException exception)
            {
                throw exception;
            }
        }

        /// <inheritdoc cref="FunctionalAssert.Some{TItem}(Option{TItem})"/>
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
                return FunctionalAssert.Some(option);
            }
            catch (XunitException exception)
            {
                throw exception;
            }
        }

        /// <inheritdoc cref="FunctionalAssert.None{TItem}(Option{TItem})"/>
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
                FunctionalAssert.None(option);
            }
            catch (XunitException exception)
            {
                throw exception;
            }
        }

        /// <inheritdoc cref="FunctionalAssert.Ok{TValidResult}(TValidResult, Result{TValidResult})"/>
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
                FunctionalAssert.Ok(expectedResult, result);
            }
            catch (XunitException exception)
            {
                throw exception;
            }
        }

        /// <inheritdoc cref="FunctionalAssert.Ok{TValidResult}(Result{TValidResult})"/>
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
                return FunctionalAssert.Ok(result);
            }
            catch (XunitException exception)
            {
                throw exception;
            }
        }

        /// <inheritdoc cref="FunctionalAssert.Error{TValidResult}(Result{TValidResult})"/>
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
                return FunctionalAssert.Error(result);
            }
            catch (XunitException exception)
            {
                throw exception;
            }
        }

        /// <inheritdoc cref="FunctionalAssert.Error{TException, TValidResult}(Result{TValidResult})"/>
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
                return FunctionalAssert.Error<TException, TValidResult>(result);
            }
            catch (XunitException exception)
            {
                throw exception;
            }
        }

        /// <inheritdoc cref="FunctionalAssert.Left{TLeft, TRight}(TLeft, Either{TLeft, TRight})"/>
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
                FunctionalAssert.Left(expectedLeft, either);
            }
            catch (XunitException exception)
            {
                throw exception;
            }
        }

        /// <inheritdoc cref="FunctionalAssert.Left{TLeft, TRight}(Either{TLeft, TRight})"/>
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
                return FunctionalAssert.Left(either);
            }
            catch (XunitException exception)
            {
                throw exception;
            }
        }

        /// <inheritdoc cref="FunctionalAssert.Right{TLeft, TRight}(TRight, Either{TLeft, TRight})"/>
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
                FunctionalAssert.Right(expectedRight, either);
            }
            catch (XunitException exception)
            {
                throw exception;
            }
        }

        /// <inheritdoc cref="FunctionalAssert.Right{TLeft, TRight}(Either{TLeft, TRight})"/>
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
                return FunctionalAssert.Right(either);
            }
            catch (XunitException exception)
            {
                throw exception;
            }
        }
    }
}
