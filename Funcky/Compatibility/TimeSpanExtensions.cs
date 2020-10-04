#if !TIMESPAN_MULTIPLY_SUPPORTED

namespace System
{
    // Implementation adapted from:
    // https://github.com/dotnet/runtime/blob/6072e4d3a7a2a1493f514cdf4be75a3d56580e84/src/libraries/System.Private.CoreLib/src/System/TimeSpan.cs
    internal static class TimeSpanExtensions
    {
        private const string TimeSpanTooLong = "TimeSpan overflowed because the duration is too long";

        private const string ArgumentCannotBeNaN = "TimeSpan does not accept floating point Not-a-Number values";

        public static TimeSpan Multiply(this TimeSpan timeSpan, double factor)
        {
            if (double.IsNaN(factor))
            {
                throw new ArgumentException(ArgumentCannotBeNaN, nameof(factor));
            }

            // Rounding to the nearest tick is as close to the result we would have with unlimited
            // precision as possible, and so likely to have the least potential to surprise.
            var ticks = Math.Round(timeSpan.Ticks * factor);
            return IntervalFromDoubleTicks(ticks);
        }

        private static TimeSpan IntervalFromDoubleTicks(double ticks)
            => ticks switch
            {
                _ when ticks > long.MaxValue || ticks < long.MinValue => throw new OverflowException(TimeSpanTooLong),
                _ when ticks == long.MaxValue => TimeSpan.MaxValue,
                _ => new TimeSpan((long)ticks),
            };
    }
}
#endif
