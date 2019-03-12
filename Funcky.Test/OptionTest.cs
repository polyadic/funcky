using System;
using System.Collections.Generic;
using System.Linq;
using Funcky.Extensions;
using Funcky.Monads;
using Xunit;

namespace Funcky.Test
{
    public class OptionsTest
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

            bool isSome = maybe.Match(
                none: false,
                some: m => true
                );


            Assert.True(isSome);
            Assert.Equal(parsed, maybe.Match(0, m => m));
        }

        [Fact]
        public void CreateMaybeWithTypeInference()
        {
            var maybe = Option.Some(1337);

            Assert.Equal(typeof(int), maybe.GetType().GetGenericArguments().First());
        }

        [Fact]
        public void CreateNone()
        {
            var none = Option<int>.None();

            Assert.Equal(typeof(int), none.GetType().GetGenericArguments().First());
        }

        [Fact]
        public void CreateSome()
        {
            var some = Option.Some(42);

            Assert.Equal(typeof(int), some.GetType().GetGenericArguments().First());
        }

        [Fact]
        public void ParseIntViaMaybeMonadWhereStringIsNoNumber()
        {
            var maybe = "no number".TryParseInt();

            Assert.False(maybe.Match(false, m => true));
        }

        [Fact]
        public void ParseDateViaMaybeMonad()
        {
            var maybe = "26.02.1982".TryParseDateTime();

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
            Assert.Equal(MyEnum.None, maybe.Match(MyEnum.None, m => m));
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

        [Fact]
        public void SupportingLinqSyntaxSelect()
        {
            Option<int> maybe = Option<int>.None();
            Option<bool> maybeBool =
                from m in maybe
                select m == 1337;

            Assert.False(maybeBool.Match(false, m => true));
        }

        [Fact]
        public void MaybeMonadShouldSupportSelectMany()
        {
            Option<int> someNumber = "1337".TryParseInt();
            Option<DateTime> someDate = "12.2.2009".TryParseDateTime();

            Option<Tuple<int, DateTime>> result = someNumber.SelectMany(number => someDate, Tuple.Create);

            var resultShouldBe = new Option<Tuple<int, DateTime>>(new Tuple<int, DateTime>(1337, new DateTime(2009, 2, 12)));
            Assert.Equal(resultShouldBe, result);
        }


        [Fact]
        public void MaybeSupportingLinqSyntaxSelectMany()
        {
            Option<int> someNumber = "1337".TryParseInt();
            Option<DateTime> someDate = "12.2.2009".TryParseDateTime();

            var result = from number in someNumber
                         from date in someDate
                         select Tuple.Create(number, date);

            var resultShouldBe = new Option<Tuple<int, DateTime>>(new Tuple<int, DateTime>(1337, new DateTime(2009, 2, 12)));
            Assert.Equal(resultShouldBe, result);
        }

        [Fact]
        public void SelectManyWithOneNoneInputShouldResultInNoneResult()
        {
            Option<int> someNumber = "1337".TryParseInt();
            Option<DateTime> someDate = "12.2.2009".TryParseDateTime();
            Option<int> someOtherNumber = "not a number".TryParseInt();

            var result = from number in someNumber
                         from date in someDate
                         from otherNumber in someOtherNumber
                         select Tuple.Create(number, date, otherNumber);

            Assert.False(result.Match(false, t => true));
        }

        [Fact]
        public void FilterAllNonesFromAMaybeEnumerable()
        {
            var input = "123,some,x,1337,42,1,1000";

            foreach (var number in input.Split(",").Select(ParseExtensions.TryParseInt).Where(maybeInt => maybeInt.Match(false, i => true)))
            {
                var value = number.Match(
                    none: 0,
                    some: i => i
                    );

                Assert.NotEqual(0, value);
            }
        }
    }
}
