using Xunit.Abstractions;

namespace Funcky.SourceGenerator.Test;

[UsesVerify] // 👈 Adds hooks for Verify into XUnit
public class OrNoneGeneratorSnapshotTests
{
    private const string OptionSource =
        """
        namespace Funcky.Monads
        {
            public struct Option<TItem> where TItem : notnull
            {
                public static implicit operator Option<TItem>(TItem item) => default;
            }
        }
        """;

    private readonly ITestOutputHelper _testOutputHelper;

    public OrNoneGeneratorSnapshotTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public Task GenerateSingleMethodWithTheSingleArgumentCandidate()
    {
        const string source =
            """
            #nullable enable

            using System.Diagnostics.Contracts;
            using Funcky.Internal;
            using Funcky.Monads;

            namespace Funcky.Extensions
            {
                [OrNoneFromTryPattern(typeof(Target), nameof(Target.TryParse))]
                public static partial class ParseExtensions
                {
                }

                public sealed class Target
                {
                    public static bool TryParse(string candidate, out Target result)
                    {
                        result = default!;
                        return false;
                    }
                }
            }
            """;

        return TestHelper.Verify(source + Environment.NewLine + OptionSource);
    }

    [Fact]
    public Task GeneratesMethodWhenTargetIsNotNullableAnnotated()
    {
        const string source = """
            using System.Diagnostics.Contracts;
            using Funcky.Internal;
            using Funcky.Monads;

            namespace Funcky.Extensions
            {
                [OrNoneFromTryPattern(typeof(Target), nameof(Target.TryParse))]
                public static partial class ParseExtensions
                {
                }

                public sealed class Target
                {
                    public static bool TryParse(string candidate, out Target result)
                    {
                        result = default!;
                        return false;
                    }
                }
            }
            """;

        return TestHelper.Verify(source + Environment.NewLine + OptionSource);
    }

    [Fact]
    public Task GenerateSingleMethodWithMultipleArgumentsToForward()
    {
        const string source = """
            #nullable enable

            using System;
            using System.Diagnostics.Contracts;
            using Funcky.Internal;
            using Funcky.Monads;

            namespace Funcky.Extensions
            {
                [OrNoneFromTryPattern(typeof(Target), nameof(Target.TryParse))]
                public static partial class ParseExtensions
                {
                }

                public sealed class Target
                {
                    public static bool TryParse(string candidate, bool caseSensitive, IFormatProvider provider, out Target result)
                    {
                        result = default!;
                        return false;
                    }
                }
            }
            """;

        return TestHelper.Verify(source + Environment.NewLine + OptionSource);
    }

    [Fact]
    public Task GeneratesAllOverloadsForAGivenMethod()
    {
        const string source = """
            #nullable enable

            using System;
            using System.Diagnostics.Contracts;
            using Funcky.Internal;
            using Funcky.Monads;

            namespace Funcky.Extensions
            {
                [OrNoneFromTryPattern(typeof(Target), nameof(Target.TryParse))]
                public static partial class ParseExtensions
                {
                }

                public sealed class Target
                {
                    public static bool TryParse(string candidate, out Target result)
                    {
                        result = default!;
                        return false;
                    }

                    public static bool TryParse(string candidate, bool caseSensitive, out Target result)
                    {
                        result = default!;
                        return false;
                    }
                }
            }
            """;

        return TestHelper.Verify(source + Environment.NewLine + OptionSource);
    }

    [Fact]
    public Task GenerateMultipleMethodsInASingleClass()
    {
        const string source = """
            #nullable enable

            using System;
            using System.Diagnostics.Contracts;
            using Funcky.Internal;
            using Funcky.Monads;

            namespace Funcky.Extensions
            {
                [OrNoneFromTryPattern(typeof(Target), nameof(Target.TryParse))]
                [OrNoneFromTryPattern(typeof(Target), nameof(Target.TryParseExact))]
                public static partial class ParseExtensions
                {
                }

                public sealed class Target
                {
                    public static bool TryParse(string candidate, out Target result)
                    {
                        result = default!;
                        return false;
                    }

                    public static bool TryParseExact(string candidate, out Target result)
                    {
                        result = default!;
                        return false;
                    }
                }
            }
            """;

        return TestHelper.Verify(source + Environment.NewLine + OptionSource);
    }

    [Fact]
    public Task CopiesStringSyntaxAttributeFromOriginalDefinition()
    {
        const string source = """
            #nullable enable

            using System;
            using System.Diagnostics.CodeAnalysis;
            using Funcky.Internal;

            namespace Funcky.Extensions
            {
                [OrNoneFromTryPattern(typeof(Target), nameof(Target.TryParse))]
                public static partial class ParseExtensions
                {
                }

                public sealed class Target
                {
                    public static bool TryParse(string candidate, [StringSyntaxAttribute("foo")] string format, out Target result)
                    {
                        result = default!;
                        return false;
                    }
                }
            }
            """;

        return TestHelper.Verify(source + Environment.NewLine + OptionSource);
    }
}
