using System;
using Funcky.Xunit;
using static System.Environment;
using static Xunit.Sdk.FormatUtility;

namespace Xunit.Sdk
{
    internal sealed class IsOkException : XunitException
    {
        private readonly Exception _exception;

        public IsOkException(Exception exception)
        {
            _exception = exception;
        }

        public override string Message =>
            $"{nameof(FunctionalAssert)}.{nameof(FunctionalAssert.IsOk)}() Failure{NewLine}" +
            $"Expected: Ok(_){NewLine}" +
            $"Actual:   {FormatException(_exception)}";
    }
}
