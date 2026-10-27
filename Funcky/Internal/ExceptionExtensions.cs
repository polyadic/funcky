namespace Funcky.Internal;

internal static class ExceptionExtensions
{
    extension(Exception exception)
    {
        /// <summary>Whether the exception signals a corrupted process rather than a failed operation. Such exceptions
        /// are never turned into a value; every <c>catch</c> in Funcky that turns exceptions into values lets them through.</summary>
        public bool IsFatal
            => exception is OutOfMemoryException or StackOverflowException or AccessViolationException or ThreadAbortException;
    }
}
