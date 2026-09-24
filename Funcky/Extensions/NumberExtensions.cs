#if GENERIC_MATH
using System.Numerics;

namespace Funcky.Extensions;

public static class NumberExtensions
{
    extension(byte number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(byte from, byte to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<byte, TFrom, TTo>(number, from, to);
    }

    extension(sbyte number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(sbyte from, sbyte to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<sbyte, TFrom, TTo>(number, from, to);
    }

    extension(short number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(short from, short to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<short, TFrom, TTo>(number, from, to);
    }

    extension(ushort number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(ushort from, ushort to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<ushort, TFrom, TTo>(number, from, to);
    }

    extension(int number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(int from, int to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<int, TFrom, TTo>(number, from, to);
    }

    extension(uint number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(uint from, uint to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<uint, TFrom, TTo>(number, from, to);
    }

    extension(long number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(long from, long to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<long, TFrom, TTo>(number, from, to);
    }

    extension(ulong number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(ulong from, ulong to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<ulong, TFrom, TTo>(number, from, to);
    }

    extension(nint number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(nint from, nint to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<nint, TFrom, TTo>(number, from, to);
    }

    extension(nuint number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(nuint from, nuint to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<nuint, TFrom, TTo>(number, from, to);
    }

    extension(Int128 number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(Int128 from, Int128 to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<Int128, TFrom, TTo>(number, from, to);
    }

    extension(UInt128 number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(UInt128 from, UInt128 to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<UInt128, TFrom, TTo>(number, from, to);
    }

    extension(Half number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(Half from, Half to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<Half, TFrom, TTo>(number, from, to);
    }

    extension(float number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(float from, float to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<float, TFrom, TTo>(number, from, to);
    }

    extension(double number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(double from, double to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<double, TFrom, TTo>(number, from, to);
    }

    extension(decimal number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(decimal from, decimal to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<decimal, TFrom, TTo>(number, from, to);
    }

    extension(char number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(char from, char to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<char, TFrom, TTo>(number, from, to);
    }

    extension(BigInteger number)
    {
        /// <inheritdoc cref="InRange{TNumber, TFrom, TTo}(TNumber, TNumber, TNumber)"/>
        [Pure]
        public bool InRange<TFrom, TTo>(BigInteger from, BigInteger to)
            where TFrom : IIntervalBoundary
            where TTo : IIntervalBoundary
            => InRange<BigInteger, TFrom, TTo>(number, from, to);
    }

    /// <summary>
    /// Checks whether <paramref name="number"/> lies within the interval given by <paramref name="from"/> and <paramref name="to"/>.
    /// The type arguments state whether each boundary is included (<see cref="Including"/>) or excluded (<see cref="Excluding"/>).
    /// The boundaries may be given in any order.
    /// </summary>
    /// <example>
    /// <code>
    /// 12.InRange&lt;Including, Excluding&gt;(0, 20); // true
    /// 20.InRange&lt;Including, Excluding&gt;(0, 20); // false
    /// </code>
    /// </example>
    [Pure]
    public static bool InRange<TNumber, TFrom, TTo>(TNumber number, TNumber from, TNumber to)
        where TNumber : IComparisonOperators<TNumber, TNumber, bool>
        where TFrom : IIntervalBoundary
        where TTo : IIntervalBoundary
        => from < to
            ? TFrom.IsLowerBoundOf(from, number) && TTo.IsUpperBoundOf(to, number)
            : TTo.IsLowerBoundOf(to, number) && TFrom.IsUpperBoundOf(from, number);
}
#endif
