using System;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test.FunctionalClass
{
    public sealed class CurryTest
    {
        [Theory]
        [MemberData(nameof(FirstTwoArguments))]
        public void GivenAFunctionWith2ParametersWeGet2FunctionsWith1Parameter(int number, string text)
        {
            Func<int, string, string> f = (int number, string text) => $"number:{number}, text:{text}";

            Assert.Equal(f(number, text), Curry(f)(number)(text));
        }

        [Theory]
        [MemberData(nameof(FirstTwoArguments))]
        public void GivenAFunctionWith3ParametersWeGet3FunctionsWith1Parameter(int number, string text)
        {
            Func<int, string, bool, string> f = (int number, string text, bool p3) => $"number:{number}, text:{text}, {p3}";

            Assert.Equal(f(number, text, true), Curry(f)(number)(text)(true));
        }

        [Theory]
        [MemberData(nameof(FirstTwoArguments))]
        public void GivenAFunctionWith4ParametersWeGet4FunctionsWith1Parameter(int number, string text)
        {
            Func<int, string, bool, bool, string> f = (int number, string text, bool p3, bool p4) => $"number:{number}, text:{text}, {p3}, {p4}";

            Assert.Equal(f(number, text, true, false), Curry(f)(number)(text)(true)(false));
        }

        public static TheoryData<int, string> FirstTwoArguments()
            => new TheoryData<int, string>
            {
                { 5, "Hello world!" },
                { -100, "TestString" },
                { 1000, "Something something dark side" },
                { 1337, "so?" },
            };
    }
}
