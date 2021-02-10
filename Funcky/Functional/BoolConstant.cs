using System.Diagnostics.Contracts;

namespace Funcky
{
    public static partial class Functional
    {
        /// <summary>A function that always returns <see langword="true"/>.</summary>
        [Pure]
        public static bool True() => true;

        /// <inheritdoc cref="True{T}"/>
        [Pure]
        public static bool True<T>(T ω) => true;

        /// <inheritdoc cref="True{T}"/>
        [Pure]
        public static bool True<T0, T1>(T0 ω0, T1 ω1) => true;

        /// <summary>A function that always returns <see langword="false"/>.</summary>
        [Pure]
        public static bool False() => false;

        /// <inheritdoc cref="False{T}"/>
        [Pure]
        public static bool False<T>(T ω) => false;

        /// <inheritdoc cref="False{T}"/>
        [Pure]
        public static bool False<T0, T1>(T0 ω0, T1 ω1) => false;
    }
}
