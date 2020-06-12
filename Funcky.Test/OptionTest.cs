using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Funcky.Extensions;
using Funcky.Monads;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test
{
    public class OptionTest
    {
        public OptionTest()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("de-CH");
        }

        private enum MyEnum
        {
            None,
            Cool,
        }

        [Fact]
        public void GivenAValueThenCreateMaybeWithTypeInference()
        {
            var maybe = Option.Some(1337);

            Assert.Equal(typeof(int), maybe.GetType().GetGenericArguments().First());
        }

        [Fact]
        public void GivenTheTypeConstructorNoneThenCreateNone()
        {
            var none = Option<int>.None();

            Assert.Equal(typeof(int), none.GetType().GetGenericArguments().First());
        }

        [Fact]
        public void GivenTheTypeConstructorSomeThenCreateSome()
        {
            var some = Option.Some(42);

            Assert.Equal(typeof(int), some.GetType().GetGenericArguments().First());
        }

        [Fact]
        public void GivenADictionaryWhenWeLookForAnExistentValueWithTryGetValueThenTheResultShouldBeASomeOfTheGivenType()
        {
            var dictionary = new Dictionary<string, string> { { "some", "value" } };

            var maybe = dictionary.TryGetValue(key: "some");

            Assert.True(maybe.Match(false, True));
            Assert.Equal("value", maybe.Match(string.Empty, m => m));
        }

        [Fact]
        public void GivenADictionaryWhenWeLookForAnInexistentValueWithTryGetValueThenTheResultShouldBeANoneOfTheGivenType()
        {
            var dictionary = new Dictionary<string, string> { { "some", "value" } };

            var maybe = dictionary.TryGetValue(readOnlyKey: "none");

            Assert.False(maybe.Match(false, True));
        }

        [Fact]
        public void GiveAValueNoneWithLinqSyntaxSelectThenTheResultShouldBeNone()
        {
            var maybe = Option<int>.None();
            var maybeBool =
                from m in maybe
                select m == 1337;

            Assert.False(maybeBool.Match(false, True));
        }

        [Fact]
        public void GivenTwoSomeValuesWithASelectManyWenWrittenInMethodSyntaxThenTheResultShouldBeSomeValue()
        {
            var someNumber = "1337".TryParseInt();
            var someDate = "12.2.2009".TryParseDateTime();

            var result = someNumber.SelectMany(number => someDate, Tuple.Create);

            var resultShouldBe = Option.Some(new Tuple<int, DateTime>(1337, new DateTime(2009, 2, 12)));
            Assert.Equal(resultShouldBe, result);
        }

        [Fact]
        public void GivenTwoSomeValuesWithASelectManyWenWrittenInLinqSyntacThenTheResultShouldBeSomeValue()
        {
            var someNumber = "1337".TryParseInt();
            var someDate = "12.2.2009".TryParseDateTime();

            var result = from number in someNumber
                         from date in someDate
                         select Tuple.Create(number, date);

            var resultShouldBe = Option.Some(new Tuple<int, DateTime>(1337, new DateTime(2009, 2, 12)));
            Assert.Equal(resultShouldBe, result);
        }

        [Fact]
        public void GivenASelectManyWithOneNoneInputThenTheResultShouldBeNone()
        {
            var someNumber = "1337".TryParseInt();
            var someDate = "12.2.2009".TryParseDateTime();
            var someOtherNumber = "not a number".TryParseInt();

            var result = from number in someNumber
                         from date in someDate
                         from otherNumber in someOtherNumber
                         select Tuple.Create(number, date, otherNumber);

            Assert.False(result.Match(false, True));
        }

        [Fact]
        public void GivenAFilterWhichFiltersAllNoneThenOnlyTheSomeValuesPassThrough()
        {
            var input = "123,some,x,1337,42,1,1000";

            foreach (var number in input.Split(",").Select(ParseExtensions.TryParseInt).Where(maybeInt => maybeInt.Match(false, True)))
            {
                var value = number.Match(
                    none: 0,
                    some: i => i);

                Assert.NotEqual(0, value);
            }
        }

        public static IEnumerable<object[]> TestValues()
        {
            yield return new object[] { "Some(1337)", Option.Some(1337) };
            yield return new object[] { "Some(-1)", Option.Some(-1) };
            yield return new object[] { "Some(10000)", Option.Some(10E3) };
            yield return new object[] { "Some(string)", Option.Some("string") };
            yield return new object[] { "None", Option<int>.None() };
            yield return new object[] { "None", Option<string>.None() };
            yield return new object[] { "Some(Cool)", Option.Some(MyEnum.Cool) };
            yield return new object[] { "None", Option<MyEnum>.None() };
        }

        [Theory]
        [MemberData(nameof(WhereTestValues))]
        public void WhereFiltersOptionCorrectly(Option<string> expectedResult, Option<string> input, Func<string, bool> predicate)
        {
            Assert.Equal(expectedResult, input.Where(predicate));
        }

        [Theory]
        [MemberData(nameof(WhereTestValues))]
        public void OptionSupportsLinqWhereSyntax(Option<string> expectedResult, Option<string> input, Func<string, bool> predicate)
        {
            var result = from x in input
                where predicate(x)
                select x;
            Assert.Equal(expectedResult, result);
        }

        public static TheoryData<Option<string>, Option<string>, Func<string, bool>> WhereTestValues()
            => new TheoryData<Option<string>, Option<string>, Func<string, bool>>
            {
                { Option.Some("foo"), Option.Some("foo"), True },
                { Option<string>.None(), Option.Some("foo"), False },
                { Option<string>.None(), Option<string>.None(), True },
                { Option<string>.None(), Option<string>.None(), False },
            };

        [Theory]
        [MemberData(nameof(TestValues))]

        public void GivenAnOptionObjectThenToStringReturnsUsefulString(string reference, IToString option)
        {
            Assert.Equal(reference, option.ToString());
        }

        [Fact]
        public void MatchOverloadWithTwoFuncObjectsWorksCorrectly()
        {
            var input = "123,some,x,1337,42,1,1000";

            foreach (var number in input.Split(",").Select(ParseExtensions.TryParseInt).Where(maybeInt => maybeInt.Match(false, True)))
            {
                var value = number.Match(
                    none: () => 0,
                    some: i => i);

                Assert.NotEqual(0, value);
            }
        }

        [Fact]
        public void GivenAMatchingStatementWhichThrowsAnExceptionThrowsTheSameException()
        {
            var none = Option<int>.None();

            Assert.Throws<ArgumentNullException>(testCode: () =>
                none.Match(
                    none: () => throw new ArgumentNullException(),
                    some: i => i));
        }

        [Fact]
        public void GivenANoneThenOrElseShouldReturnTheArgument()
        {
            var none = Option<int>.None();
            var some = Option.Some(42);

            Assert.Equal(some, some.OrElse(none));
            Assert.Equal(none, none.OrElse(none));
            Assert.Equal(some, none.OrElse(some));
            Assert.Equal(some, none.OrElse(none).OrElse(some));
            Assert.Equal(none, none.OrElse(none).OrElse(none));
            Assert.Equal(1337, none.OrElse(1337));
            Assert.Equal(1337, none.OrElse(none).OrElse(1337));
        }

        [Fact]
        public void GivenAnOptionWithTheIdentityFunctionThenTheResultShouldBeTheSame()
        {
            var none = Option<int>.None();
            var some = Option.Some(42);

            Assert.Equal(none, none.AndThen(Identity));
            Assert.Equal(some, some.AndThen(Identity));
        }

        [Fact]
        public void GivenAnOptionToAndThenWithAStatementThenItShouldCompile()
        {
            var none = Option<int>.None();
            var some = Option.Some(42);

            none.AndThen(Statement);
            some.AndThen(Statement);
        }

        [Fact]
        public void GivenAnOptionAndAndAFuncToOptionItShouldBeFlattened()
        {
            var none = Option<int>.None();
            var some = Option.Some(42);

            Assert.False(none.AndThen(value => Option.Some(1337)).Match(false, v => v == 1337));
            Assert.True(some.AndThen(value => Option.Some(1337)).Match(false, v => v == 1337));
        }

        [Fact]
        public void GivenANoneWithOrElseFuncWeGetTheCorrectValue()
        {
            var none = Option<int>.None();
            var some = Option.Some(42);

            var maybe = none
                .OrElse(() => none)
                .OrElse(() => some);

            Assert.Equal(some, maybe);
        }

        [Fact]
        public void GivenASomeCaseTheOrElseFuncIsNotExecuted()
        {
            var none = Option<int>.None();
            var some = Option.Some(42);
            var global = 0;

            Func<int> sideEffect = () =>
            {
                global = 42;
                return 11;
            };

            var maybe = some
                .OrElse(sideEffect);

            Assert.Equal(0, global);
        }

        [Theory]
        [InlineData("-12")]
        [InlineData("0")]
        [InlineData("-953542")]
        [InlineData("1337")]
        [InlineData("not a number")]
        [InlineData("13 is a number")]
        [InlineData("")]
        public void GivenAnOptionAndTheMatchFunctionAStatmentItShouldCompile(string stringToParse)
        {
            var maybe = stringToParse.TryParseInt();

            maybe.Match(
                none: Statement,
                some: Statement);
        }

        [Fact]
        public void GivenAnOptionOfAnOptionWeGetASimpleOption()
        {
            var option1 = Option.Some(1);
            var option2 = Option.Some(option1);

            Assert.Equal(option1, option2);
        }

        [Fact]
        public void ToEnumerableReturnsEmptyEnumerableWhenOptionIsEmpty()
        {
            var option = Option<int>.None();
            Assert.Empty(option.ToEnumerable());
        }

        [Fact]
        public void ToEnumerableReturnsEnumerableWithOneElementWhenOptionHasValue()
        {
            const int value = 42;
            var option = Option.Some(value);
            Assert.Single(option.ToEnumerable(), value);
        }

        [Fact]
        public void OptionCanBeCreatedFromReferenceType()
        {
            Assert.Equal(Option.Some("foo"), Option.From("foo"));
            Assert.Equal(Option<string>.None(), Option.From<string>(null));
        }

        [Fact]
        public void OptionCanBeCreatedFromNullableValueType()
        {
            Assert.Equal(Option.Some(10), Option.From((int?)10));
            Assert.Equal(Option<int>.None(), Option.From<int>(null));
        }

        [Fact]
        public void OptionCanBeCreatedFromValueType()
        {
            Assert.Equal(Option.Some(10), Option.From(10));
        }

        private void Statement(int value)
        {
        }

        private void Statement()
        {
        }
    }
}
