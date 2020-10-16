using System;
using System.Globalization;
using System.Threading;
using Funcky.Extensions;
using Funcky.Xunit;
using Xunit;

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

            FunctionalAssert.IsSome(parsed, maybe);
        }

        [Fact]
        public void GivenAStringWhichIsNotAnIntThenTryParseIntReturnsANoneValue()
        {
            var maybe = "no number".TryParseInt();

            FunctionalAssert.IsNone(maybe);
        }

        [Fact]
        public void GivenADateThenTryParseDateReturnsAnOptionOfDate()
        {
            var maybe = "26.02.1982".TryParseDateTime();

            FunctionalAssert.IsSome(new DateTime(1982, 2, 26), maybe);
        }

        [Fact]
        public void GivenAnEnumThenTryParseEnumReturnsAnOptionOfDate()
        {
            var maybe = "Cool".TryParseEnum<MyEnum>();

            FunctionalAssert.IsSome(MyEnum.Cool, maybe);
        }

        [Fact]
        public void GivenAnInvalidEnumValueThenTryParseEnumReturnsANone()
        {
            var maybe = "NotCool".TryParseEnum<MyEnum>();

            FunctionalAssert.IsNone(maybe);
        }
    }
}
