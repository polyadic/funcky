using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace Funcky.Test.Extensions
{
    public sealed class FunctionCompositionTest
    {
        [Property]
        public Property FunctionWithTheSameInputAsOutputCanBeComposedBothWays(int argument)
        {
            Func<int, int> f = x => x + 2;
            Func<int, int> g = x => x * 3;

            return (f(g(argument)) == f.Compose(g)(argument)
                    && g(f(argument)) == g.Compose(f)(argument))
                    .ToProperty();
        }

        [Property]
        public Property TheComposedFunctionIsTheSameAsTheFunctionApplication(Func<int, bool> f, Func<string, int> g, string s)
        {
            var composed = f.Compose(g);

            return (f(g(s)) == composed(s)).ToProperty();
        }

        [Fact]
        public void FunctionCompositionWorksWithNullaryFunctionInput()
        {
            Func<int, string> f = x => $"{x} + 2";
            Func<int> g = () => 9;

            Assert.Equal(f.Compose(g)(), f(g()));
        }

        [Property]
        public Property FunctionCompositionWorksWithAction(int argument)
        {
            var sideEffect = new SideEffect();
            Action<string> f = sideEffect.Store;
            Func<int, string> g = x => $"{x} + 2";

            var composed = f.Compose(g);
            composed.Invoke(argument);

            return (sideEffect.Retrieve() == $"{argument} + 2").ToProperty();
        }

        [Fact]
        public void FunctionCompositionWorksWithActionAndNullaryFunctionInput()
        {
            var sideEffect = new SideEffect();
            Action<string> f = sideEffect.Store;
            Func<string> g = () => "Hello World!";

            var composed = f.Compose(g);
            composed.Invoke();

            Assert.Equal("Hello World!", sideEffect.Retrieve());
        }
    }
}
