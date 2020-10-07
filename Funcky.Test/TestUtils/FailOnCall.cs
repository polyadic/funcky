using System;
using Xunit.Sdk;

namespace Funcky.Test.TestUtils
{
    internal static class FailOnCall
    {
        internal static TResult Function<TResult>()
            => throw new XunitException("Function was unexpectedly called.");

        internal static TResult Function<T1, TResult>(T1 p1)
            => throw new XunitException("Function was unexpectedly called.");

        internal static TResult Function<T1, T2, TResult>(T1 p1, T2 p2)
            => throw new XunitException("Function was unexpectedly called.");

        internal static TResult Function<T1, T2, T3, TResult>(T1 p1, T2 p2, T3 p3)
            => throw new XunitException("Function was unexpectedly called.");
    }
}
