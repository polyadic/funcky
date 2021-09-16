using Funcky.Xunit;
using static Xunit.Sdk.FormatUtility;

namespace Xunit.Sdk
{
    internal sealed class IsOkWithExpectedValueException : XunitException
    {
        private readonly object? _expectedValue;

        private readonly Result<object?> _actualValue;

        public IsOkWithExpectedValueException(object? expectedValue, Result<object?> actualValue)
        {
            _expectedValue = expectedValue;
            _actualValue = actualValue;
        }

        public override string Message =>
            $"{nameof(FunctionalAssert)}.{nameof(FunctionalAssert.IsOk)}() Failure{Environment.NewLine}" +
            $"Expected: Ok({_expectedValue}){Environment.NewLine}" +
            $"Actual:   {FormatResult(_actualValue)}";
    }
}
