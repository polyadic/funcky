using Funcky.Monads;
using Xunit;

namespace Funcky.Test
{
    public class NoOperationTest
    {
        [Fact]
        void GivenTheNoOperationFunctionWeCanApplyItToMatch()
        {
            var none = Option<int>.None();

            int sideEffect = 0;
            none.Match(Functional.NoOperation, i => sideEffect = i);

            Assert.Equal(0, sideEffect);
        }
    }
}
