using System;
using Funcky.Extensions;
using Funcky.Monads;
using Funcky.Xunit;
using Xunit;

namespace Funcky.Test.Extensions
{
    public sealed partial class ParseExtensionsTest
    {
        [Theory]
        [MemberData(nameof(DateTimeStrings))]
        public void GivenAStringParseDateTimeOrNoneReturnsTheCorrectValue(Option<DateTime> expected, string input)
        {
            using var current = new DisposableCulture("de-CH");

            Assert.Equal(expected, input.ParseDateTimeOrNone());
        }

        [Theory]
        [MemberData(nameof(DateTimeStrings))]
        public void TheParseOrNoneExtensionReturnTheSameValueAsTheTryParseMethods(string input)
        {
            using var current = new DisposableCulture("de-CH");

            if (DateTime.TryParse(input, out var dateTime))
            {
                Assert.Equal(dateTime, FunctionalAssert.IsSome(input.ParseDateTimeOrNone()));
            }
            else
            {
                FunctionalAssert.IsNone(input.ParseDateTimeOrNone());
            }
        }

        private static TheoryData<Option<DateTime>, string> DateTimeStrings()
            => new()
            {
                { Option<DateTime>.None(), string.Empty },
                { Option<DateTime>.None(), "no number" },
                { Option.Some(new DateTime(1982, 2, 26)), "26.02.1982" },
                { Option.Some(new DateTime(2008, 11, 1, 19, 35, 0)), "Sat, 01 Nov 2008 19:35:00" },
                { Option<DateTime>.None(), "One" },
                { Option<DateTime>.None(), "+-1" },
                { Option<DateTime>.None(), "MCMI" },
            };
    }
}
