using Funcky.Xunit;
using static System.Environment;

namespace Xunit.Sdk
{
    internal sealed class IsNoneException : XunitException
    {
        private readonly object _actualValue;

        public IsNoneException(object actualValue)
        {
            _actualValue = actualValue;
        }

        public override string Message =>
            $"{nameof(FunctionalAssert)}.{nameof(FunctionalAssert.IsNone)} Failure{NewLine}" +
            $"Expected: None{NewLine}" +
            $"Actual:   Some({_actualValue})";
    }
}
