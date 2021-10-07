using Funcky.Test.TestUtils;

namespace Funcky.Test.FunctionalClass
{
    public sealed class RetryAsyncTest
    {
        [Fact]
        public async Task ReturnsTheValueImmediatelyIfTheProducerIsPureAndReturnsSome()
        {
            const int value = 10;
            FunctionalAssert.IsSome(value, await RetryAsync(() => Task.FromResult(Option.Some(value)), new DoNotRetryPolicy()));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(15)]
        [InlineData(90)]
        public async Task RetriesWithDoNotRetryPolicyAlwaysTriesNumberOfRetriesTimes(int numberOfRetries)
        {
            const string produceString = "Hello world!";
            var producer = new MaybeProducer<string>(1000, produceString);

            FunctionalAssert.IsNone(await RetryAsync(producer.ProduceAsync, new NoDelayRetryPolicy(numberOfRetries)));
            Assert.Equal(numberOfRetries + 1, producer.Called);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(15)]
        [InlineData(90)]
        public async Task RetriesWithDoNotRetryRetriesUntilValueProduced(int numberOfRetries)
        {
            const string produceString = "Hello world!";
            var producer = new MaybeProducer<string>(numberOfRetries, produceString);

            FunctionalAssert.IsSome(produceString, await RetryAsync(producer.ProduceAsync, new NoDelayRetryPolicy(1000)));
            Assert.Equal(numberOfRetries + 1, producer.Called);
        }
    }
}
