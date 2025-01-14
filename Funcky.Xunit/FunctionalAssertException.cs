using System.Runtime.CompilerServices;
using Xunit.Sdk;

namespace Funcky;

internal static class FunctionalAssertException
{
    private static readonly string NewLineAndIndent = Environment.NewLine + new string(' ', 10);  // Length of "Expected: " and "Actual:   "

    public static XunitException ForMismatchedValues(
        string expected,
        string actual,
        [CallerMemberName] string? assertionName = null)
    {
        var assertionLabel = assertionName is not null
            ? $"{nameof(FunctionalAssert)}.{assertionName}()"
            : nameof(FunctionalAssert);
        return new XunitException(
              $"{assertionLabel} Failure: Values differ{Environment.NewLine}"
            + $"Expected: {expected.Replace(Environment.NewLine, NewLineAndIndent)}{Environment.NewLine}"
            + $"Actual:   {actual.Replace(Environment.NewLine, NewLineAndIndent)}");
    }
}
