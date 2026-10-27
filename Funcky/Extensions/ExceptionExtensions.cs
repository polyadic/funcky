namespace Funcky.Extensions;

public static class ExceptionExtensions
{
    extension(Exception exception)
    {
        /// <summary>The exception that caused this one, or <c>None</c> when there is none, where
        /// <see cref="Exception.InnerException"/> is <see langword="null"/>.</summary>
        public Option<Exception> InnerExceptionOrNone
            => Option.FromNullable(exception.InnerException);
    }
}
