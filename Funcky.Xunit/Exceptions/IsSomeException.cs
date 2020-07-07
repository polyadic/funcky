using Funcky.Xunit;
using static System.Environment;

namespace Xunit.Sdk
{
    internal sealed class IsSomeException : XunitException
    {
        public override string Message =>
            $"{nameof(FunctionalAssert)}.{nameof(FunctionalAssert.IsSome)}() Failure{NewLine}" +
            $"Expected: Some(_){NewLine}" +
            $"Actual:   None";
    }
}
