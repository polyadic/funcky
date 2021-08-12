namespace Funcky.Internal
{
    internal class HandleException<TException>
        where TException : Exception
    {
        public static Option<TResult> ToNone<TResult>(Func<TResult> func)
            where TResult : notnull
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
