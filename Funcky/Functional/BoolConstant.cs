namespace Funcky
{
    public static partial class Functional
    {
        /// <summary>A function that always returns <see langword="true"/>.</summary>
        [Pure]
        public static bool True() => true;

        /// <inheritdoc cref="True"/>
        [Pure]
        public static bool True<T1>(T1 ω1) => true;

        /// <inheritdoc cref="True"/>
        [Pure]
        public static bool True<T1, T2>(T1 ω1, T2 ω2) => true;

        /// <inheritdoc cref="True"/>
        [Pure]
        public static bool True<T1, T2, T3>(T1 ω1, T2 ω2, T3 ω3) => true;

        /// <inheritdoc cref="True"/>
        [Pure]
        public static bool True<T1, T2, T3, T4>(T1 ω1, T2 ω2, T3 ω3, T4 ω4) => true;

        /// <summary>A function that always returns <see langword="false"/>.</summary>
        [Pure]
        public static bool False() => false;

        /// <inheritdoc cref="False"/>
        [Pure]
        public static bool False<T1>(T1 ω1) => false;

        /// <inheritdoc cref="False"/>
        [Pure]
        public static bool False<T1, T2>(T1 ω1, T2 ω2) => false;

        /// <inheritdoc cref="False"/>
        [Pure]
        public static bool False<T1, T2, T3>(T1 ω1, T2 ω2, T3 ω3) => false;

        /// <inheritdoc cref="False"/>
        [Pure]
        public static bool False<T1, T2, T3, T4>(T1 ω1, T2 ω2, T3 ω3, T4 ω4) => false;
    }
}
