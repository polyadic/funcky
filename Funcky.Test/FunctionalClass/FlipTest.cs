using FsCheck;
using FsCheck.Xunit;

namespace Funcky.Test.FunctionalClass
{
    public sealed class FlipTest
    {
        [Property]
        public Property GivenAFunctionWith2ParametersTheFirstTwoParametersGetFlipped(int number, string text, Func<int, string, string> f)
        {
            return (f(number, text) == Flip(f)(text, number)).ToProperty();
        }

        [Property]
        public Property GivenAFunctionWith3ParametersTheFirstTwoParametersGetFlipped(int number, string text, Func<int, string, bool, string> f)
        {
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

        [Property]
        public Property GivenAnActionWith2ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            string side = string.Empty;
            Action<int, string> f = (number, text) => side = $"number:{number}, text:{text}";

            f(number, text);
            var expected = side;
            Flip(f)(text, number);

            return (expected == side).ToProperty();
        }

        [Property]
        public Property GivenAnActionWith3ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            string side = string.Empty;
            Action<int, string, bool> f = (number, text, p3) => side = $"number:{number}, text:{text}, {p3}";

            f(number, text, true);
            var expected = side;
            Flip(f)(text, number, true);

            return (expected == side).ToProperty();
        }

        [Property]
        public Property GivenAnActionWith4ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            string side = string.Empty;
            Action<int, string, bool, bool> f = (number, text, p3, p4) => side = $"number:{number}, text:{text}, {p3}, {p4}";

            f(number, text, true, false);
            var expected = side;
            Flip(f)(text, number, true, false);

            return (expected == side).ToProperty();
        }

        [Property]
        public Property GivenAnActionWith5ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            string side = string.Empty;
            Action<int, string, bool, bool, bool> f = (number, text, p3, p4, p5) => side = $"number:{number}, text:{text}, {p3}, {p4}, {p5}";

            f(number, text, true, false, false);
            var expected = side;
            Flip(f)(text, number, true, false, false);

            return (expected == side).ToProperty();
        }

        [Property]
        public Property GivenAnActionWith6ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            string side = string.Empty;
            Action<int, string, bool, bool, bool, bool> f = (number, text, p3, p4, p5, p6) => side = $"number:{number}, text:{text}, {p3}, {p4}, {p5}, {p6}";

            f(number, text, true, false, false, true);
            var expected = side;
            Flip(f)(text, number, true, false, false, true);

            return (expected == side).ToProperty();
        }

        [Property]
        public Property GivenAnActionWith7ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            string side = string.Empty;
            Action<int, string, bool, bool, bool, bool, bool> f = (number, text, p3, p4, p5, p6, p7) => side = $"number:{number}, text:{text}, {p3}, {p4}, {p5}, {p6}, {p7}";

            f(number, text, true, false, false, true, true);
            var expected = side;
            Flip(f)(text, number, true, false, false, true, true);

            return (expected == side).ToProperty();
        }

        [Property]
        public Property GivenAnActionWith8ParametersTheFirstTwoParametersGetFlipped(int number, string text)
        {
            string side = string.Empty;
            Action<int, string, bool, bool, bool, bool, bool, bool> f = (number, text, p3, p4, p5, p6, p7, p8) => side = $"number:{number}, text:{text}, {p3}, {p4}, {p5}, {p6}, {p7}, {p8}";

            f(number, text, true, false, false, true, true, true);
            var expected = side;
            Flip(f)(text, number, true, false, false, true, true, true);

            return (expected == side).ToProperty();
        }
    }
}
