using Funcky.Xunit;
using static System.Environment;

namespace Xunit.Sdk
{
    internal sealed class IsErrorException : XunitException
    {
        private readonly object? _actualValue;

        public IsErrorException(object? actualValue)
        {
            _actualValue = actualValue;
        }

        public override string Message =>
            $"{nameof(FunctionalAssert)}.{nameof(FunctionalAssert.IsError)} Failure{NewLine}" +
            $"Expected: Error(_){NewLine}" +
            $"Actual:   Ok({_actualValue})";
    }
}
