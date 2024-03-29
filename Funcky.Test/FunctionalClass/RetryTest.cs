using Funcky.RetryPolicies;
using Funcky.Test.TestUtils;

namespace Funcky.Test.FunctionalClass;

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
            Option<int>.None,
            value,
            Option<int>.None,
            Option<int>.None,
            Option<int>.None,
            Option<int>.None,
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
        const string produceString = "Hello world!";
        var producer = new OptionProducer<string>(1000, produceString);

        Assert.Equal(Option<string>.None, Retry(producer.Produce, new NoDelayRetryPolicy(numberOfRetries)));
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
        const string produceString = "Hello world!";
        var producer = new OptionProducer<string>(numberOfRetries, produceString);

        Assert.Equal(produceString, Retry(producer.Produce, new NoDelayRetryPolicy(1000)));
        Assert.Equal(numberOfRetries + 1, producer.Called);
    }
}
