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
                Option.Some(value),
                Option<int>.None(),
                Option<int>.None(),
                Option<int>.None(),
                Option<int>.None(),
            });
            Assert.Equal(value, Retry(stack.Pop));
            Assert.Single(stack);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(15)]
        [InlineData(90)]
        public void RetriesWithDoNotRetryPolicyAlwaysTriesNumberOfRetriesTimes(int numberOfRetries)
        {
            var produceString = "Hello world!";
            var producer = new MaybeProducer<string>(1000, produceString);

            Assert.Equal(Option<string>.None(), Retry(producer.Produce, new NoDelayRetryPolicy(numberOfRetries)));
            Assert.Equal(numberOfRetries + 1, producer.Called);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(15)]
        [InlineData(90)]
        public void RetriesWithDoNotRetryRetriesUntilValueProduced(int numberOfRetries)
        {
            var produceString = "Hello world!";
            var producer = new MaybeProducer<string>(numberOfRetries, produceString);

            Assert.Equal(Option.Some("Hello world!"), Retry(producer.Produce, new NoDelayRetryPolicy(1000)));
            Assert.Equal(numberOfRetries + 1, producer.Called);
        }
    }
}
