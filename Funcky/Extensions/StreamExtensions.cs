using System;
using System.IO;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>Gets the length in bytes of the stream.</summary>
        /// <returns>A long value representing the length of the stream in bytes or None if the stream does not support seeking.</returns>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public static Option<long> GetLengthOrNone(this Stream stream)
            => GetSupportedPropertyOrNone<long, NotSupportedException>(() => stream.Length);

        /// <summary>Gets the position within the current stream.</summary>
        /// <returns>The current position within the stream or None if the stream does not support seeking.</returns>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public static Option<long> GetPositionOrNone(this Stream stream)
            => GetSupportedPropertyOrNone<long, NotSupportedException>(() => stream.Position);

        /// <summary>Gets a value, in milliseconds, that determines how long the stream will attempt to read before timing out.</summary>
        /// <returns>A value, in milliseconds, that determines how long the stream will attempt to read before timing out or None if the stream does not support timing out.</returns>
        public static Option<int> GetReadTimeoutOrNone(this Stream stream)
            => GetSupportedPropertyOrNone<int, InvalidOperationException>(() => stream.ReadTimeout);

        /// <summary>Gets a value, in milliseconds, that determines how long the stream will attempt to write before timing out.</summary>
        /// <returns>A value, in milliseconds, that determines how long the stream will attempt to write before timing out or None if the stream does not support timing out.</returns>
        public static Option<int> GetWriteTimeoutOrNone(this Stream stream)
            => GetSupportedPropertyOrNone<int, InvalidOperationException>(() => stream.WriteTimeout);

        private static Option<TResult> GetSupportedPropertyOrNone<TResult, TException>(Func<TResult> func)
            where TResult : notnull
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
