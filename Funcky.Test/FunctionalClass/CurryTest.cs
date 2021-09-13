using FsCheck;
using FsCheck.Xunit;

namespace Funcky.Test.FunctionalClass
{
    public sealed class CurryTest
    {
        [Property]
        public Property GivenAFunctionWith2ParametersTheFunctionsAlwaysGiveTheSameResult(Func<int, string, string> f, int number1, string text1)
        {
            var functionForm = f(number1, text1) == Curry(f)(number1)(text1);
            var extensionForm = f(number1, text1) == f.Curry()(number1)(text1);

            return (functionForm && extensionForm).ToProperty();
        }

        [Property]
        public Property GivenAFunctionWith3ParametersTheFunctionsAlwaysGiveTheSameResult(Func<int, string, bool, string> f, int number1, string text1, bool bool1)
        {
            var functionForm = f(number1, text1, bool1) == Curry(f)(number1)(text1)(bool1);
            var extensionForm = f(number1, text1, bool1) == f.Curry()(number1)(text1)(bool1);

            return (functionForm && extensionForm).ToProperty();
        }

        [Property(Skip = "https://github.com/fscheck/FsCheck/issues/557")]
        public Property GivenAFunctionWith4ParametersTheFunctionsAlwaysGiveTheSameResult(Func<int, string, int, string, string> f, int number1, string text1, int number2, string text2)
        {
            var functionForm = f(number1, text1, number2, text2) == Curry(f)(number1)(text1)(number2)(text2);
            var extensionForm = f(number1, text1, number2, text2) == f.Curry()(number1)(text1)(number2)(text2);

            return (functionForm && extensionForm).ToProperty();
        }

        [Property(Skip = "https://github.com/fscheck/FsCheck/issues/557")]
        public Property GivenAFunctionWith5ParametersTheFunctionsAlwaysGiveTheSameResult(Func<int, string, int, string, bool, string> f, int number1, string text1, int number2, string text2, bool bool1)
        {
            var functionForm = f(number1, text1, number2, text2, bool1) == Curry(f)(number1)(text1)(number2)(text2)(bool1);
            var extensionForm = f(number1, text1, number2, text2, bool1) == f.Curry()(number1)(text1)(number2)(text2)(bool1);

            return (functionForm && extensionForm).ToProperty();
        }

        [Property(Skip = "https://github.com/fscheck/FsCheck/issues/557")]
        public Property GivenAFunctionWith6ParametersTheFunctionsAlwaysGiveTheSameResult(Func<int, string, int, string, int, string, string> f, int number1, string text1, int number2, string text2, int number3, string text3)
        {
            var functionForm = f(number1, text1, number2, text2, number3, text3) == Curry(f)(number1)(text1)(number2)(text2)(number3)(text3);
            var extensionForm = f(number1, text1, number2, text2, number3, text3) == f.Curry()(number1)(text1)(number2)(text2)(number3)(text3);

            return (functionForm && extensionForm).ToProperty();
        }

        [Property(Skip = "https://github.com/fscheck/FsCheck/issues/557")]
        public Property GivenAFunctionWith7ParametersTheFunctionsAlwaysGiveTheSameResult(Func<int, string, int, string, int, string, bool, string> f, int number1, string text1, int number2, string text2, int number3, string text3, bool bool1)
        {
            var functionForm = f(number1, text1, number2, text2, number3, text3, bool1) == Curry(f)(number1)(text1)(number2)(text2)(number3)(text3)(bool1);
            var extensionForm = f(number1, text1, number2, text2, number3, text3, bool1) == f.Curry()(number1)(text1)(number2)(text2)(number3)(text3)(bool1);

            return (functionForm && extensionForm).ToProperty();
        }

        [Property(Skip = "https://github.com/fscheck/FsCheck/issues/557")]
        public Property GivenAFunctionWith8ParametersTheFunctionsAlwaysGiveTheSameResult(Func<int, string, int, string, int, string, int, string, string> f, int number1, string text1, int number2, string text2, int number3, string text3, int number4, string text4)
        {
            var functionForm = f(number1, text1, number2, text2, number3, text3, number4, text4) == Curry(f)(number1)(text1)(number2)(text2)(number3)(text3)(number4)(text4);
            var extensionForm = f(number1, text1, number2, text2, number3, text3, number4, text4) == f.Curry()(number1)(text1)(number2)(text2)(number3)(text3)(number4)(text4);

            return (functionForm && extensionForm).ToProperty();
        }
    }
}
