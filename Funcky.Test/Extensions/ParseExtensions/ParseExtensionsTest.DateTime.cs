using System;
using FsCheck;
using FsCheck.Xunit;
using Funcky.Extensions;
using Funcky.Monads;
using Funcky.Xunit;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test.Extensions
{
    public sealed partial class ParseExtensionsTest
    {
        [Theory]
        [MemberData(nameof(DateTimeStrings))]
        public void GivenAStringParseDateTimeOrNoneReturnsTheCorrectValue(Option<DateTime> expected, string? input)
        {
            using var current = new DisposableCulture("de-CH");

            Assert.Equal(expected, input!.ParseDateTimeOrNone());
        }

        [Theory]
        [MemberData(nameof(DateTimeStrings))]
        public void TheParseOrNoneExtensionReturnTheSameValueAsTheTryParseMethods(Option<DateTime> expected, string? input)
        {
            using var current = new DisposableCulture("de-CH");

            if (DateTime.TryParse(input, out var dateTime))
            {
                Assert.Equal(dateTime, FunctionalAssert.IsSome(input!.ParseDateTimeOrNone()));
            }
            else
            {
                FunctionalAssert.IsNone(input!.ParseDateTimeOrNone());
            }
        }

        [Property]
        public Property ParseDateTimeOrNoneReturnsTheSameAsTryParseForValidStrings(DateTime dateTime)
        {
            using var current = new DisposableCulture("de-CH");
            var input = dateTime.ToString();

            if (DateTime.TryParse(input, out var expected))
            {
                return input
                    .ParseDateTimeOrNone()
                    .Match(none: false, some: parsed => parsed == expected)
                    .ToProperty();
            }
            else
            {
                return input
                    .ParseDateTimeOrNone()
                    .Match(none: true, some: False)
                    .ToProperty();
            }
        }

        private static TheoryData<Option<DateTime>, string?> DateTimeStrings()
            => new()
            {
                { Option<DateTime>.None(), null },
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
