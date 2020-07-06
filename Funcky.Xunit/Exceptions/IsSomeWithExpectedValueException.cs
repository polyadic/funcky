using Funcky.Xunit;
using static System.Environment;

namespace Xunit.Sdk
{
    internal sealed class IsSomeWithExpectedValueException : XunitException
    {
        private readonly object _expectedValue;

        private readonly object _actualValue;

        public IsSomeWithExpectedValueException(object expectedValue, object actualValue)
        {
            _expectedValue = expectedValue;
            _actualValue = actualValue;
        }

        public override string Message =>
            $"{nameof(FunctionalAssert)}.{nameof(FunctionalAssert.IsSome)} Failure{NewLine}" +
            $"Expected: Some({_expectedValue}){NewLine}" +
            $"Actual:   {_actualValue}";
    }
}
