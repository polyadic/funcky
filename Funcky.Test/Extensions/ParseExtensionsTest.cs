using System;
using System.Globalization;
using System.Threading;
using Funcky.Extensions;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test.Extensions
{
    public sealed class ParseExtensionsTest
    {
        public ParseExtensionsTest()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-CH");
        }

        private enum MyEnum
        {
            None,
            Cool,
        }

        [Theory]
        [InlineData(-12, "-12")]
        [InlineData(0, "0")]
        [InlineData(-953542, "-953542")]
        [InlineData(1337, "1337")]
        public void GivenStringsThenTheTryParseIntFunctionReturnsSomeForNumbers(int parsed, string stringToParse)
        {
            var maybe = stringToParse.TryParseInt();

            var isSome = maybe.Match(
                none: false,
                some: True);

            Assert.True(isSome);
            Assert.Equal(parsed, maybe.Match(0, Identity));
        }

        [Fact]
        public void GivenAStringWhichIsNotAnIntThenTryParseIntReturnsANoneValue()
        {
            var maybe = "no number".TryParseInt();

            Assert.Equal(default, maybe);
        }

        [Fact]
        public void GivenADateThenTryParseDateReturnsAnOptionOfDate()
        {
            var maybe = "26.02.1982".TryParseDateTime();

            Assert.True(maybe.Match(false, True));
            Assert.Equal(new DateTime(1982, 2, 26), maybe.Match(DateTime.Now, Identity));
        }

        [Fact]
        public void GivenAnEnumThenTryParseEnumReturnsAnOptionOfDate()
        {
            var maybe = "Cool".TryParseEnum<MyEnum>();

            Assert.True(maybe.Match(false, True));
            Assert.Equal(MyEnum.Cool, maybe.Match(MyEnum.None, Identity));
        }

        [Fact]
        public void GivenAnInvalidEnumValueThenTryParseEnumReturnsANone()
        {
            var maybe = "NotCool".TryParseEnum<MyEnum>();

            Assert.False(maybe.Match(false, True));
            Assert.Equal(MyEnum.None, maybe.Match(MyEnum.None, Identity));
        }
    }
}
