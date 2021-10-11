using System.Diagnostics.Contracts;

namespace Funcky.Async
{
    public static partial class Functional
    {
        [Pure]
        public static Task NoOperationAsync()
            => Task.CompletedTask;

        [Pure]
        public static Task NoOperationAsync<T1>(T1 p1)
            => Task.CompletedTask;

        [Pure]
        public static Task NoOperationAsync<T1, T2>(T1 p1, T2 p2)
            => Task.CompletedTask;

        [Pure]
        public static Task NoOperationAsync<T1, T2, T3>(T1 p1, T2 p2, T3 p3)
            => Task.CompletedTask;

        [Pure]
        public static Task NoOperationAsync<T1, T2, T3, T4>(T1 p1, T2 p2, T3 p3, T4 p4)
            => Task.CompletedTask;

        [Pure]
        public static Task NoOperationAsync<T1, T2, T3, T4, T5>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5)
            => Task.CompletedTask;

        [Pure]
        public static Task NoOperationAsync<T1, T2, T3, T4, T5, T6>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6)
            => Task.CompletedTask;

        [Pure]
        public static Task NoOperationAsync<T1, T2, T3, T4, T5, T6, T7>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7)
            => Task.CompletedTask;

        [Pure]
        public static Task NoOperationAsync<T1, T2, T3, T4, T5, T6, T7, T8>(T1 p1, T2 p2, T3 p3, T4 p4, T5 p5, T6 p6, T7 p7, T8 p8)
            => Task.CompletedTask;
    }
}
