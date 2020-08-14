using System;
using Funcky.Monads;

namespace Funcky
{
    public static partial class Functional
    {
        /// <summary>
        /// Calls the given <paramref name="producer"/> over and over until it returns a value.
        /// </summary>
        public static TResult Retry<TResult>(Func<Option<TResult>> producer)
            where TResult : notnull
            => producer().GetOrElse(() => Retry(producer));
    }
}
