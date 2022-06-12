namespace Funcky.SourceGenerator.Test;

[UsesVerify] // 👈 Adds hooks for Verify into XUnit
public class OrNoneGeneratorSnapshotTests
{
    private const string OptionSource = @"namespace Funcky.Monads
{
    public struct Option<TItem> where TItem : notnull { }
}";

    [Fact]
    public Task GenerateSingleMethodWithTheSingleArgumentCandidate()
    {
        const string source = @"using System.Diagnostics.Contracts;
using Funcky.Internal;
using Funcky.Monads;

namespace Funcky.Extensions
{
    [OrNoneFromTryPattern(typeof(bool), nameof(bool.TryParse))]
    public static partial class ParseExtensions
    {
    }
}";

        return TestHelper.Verify(source + Environment.NewLine + OptionSource);
    }

    [Fact]
    public Task GenerateSingleMethodWithMultipleArgumentsToForward()
    {
        const string source = @"using System.Diagnostics.Contracts;
using Funcky.Internal;
using Funcky.Monads;

namespace Funcky.Extensions
{
    [OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParse))]
    public static partial class ParseExtensions
    {
    }
}";

        return TestHelper.Verify(source + Environment.NewLine + OptionSource);
    }

    [Fact]
    public Task GenerateMultipleMethodsInASingleClass()
    {
        const string source = @"using System.Diagnostics.Contracts;
using Funcky.Internal;
using Funcky.Monads;

namespace Funcky.Extensions
{
    [OrNoneFromTryPattern(typeof(bool), nameof(bool.TryParse))]
    [OrNoneFromTryPattern(typeof(DateTime), nameof(DateTime.TryParse))]
    public static partial class ParseExtensions
    {
    }
}";

        return TestHelper.Verify(source + Environment.NewLine + OptionSource);
    }
}
