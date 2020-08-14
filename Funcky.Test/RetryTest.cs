using System.Collections.Generic;
using Funcky.Monads;
using Xunit;
using static Funcky.Functional;

namespace Funcky.Test
{
    public sealed class RetryTest
    {
        [Fact]
        public void ReturnsTheValueImmediatelyIfTheProducerIsPureAndReturnsSome()
        {
            const int value = 10;
            Assert.Equal(value, Retry(() => Option.Some(value)));
        }

        [Fact]
        public void RetriesTheProducerUntilAValueIsReturned()
        {
            const int value = 10;
            var stack = new Stack<Option<int>>(new[]
            {
                Option<int>.None(),
                Option<int>.None(),
                Option<int>.None(),
                Option<int>.None(),
                Option.Some(value),
                Option<int>.None(),
            });
            Assert.Equal(value, Retry(stack.Pop));
        }
    }
}
