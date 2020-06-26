using System;
using System.IO;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>Gets the length in bytes of the stream.</summary>
        /// <returns>A long value representing the length of the stream in bytes or None if the <see cref="Stream"/> does not support seeking.</returns>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public static Option<long> GetLengthOrNone(this Stream stream)
            => GetSupportedPropertyOrNone(() => stream.Length);

        /// <summary>Gets or sets a value, in milliseconds, that determines how long the stream will attempt to read before timing out.</summary>
        /// <returns>A value, in milliseconds, that determines how long the stream will attempt to read before timing out.</returns>
        public static Option<int> GetReadTimeoutOrNone(this Stream stream)
            => GetSupportedPropertyOrNone(() => stream.ReadTimeout);

        /// <summary>Gets or sets a value, in milliseconds, that determines how long the stream will attempt to write before timing out.</summary>
        /// <returns>A value, in milliseconds, that determines how long the stream will attempt to write before timing out.</returns>
        public static Option<int> GetWriteTimeoutOrNone(this Stream stream)
            => GetSupportedPropertyOrNone(() => stream.WriteTimeout);

        private static Option<TResult> GetSupportedPropertyOrNone<TResult>(Func<TResult> func)
            where TResult : notnull
        {
            try
            {
                return Option.Some(func());
            }
            catch (NotSupportedException)
            {
                return Option<TResult>.None();
            }
        }
    }
}
