using Xunit;
using Xunit.Sdk;

namespace Funcky;

public static partial class FunctionalAssert
{
    private static void EqualOrThrow<T>(
        T expected,
        T actual,
        Action @throw)
    {
        try
        {
            Assert.Equal(expected, actual);
        }
        catch (XunitException)
        {
            @throw();
        }
    }
}
