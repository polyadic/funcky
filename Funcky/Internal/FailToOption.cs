using System.Diagnostics.CodeAnalysis;

namespace Funcky.Internal
{
    internal static class FailToOption<TResult>
        where TResult : notnull
    {
        public delegate bool TryDelegate([MaybeNullWhen(false)] out TResult result);

        public delegate bool TryDelegate<TInput>(TInput input, [MaybeNullWhen(false)] out TResult result);

        public delegate bool TryDelegate<TInput, T1>(TInput input, T1 p1, [MaybeNullWhen(false)] out TResult result);

        public delegate bool TryDelegate<TInput, T1, T2>(TInput input, T1 p1, T2 p2, [MaybeNullWhen(false)] out TResult result);

        public static Option<TResult> FromTryPattern(TryDelegate @try)
            => @try(out TResult? result)
                ? result
                : Option<TResult>.None();

        public static Option<TResult> FromTryPattern<TInput>(TryDelegate<TInput> @try, TInput input)
            => @try(input, out TResult? result)
                ? result
                : Option<TResult>.None();

        public static Option<TResult> FromTryPattern<TInput, T1>(TryDelegate<TInput, T1> @try, TInput input, T1 value1)
            => @try(input, value1, out TResult? result)
                ? result
                : Option<TResult>.None();

        public static Option<TResult> FromTryPattern<TInput, T1, T2>(TryDelegate<TInput, T1, T2> @try, TInput input, T1 value1, T2 value2)
            => @try(input, value1, value2, out TResult? result)
                ? result
                : Option<TResult>.None();

        public static Option<TResult> FromException<TException>(Func<TResult> func)
            where TException : Exception
        {
            try
            {
                return func();
            }
            catch (TException)
            {
                return Option<TResult>.None();
            }
        }
    }
}
