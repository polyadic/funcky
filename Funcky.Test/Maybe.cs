using System;
using System.Collections.Generic;
using Funcky.Extensions;
using Xunit;

namespace Funcky.Test
{
    public class Maybe
    {
        enum MyEnum { None, Cool }

        [Theory]
        [InlineData(-12, "-12")]
        [InlineData(0, "0")]
        [InlineData(-953542, "-953542")]
        [InlineData(1337, "1337")]
        public void ParseIntViaMaybeMonadWhereStringIsNumber(int parsed, string stringToParse)
        {
            var maybe = stringToParse.TryParseInt();

            Assert.True(maybe.Match(false, m => true));
            Assert.Equal(parsed, maybe.Match(0, m => m));
        }

        [Fact]
        public void ParseIntViaMaybeMonadWhereStringIsNoNumber()
        {
            var maybe = "no number".TryParseInt();

            Maybe<bool> isLeet = maybe.Select(m => m == 1337);

            Assert.False(isLeet.Match(false, b => true));
        }

        [Fact]
        public void ParseDateViaMaybeMonad()
        {
            var maybe = "26.02.1982".TryParseDate();

            Assert.True(maybe.Match(false, m => true));
            Assert.Equal(new DateTime(1982, 2, 26), maybe.Match(DateTime.Now, m => m));
        }

        [Fact]
        public void ParseEnumViaMaybeMonad()
        {

            var maybe = "Cool".TryParseEnum<MyEnum>();

            Assert.True(maybe.Match(false, m => true));
            Assert.Equal(MyEnum.Cool, maybe.Match(MyEnum.None, m => m));
        }

        [Fact]
        public void ParseEnumViaMaybeMonadWhereValueIsInvalid()
        {

            var maybe = "NotCool".TryParseEnum<MyEnum>();

            Assert.False(maybe.Match(false, m => true));
        }

        [Fact]
        public void GetValueFromDictionaryViaMaybeMonad()
        {
            var dictionary = new Dictionary<string, string> { { "some", "value" } };

            var maybe = dictionary.TryGetValue("some");

            Assert.True(maybe.Match(false, m => true));
            Assert.Equal("value", maybe.Match("", m => m));
        }



        [Fact]
        public void GetValueFromDictionaryViaMaybeMonadWhereKeyDoesNotExist()
        {
            var dictionary = new Dictionary<string, string> { { "some", "value" } };

            var maybe = dictionary.TryGetValue("none");

            Assert.False(maybe.Match(false, m => true));
        }
    }
}
