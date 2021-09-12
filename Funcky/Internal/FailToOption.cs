namespace Funcky.Internal
{
    internal static class FailToOption<TResult>
        where TResult : notnull
    {
        public static Option<TResult> FromException<TException>(Func<TResult> func)
            where TException : Exception
        {
            try
            {
                return func();
            }
            catch (TException)
            {
                return Option<TResult>.None;
            }
        }
    }
}
