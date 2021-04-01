using System;
using FsCheck;
using FsCheck.Xunit;
using static Funcky.Functional;

namespace Funcky.Test.FunctionalClass
{
    public sealed class FlipTest
    {
        [Property]
        public Property GivenAFunctionWith2ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            Func<int, string, string> f = (number, text) => $"number:{number}, text:{text}";

            return (f(number, text) == Flip(f)(text, number)).ToProperty();
        }

        [Property]
        public Property GivenAFunctionWith3ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            Func<int, string, bool, string> f = (number, text, p3) => $"number:{number}, text:{text}, {p3}";

            return (f(number, text, true) == Flip(f)(text, number, true)).ToProperty();
        }

        [Property]
        public Property GivenAFunctionWith4ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            Func<int, string, bool, bool, string> f = (number, text, p3, p4) => $"number:{number}, text:{text}, {p3}, {p4}";

            return (f(number, text, true, false) == Flip(f)(text, number, true, false)).ToProperty();
        }

        [Property]
        public Property GivenAFunctionWith5ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            Func<int, string, bool, bool, bool, string> f = (number, text, p3, p4, p5) => $"number:{number}, text:{text}, {p3}, {p4}, {p5}";

            return (f(number, text, true, false, true) == Flip(f)(text, number, true, false, true)).ToProperty();
        }

        [Property]
        public Property GivenAFunctionWith6ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            Func<int, string, bool, bool, bool, bool, string> f = (number, text, p3, p4, p5, p6) => $"number:{number}, text:{text}, {p3}, {p4}, {p5}, {p6}";

            return (f(number, text, true, false, true, false) == Flip(f)(text, number, true, false, true, false)).ToProperty();
        }

        [Property]
        public Property GivenAFunctionWith7ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            Func<int, string, bool, bool, bool, bool, bool, string> f = (number, text, p3, p4, p5, p6, p7) => $"number:{number}, text:{text}, {p3}, {p4}, {p5}, {p6}, {p7}";

            return (f(number, text, true, false, true, false, true) == Flip(f)(text, number, true, false, true, false, true)).ToProperty();
        }

        [Property]
        public Property GivenAFunctionWith8ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            Func<int, string, bool, bool, bool, bool, bool, bool, string> f = (number, text, p3, p4, p5, p6, p7, p8) => $"number:{number}, text:{text}, {p3}, {p4}, {p5}, {p6}, {p7}, {p8}";

            return (f(number, text, true, false, true, false, true, false) == Flip(f)(text, number, true, false, true, false, true, false)).ToProperty();
        }
    }
}
