using System.Globalization;

namespace Funcky.Test.Extensions
{
    public sealed class ParseExtensionsTest
    {
        public ParseExtensionsTest()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-CH");
        }

        public enum MyEnum
        {
            Cool,
            Warp,
            FortyTwo,
        }

        [Theory]
        [MemberData(nameof(BooleanStrings))]
        public void GivenAStringParseBooleanOrNoneReturnsTheCorrectValue(Option<bool> expected, string input)
        {
            Assert.Equal(expected, input.ParseBooleanOrNone());
        }

        [Theory]
        [MemberData(nameof(ByteStrings))]
        public void GivenAStringParseByteOrNoneReturnsTheCorrectValue(Option<byte> expected, string input)
        {
            Assert.Equal(expected, input.ParseByteOrNone());
        }

        [Theory]
        [MemberData(nameof(ShortStrings))]
        public void GivenAStringParseShortOrNoneReturnsTheCorrectValue(Option<short> expected, string input)
        {
            Assert.Equal(expected, input.ParseShortOrNone());
        }

        [Theory]
        [MemberData(nameof(IntStrings))]
        public void GivenAStringParseIntOrNoneReturnsTheCorrectValue(Option<int> expected, string input)
        {
            Assert.Equal(expected, input.ParseIntOrNone());
        }

        [Theory]
        [MemberData(nameof(LongStrings))]
        public void GivenAStringParseLongOrNoneReturnsTheCorrectValue(Option<long> expected, string input)
        {
            Assert.Equal(expected, input.ParseLongOrNone());
        }

        [Theory]
        [MemberData(nameof(FloatStrings))]
        public void GivenAStringParseFloatOrNoneReturnsTheCorrectValue(Option<float> expected, string input)
        {
            Assert.Equal(expected, input.ParseFloatOrNone());
        }

        [Theory]
        [MemberData(nameof(DoubleStrings))]
        public void GivenAStringParseDoubleOrNoneReturnsTheCorrectValue(Option<double> expected, string input)
        {
            Assert.Equal(expected, input.ParseDoubleOrNone());
        }

        [Theory]
        [MemberData(nameof(DecimalStrings))]
        public void GivenAStringParseDecimalOrNoneReturnsTheCorrectValue(Option<decimal> expected, string input)
        {
            Assert.Equal(expected, input.ParseDecimalOrNone());
        }

        [Theory]
        [MemberData(nameof(DateTimeStrings))]
        public void GivenAStringParseDateTimeOrNoneReturnsTheCorrectValue(Option<DateTime> expected, string input)
        {
            using var current = new DisposableCulture("de-CH");

            Assert.Equal(expected, input.ParseDateTimeOrNone());
        }

        [Theory]
        [MemberData(nameof(EnumStrings))]
        public void GivenAStringParseEnumOrNoneReturnsTheCorrectValue(Option<MyEnum> expected, string input)
        {
            Assert.Equal(expected, input.ParseEnumOrNone<MyEnum>());
        }

        private static TheoryData<Option<bool>, string> BooleanStrings()
            => new()
            {
                { Option<bool>.None, string.Empty },
                { Option.Some(true), "true" },
                { Option.Some(false), "false" },
                { Option.Some(true), "TrUe" },
                { Option.Some(false), "FalsE" },
                { Option<bool>.None, "0" },
                { Option<bool>.None, "1" },
                { Option<bool>.None, "T" },
                { Option<bool>.None, "F" },
                { Option<bool>.None, "falsch" },
                { Option<bool>.None, "bool" },
                { Option<bool>.None, "none" },
            };

        private static TheoryData<Option<byte>, string> ByteStrings()
            => new()
            {
                { Option<byte>.None, string.Empty },
                { Option<byte>.None, "no number" },
                { Option.Some((byte)12), "+12" },
                { Option.Some((byte)42), "42" },
                { Option<byte>.None, "One" },
                { Option<byte>.None, "+-1" },
                { Option<byte>.None, "MCMI" },
                { Option<byte>.None, "2000" },
                { Option<byte>.None, "1337E+02" },
            };

        private static TheoryData<Option<short>, string> ShortStrings()
            => new()
            {
                { Option<short>.None, string.Empty },
                { Option<short>.None, "no number" },
                { Option.Some((short)-12), "-12" },
                { Option.Some((short)0), "0" },
                { Option.Some((short)-9542), "-9542" },
                { Option.Some((short)1337), "1337" },
                { Option<short>.None, "One" },
                { Option<short>.None, "+-1" },
                { Option<short>.None, "MCMI" },
                { Option<short>.None, "1337E+02" },
            };

        private static TheoryData<Option<int>, string> IntStrings()
            => new()
            {
                { Option<int>.None, string.Empty },
                { Option<int>.None, "no number" },
                { Option.Some(-12), "-12" },
                { Option.Some(0), "0" },
                { Option.Some(-953542), "-953542" },
                { Option.Some(1337), "1337" },
                { Option<int>.None, "One" },
                { Option<int>.None, "+-1" },
                { Option<int>.None, "MCMI" },
                { Option<int>.None, "1337E+02" },
            };

        private static TheoryData<Option<long>, string> LongStrings()
            => new()
            {
                { Option<long>.None, string.Empty },
                { Option<long>.None, "no number" },
                { Option.Some(-12L), "-12" },
                { Option.Some(0L), "0" },
                { Option.Some(-953542L), "-953542" },
                { Option.Some(1337L), "1337" },
                { Option<long>.None, "One" },
                { Option<long>.None, "+-1" },
                { Option<long>.None, "MCMI" },
                { Option<long>.None, "1337E+02" },
            };

        private static TheoryData<Option<float>, string> FloatStrings()
            => new()
            {
                { Option<float>.None, string.Empty },
                { Option<float>.None, "no number" },
                { Option.Some(-12.7f), "-12.7" },
                { Option.Some(0f), "0" },
                { Option.Some(-953542.999f), "-953542.999" },
                { Option.Some(1337f), "1337.00" },
                { Option.Some(13.37f), "1337E-02" },
                { Option<float>.None, "One" },
                { Option<float>.None, "+-1" },
                { Option<float>.None, "MCMI" },
                { Option<float>.None, "1337F" },
            };

        private static TheoryData<Option<double>, string> DoubleStrings()
            => new()
            {
                { Option<double>.None, string.Empty },
                { Option<double>.None, "no number" },
                { Option.Some(-12.7), "-12.7" },
                { Option.Some(0.0), "0" },
                { Option.Some(-953542.999), "-953542.999" },
                { Option.Some(1337.0), "1337.000" },
                { Option.Some(133700.0), "1337E+02" },
                { Option<double>.None, "One" },
                { Option<double>.None, "+-1" },
                { Option<double>.None, "MCMI" },
                { Option<double>.None, "1337D" },
            };

        private static TheoryData<Option<decimal>, string> DecimalStrings()
        => new()
            {
                { Option<decimal>.None, string.Empty },
                { Option<decimal>.None, "no number" },
                { Option.Some(-12.7m), "-12.7" },
                { Option.Some(0.0m), "0" },
                { Option.Some(-953542.999m), "-953542.999" },
                { Option.Some(1337m), "1337.000" },
                { Option<decimal>.None, "1337E+02" },
                { Option<decimal>.None, "One" },
                { Option<decimal>.None, "+-1" },
                { Option<decimal>.None, "MCMI" },
                { Option<decimal>.None, "1337M" },
            };

        private static TheoryData<Option<DateTime>, string> DateTimeStrings()
            => new()
            {
                { Option<DateTime>.None, string.Empty },
                { Option<DateTime>.None, "no number" },
                { Option.Some(new DateTime(1982, 2, 26)), "26.02.1982" },
                { Option.Some(new DateTime(2008, 11, 1, 19, 35, 0)), "Sat, 01 Nov 2008 19:35:00" },
                { Option<DateTime>.None, "One" },
                { Option<DateTime>.None, "+-1" },
                { Option<DateTime>.None, "MCMI" },
            };

        private static TheoryData<Option<MyEnum>, string> EnumStrings()
            => new()
            {
                { Option<MyEnum>.None, string.Empty },
                { Option.Some(MyEnum.Cool), "Cool" },
                { Option.Some(MyEnum.FortyTwo), "FortyTwo" },
                { Option.Some(MyEnum.Warp), "Warp" },
                { Option<MyEnum>.None, "NotCool" },
                { Option<MyEnum>.None, "WarpCool" },
                { Option<MyEnum>.None, "MyEnum.Cool" },
                { Option<MyEnum>.None, "fortytwo" },
            };
    }
}
