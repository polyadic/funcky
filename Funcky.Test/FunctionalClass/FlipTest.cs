using System;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test.FunctionalClass
{
    public sealed class FlipTest
    {
        [Theory]
        [MemberData(nameof(FirstTwoArguments))]
        public void GivenAFunctionWith2ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            Func<int, string, string> f = (int number, string text) => $"number:{number}, text:{text}";

            Assert.Equal(f(number, text), Flip(f)(text, number));
        }

        [Theory]
        [MemberData(nameof(FirstTwoArguments))]
        public void GivenAFunctionWith3ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            Func<int, string, bool, string> f = (int number, string text, bool p3) => $"number:{number}, text:{text}, {p3}";

            Assert.Equal(f(number, text, true), Flip(f)(text, number, true));
        }

        [Theory]
        [MemberData(nameof(FirstTwoArguments))]
        public void GivenAFunctionWith4ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            Func<int, string, bool, bool, string> f = (int number, string text, bool p3, bool p4) => $"number:{number}, text:{text}, {p3}, {p4}";

            Assert.Equal(f(number, text, true, false), Flip(f)(text, number, true, false));
        }

        [Theory]
        [MemberData(nameof(FirstTwoArguments))]
        public void GivenAFunctionWith5ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            Func<int, string, bool, bool, bool, string> f = (int number, string text, bool p3, bool p4, bool p5) => $"number:{number}, text:{text}, {p3}, {p4}, {p5}";

            Assert.Equal(f(number, text, true, false, true), Flip(f)(text, number, true, false, true));
        }

        [Theory]
        [MemberData(nameof(FirstTwoArguments))]
        public void GivenAFunctionWith6ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            Func<int, string, bool, bool, bool, bool, string> f = (int number, string text, bool p3, bool p4, bool p5, bool p6) => $"number:{number}, text:{text}, {p3}, {p4}, {p5}, {p6}";

            Assert.Equal(f(number, text, true, false, true, false), Flip(f)(text, number, true, false, true, false));
        }

        [Theory]
        [MemberData(nameof(FirstTwoArguments))]
        public void GivenAFunctionWith7ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            Func<int, string, bool, bool, bool, bool, bool, string> f = (int number, string text, bool p3, bool p4, bool p5, bool p6, bool p7) => $"number:{number}, text:{text}, {p3}, {p4}, {p5}, {p6}, {p7}";

            Assert.Equal(f(number, text, true, false, true, false, true), Flip(f)(text, number, true, false, true, false, true));
        }

        [Theory]
        [MemberData(nameof(FirstTwoArguments))]
        public void GivenAFunctionWith8ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            Func<int, string, bool, bool, bool, bool, bool, bool, string> f = (int number, string text, bool p3, bool p4, bool p5, bool p6, bool p7, bool p8) => $"number:{number}, text:{text}, {p3}, {p4}, {p5}, {p6}, {p7}";

            Assert.Equal(f(number, text, true, false, true, false, true, false), Flip(f)(text, number, true, false, true, false, true, false));
        }

        public static TheoryData<int, string> FirstTwoArguments()
           => new ()
           {
                { 5, "Hello world!" },
                { -100, "TestString" },
                { 1000, "Something something dark side" },
                { 1337, "so?" },
           };
    }
}
