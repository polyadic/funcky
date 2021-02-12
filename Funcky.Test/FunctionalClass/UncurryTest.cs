using System;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test.FunctionalClass
{
    public sealed class UncurryTest
    {
        [Theory]
        [MemberData(nameof(FirstTwoArguments))]
        public void GivenA2ndLevelCurriedFunctionWeGetAFunctionWith2Parameters(int number, string text)
        {
            Func<int, Func<string, string>> f = (int number) => (string text) => $"number:{number}, text:{text}";

            Assert.Equal(f(number)(text), Uncurry(f)(number, text));
        }

        [Theory]
        [MemberData(nameof(FirstTwoArguments))]
        public void GivenA3rdLevelCurriedFunctionWeGetAFunctionWith3Parameters(int number, string text)
        {
            Func<int, Func<string, Func<bool, string>>> f = (int number) => (string text) => (bool p3) => $"number:{number}, text:{text}, {p3}";

            Assert.Equal(f(number)(text)(true), Uncurry(f)(number, text, true));
        }

        [Theory]
        [MemberData(nameof(FirstTwoArguments))]
        public void GivenA4thLevelCurriedFunctionWeGetAFunctionWith4Parameters(int number, string text)
        {
            Func<int, Func<string, Func<bool, Func<bool, string>>>> f = (int number) => (string text) => (bool p3) => (bool p4) => $"number:{number}, text:{text}, {p3}, {p4}";

            Assert.Equal(f(number)(text)(true)(false), Uncurry(f)(number, text, true, false));
        }

        [Theory]
        [MemberData(nameof(FirstTwoArguments))]
        public void GivenA5thLevelCurriedFunctionWeGetAFunctionWith5Parameters(int number, string text)
        {
            Func<int, Func<string, Func<bool, Func<bool, Func<bool, string>>>>> f = (int number) => (string text) => (bool p3) => (bool p4) => (bool p5) => $"number:{number}, text:{text}, {p3}, {p4}, {p5}";

            Assert.Equal(f(number)(text)(true)(false)(true), Uncurry(f)(number, text, true, false, true));
        }

        [Theory]
        [MemberData(nameof(FirstTwoArguments))]
        public void GivenA6thLevelCurriedFunctionWeGetAFunctionWith6Parameters(int number, string text)
        {
            Func<int, Func<string, Func<bool, Func<bool, Func<bool, Func<bool, string>>>>>> f = (int number) => (string text) => (bool p3) => (bool p4) => (bool p5) => (bool p6) => $"number:{number}, text:{text}, {p3}, {p4}, {p5}, {p6}";

            Assert.Equal(f(number)(text)(true)(false)(true)(false), Uncurry(f)(number, text, true, false, true, false));
        }

        [Theory]
        [MemberData(nameof(FirstTwoArguments))]
        public void GivenA7thLevelCurriedFunctionWeGetAFunctionWith7Parameters(int number, string text)
        {
            Func<int, Func<string, Func<bool, Func<bool, Func<bool, Func<bool, Func<bool, string>>>>>>> f = (int number) => (string text) => (bool p3) => (bool p4) => (bool p5) => (bool p6) => (bool p7) => $"number:{number}, text:{text}, {p3}, {p4}, {p5}, {p6}";

            Assert.Equal(f(number)(text)(true)(false)(true)(false)(true), Uncurry(f)(number, text, true, false, true, false, true));
        }

        [Theory]
        [MemberData(nameof(FirstTwoArguments))]
        public void GivenA8thLevelCurriedFunctionWeGetAFunctionWith8Parameters(int number, string text)
        {
            Func<int, Func<string, Func<bool, Func<bool, Func<bool, Func<bool, Func<bool, Func<bool, string>>>>>>>> f = (int number) => (string text) => (bool p3) => (bool p4) => (bool p5) => (bool p6) => (bool p7) => (bool p8) => $"number:{number}, text:{text}, {p3}, {p4}, {p5}, {p6}, {p7}, {p8}";

            Assert.Equal(f(number)(text)(true)(false)(true)(false)(true)(false), Uncurry(f)(number, text, true, false, true, false, true, false));
        }

        public static TheoryData<int, string> FirstTwoArguments()
            => new()
            {
                { 5, "Hello world!" },
                { -100, "TestString" },
                { 1000, "Something something dark side" },
                { 1337, "so?" },
            };
    }
}
