using System.Globalization;
using Funcky.FsCheck;
using Xunit.Sdk;

namespace Funcky.Test.Monads;

public sealed partial class OptionTest
{
    public OptionTest()
    {
        FunckyGenerators.Register();
        Thread.CurrentThread.CurrentCulture = new CultureInfo("de-CH");
    }

    private enum MyEnum
    {
        None,
        Cool,
    }

    [Fact]
    public void OptionConstructorThrowsWhenNullIsPassed()
    {
        Assert.Throws<ArgumentNullException>(() => Option.Some<string>(null!));
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
        var none = Option<int>.None;

        Assert.Equal(typeof(int), none.GetType().GetGenericArguments().First());
    }

    [Fact]
    public void GivenTheTypeConstructorSomeThenCreateSome()
    {
        var some = Option.Some(42);

        Assert.Equal(typeof(int), some.GetType().GetGenericArguments().First());
    }

    [Fact]
    public void GiveAValueNoneWithLinqSyntaxSelectThenTheResultShouldBeNone()
    {
        var maybe = Option<int>.None;
        var maybeBool =
            from m in maybe
            select m == 1337;

        FunctionalAssert.IsNone(maybeBool);
    }

    [Fact]
    public void GivenTwoSomeValuesWithASelectManyWenWrittenInMethodSyntaxThenTheResultShouldBeSomeValue()
    {
        var someNumber = "1337".ParseInt32OrNone();
        var someDate = "12.2.2009".ParseDateTimeOrNone();

        var result = someNumber.SelectMany(_ => someDate, Tuple.Create);

        var resultShouldBe = Option.Some(new Tuple<int, DateTime>(1337, new DateTime(2009, 2, 12)));
        Assert.Equal(resultShouldBe, result);
    }

    [Fact]
    public void GivenTwoSomeValuesWithASelectManyWenWrittenInLinqSyntaxThenTheResultShouldBeSomeValue()
    {
        var someNumber = "1337".ParseInt32OrNone();
        var someDate = "12.2.2009".ParseDateTimeOrNone();

        var result = from number in someNumber
                     from date in someDate
                     select Tuple.Create(number, date);

        var resultShouldBe = Option.Some(new Tuple<int, DateTime>(1337, new DateTime(2009, 2, 12)));
        Assert.Equal(resultShouldBe, result);
    }

    [Fact]
    public void GivenASelectManyWithOneNoneInputThenTheResultShouldBeNone()
    {
        var someNumber = "1337".ParseInt32OrNone();
        var someDate = "12.2.2009".ParseDateTimeOrNone();
        var someOtherNumber = "not a number".ParseInt32OrNone();

        var result = from number in someNumber
                     from date in someDate
                     from otherNumber in someOtherNumber
                     select Tuple.Create(number, date, otherNumber);

        FunctionalAssert.IsNone(result);
    }

    [Fact]
    public void GivenAFilterWhichFiltersAllNoneThenOnlyTheSomeValuesPassThrough()
    {
        const string input = "123,some,x,1337,42,1,1000";

        foreach (var number in input.Split(',').Select(ParseExtensions.ParseInt32OrNone).Where(maybeInt => maybeInt.Match(none: false, some: True)))
        {
            _ = FunctionalAssert.IsSome(number);
        }
    }

    public static IEnumerable<object[]> TestValues()
    {
        yield return new object[] { "Some(1337)", Option.Some(1337) };
        yield return new object[] { "Some(-1)", Option.Some(-1) };
        yield return new object[] { "Some(10000)", Option.Some(10E3) };
        yield return new object[] { "Some(string)", Option.Some("string") };
        yield return new object[] { "None", Option<int>.None };
        yield return new object[] { "None", Option<string>.None };
        yield return new object[] { "Some(Cool)", Option.Some(MyEnum.Cool) };
        yield return new object[] { "None", Option<MyEnum>.None };
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
        => new()
        {
            { Option.Some("foo"), Option.Some("foo"), True },
            { Option<string>.None, Option.Some("foo"), False },
            { Option<string>.None, Option<string>.None, True },
            { Option<string>.None, Option<string>.None, False },
        };

    [Theory]
    [MemberData(nameof(TestValues))]

    public void GivenAnOptionObjectThenToStringReturnsUsefulString(string reference, object option)
    {
        Assert.Equal(reference, option.ToString());
    }

    [Fact]
    public void MatchOverloadWithTwoFuncObjectsWorksCorrectly()
    {
        const string input = "123,some,x,1337,42,1,1000";

        foreach (var number in input.Split(',').Select(ParseExtensions.ParseInt32OrNone).Where(maybeInt => maybeInt.Match(none: false, some: True)))
        {
            var value = number.Match(
                none: () => 0,
                some: Identity);

            Assert.NotEqual(0, value);
        }
    }

    [Fact]
    public void GivenAMatchingStatementWhichThrowsAnExceptionThrowsTheSameException()
    {
        var none = Option<int>.None;

        Assert.Throws<ArgumentNullException>(testCode: () =>
            none.Match(
                none: () => throw new ArgumentNullException(),
                some: Identity));
    }

    [Fact]
    public void GivenANoneThenOrElseShouldReturnTheArgument()
    {
        var none = Option<int>.None;
        var some = Option.Some(42);

        Assert.Equal(some, some.OrElse(none));
        Assert.Equal(none, none.OrElse(none));
        Assert.Equal(some, none.OrElse(some));
        Assert.Equal(some, none.OrElse(none).OrElse(some));
        Assert.Equal(none, none.OrElse(none).OrElse(none));
        Assert.Equal(42, some.GetOrElse(1337));
        Assert.Equal(1337, none.GetOrElse(1337));
        Assert.Equal(1337, none.OrElse(none).GetOrElse(1337));
    }

    [Fact]
    public void GivenAnOptionWithTheIdentityFunctionThenTheResultShouldBeTheSame()
    {
        var none = Option<int>.None;
        var some = Option.Some(42);

        Assert.Equal(none, none.AndThen(Identity));
        Assert.Equal(some, some.AndThen(Identity));
    }

    [Fact]
    public void GivenAnOptionToAndThenWithAStatementThenItShouldCompile()
    {
        var none = Option<int>.None;
        var some = Option.Some(42);

        none.AndThen(Statement);
        some.AndThen(Statement);
    }

    [Fact]
    public void GivenAnOptionAndAndAFuncToOptionItShouldBeFlattened()
    {
        var none = Option<int>.None;
        var some = Option.Some(42);

        FunctionalAssert.IsNone(none.AndThen(_ => 1337));
        Assert.Equal(1337, FunctionalAssert.IsSome(some.AndThen(_ => 1337)));

        FunctionalAssert.IsNone(none.AndThen(_ => Option.Some(1337)));
        Assert.Equal(1337, FunctionalAssert.IsSome(some.AndThen(_ => Option.Some(1337))));
    }

    [Fact]
    public void GivenANoneWithOrElseFuncWeGetTheCorrectValue()
    {
        var none = Option<int>.None;
        var some = Option.Some(42);

        var maybe = none
            .OrElse(() => none)
            .OrElse(() => some);

        Assert.Equal(some, maybe);
    }

    [Fact]
    public void GivenASomeCaseTheOrElseFuncIsNotExecuted()
    {
        var some = Option.Some(42);

        _ = some.GetOrElse(SideEffect);

        int SideEffect()
            => throw new XunitException("This side effect should not happen!");
    }

    [Theory]
    [InlineData("-12")]
    [InlineData("0")]
    [InlineData("-953542")]
    [InlineData("1337")]
    [InlineData("not a number")]
    [InlineData("13 is a number")]
    [InlineData("")]
    public void GivenAnOptionAndTheMatchFunctionAStatementItShouldCompile(string stringToParse)
    {
        var maybe = stringToParse.ParseInt32OrNone();

        maybe.Switch(
            none: Statement,
            some: Statement);
    }

    [Fact]
    public void OptionFromDoesNotFlattenOptions()
    {
        var option = Option.Some(Option.Some(1));
#pragma warning disable 183 // The given operation is always of the provided type
        Assert.True(option is Option<Option<int>>);
#pragma warning restore 183
    }

    [Fact]
    public void ToEnumerableReturnsEmptyEnumerableWhenOptionIsEmpty()
    {
        var option = Option<int>.None;
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
        Assert.Equal(Option.Some("foo"), Option.FromNullable("foo"));
        Assert.Equal(Option<string>.None, Option.FromNullable<string>(null));
    }

    [Fact]
    public void OptionCanBeCreatedFromNullableValueType()
    {
        Assert.Equal(Option.Some(10), Option.FromNullable((int?)10));
        Assert.Equal(Option<int>.None, Option.FromNullable((int?)null));
    }

    [Theory]
    [MemberData(nameof(EqualOptions))]
    public void EqualsReturnsTrueForEqualOptions(Option<int> lhs, Option<int> rhs)
    {
        Assert.True(lhs == rhs);
    }

    public static TheoryData<Option<int>, Option<int>> EqualOptions()
        => new()
        {
            { Option.Some(1), Option.Some(1) },
            { Option<int>.None, Option<int>.None },
        };

    [Theory]
    [MemberData(nameof(NotEqualOptions))]
    public void EqualsReturnsFalseForNotEqualOptions(Option<int> lhs, Option<int> rhs)
    {
        Assert.True(lhs != rhs);
    }

    public static TheoryData<Option<int>, Option<int>> NotEqualOptions()
        => new()
        {
            { Option.Some(1), Option<int>.None },
            { Option<int>.None, Option.Some(1) },
            { Option<int>.None, Option.Some(default(int)) },
            { Option.Some(default(int)), Option<int>.None },
            { Option.Some(1), Option.Some(2) },
            { Option.Some(2), Option.Some(1) },
        };

    [Fact]
    public void InspectExecutesSideEffectWhenOptionIsSome()
    {
        const int arbitraryValue = 10;
        var option = Option.Some(arbitraryValue);
        var sideEffect = false;
        var inspectedOption = option.Inspect(v =>
        {
            Assert.Equal(arbitraryValue, v);
            sideEffect = true;
        });
        Assert.True(sideEffect);
        Assert.Equal(option, inspectedOption);
    }

    [Fact]
    public void InspectDoesNotExecuteSideEffectWhenOptionIsNone()
    {
        var option = Option<int>.None;
        var sideEffect = false;
        var inspectedOption = option.Inspect(_ => sideEffect = true);
        Assert.False(sideEffect);
        Assert.Equal(option, inspectedOption);
    }

    [Fact]
    public void GivenAFunctionWithADefaultOptionParameterGivesANoneWhenNoValueGiven()
    {
        FunctionalAssert.IsNone(MethodWithDefaultOptionParameter());
        FunctionalAssert.IsSome("value", MethodWithDefaultOptionParameter(Option.Some("value")));
    }

    [Fact]
    public void GivenAnEmptyListOfOptionsFirstOrDefaultProducesANoneValue()
    {
        var defaultValue = Enumerable.Range(0, 10)
            .Select(_ => Option.Some(1337))
            .FirstOrDefault(False);

        FunctionalAssert.IsNone(defaultValue);
    }

    private Option<string> MethodWithDefaultOptionParameter(Option<string> value = default) => value;

    private static void Statement(int value)
    {
    }

    private static void Statement()
    {
    }
}
