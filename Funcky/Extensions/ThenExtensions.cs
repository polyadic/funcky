using System;

#nullable enable

namespace Funcky.Extensions
{
    public static class ThenExtensions
    {
        public static TResult Then<TInput, TResult>(this TInput value, Func<TInput, TResult> func)
            => func(value);
    }
}
