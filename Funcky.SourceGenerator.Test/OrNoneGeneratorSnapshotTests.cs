namespace Funcky.SourceGenerator.Test;

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
    public Task GenerateMethodWithDefaultValuedArgument()
    {
        const string source =
            """
            #nullable enable

            using Funcky.Internal;

            namespace Funcky.Extensions
            {
                [OrNoneFromTryPattern(typeof(Target), nameof(Target.TryParse))]
                public static partial class ParseExtensions
                {
                }

                public sealed class Target
                {
                    public static bool TryParse(string candidate, out Target result, string? hasDefault = null)
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
    public Task ForwardsInAndRefParametersWithTheirModifiers()
    {
        const string source = """
            #nullable enable

            using System;
            using Funcky.Internal;

            namespace Funcky.Extensions
            {
                [OrNoneFromTryPattern(typeof(Target), nameof(Target.TryParse))]
                public static partial class ParseExtensions
                {
                }

                public sealed class Target
                {
                    public static bool TryParse(string candidate, in int budget, ref int consumed, out Target result)
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
    public Task GenerateMethodWithNonNullDefaultValues()
    {
        const string source = """
            #nullable enable

            using System;
            using Funcky.Internal;

            namespace Funcky.Extensions
            {
                [OrNoneFromTryPattern(typeof(Target), nameof(Target.TryParse))]
                public static partial class ParseExtensions
                {
                }

                public enum Mode { Strict = 0, Lenient = 1 }

                public sealed class Target
                {
                    public static bool TryParse(
                        string candidate,
                        out Target result,
                        bool strict = true,
                        int retries = 3,
                        double tolerance = 0.5,
                        char separator = ',',
                        string prefix = "x",
                        Mode mode = Mode.Lenient,
                        DateTime timestamp = default,
                        int? limit = null)
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
    public Task SkipsOverloadsThatDoNotFollowTheTryPattern()
    {
        const string source = """
            #nullable enable

            using System;
            using Funcky.Internal;

            namespace Funcky.Extensions
            {
                [OrNoneFromTryPattern(typeof(Target), nameof(Target.TryParse))]
                public static partial class ParseExtensions
                {
                }

                public sealed class Target
                {
                    // The only overload that follows the pattern: static, bool, exactly one out parameter.
                    public static bool TryParse(string candidate, out Target result)
                    {
                        result = default!;
                        return false;
                    }

                    // No out parameter.
                    public static bool TryParse(string candidate)
                        => false;

                    // Two out parameters.
                    public static bool TryParse(string candidate, out Target result, out int consumed)
                    {
                        result = default!;
                        consumed = 0;
                        return false;
                    }

                    // Not a bool.
                    public static Target TryParse(string candidate, long unused, out bool success)
                    {
                        success = false;
                        return default!;
                    }

                    // Not static.
                    public bool TryParse(string candidate, int unused, out Target result)
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
    public Task GeneratesDistinctFilesForSourceFilesWithTheSameName()
    {
        const string first = """
            #nullable enable

            using Funcky.Internal;

            namespace Funcky.Extensions
            {
                [OrNoneFromTryPattern(typeof(First), nameof(First.TryParse))]
                public static partial class ParseExtensions
                {
                }

                public sealed class First
                {
                    public static bool TryParse(string candidate, out First result)
                    {
                        result = default!;
                        return false;
                    }
                }
            }
            """;

        const string second = """
            #nullable enable

            using Funcky.Internal;

            namespace Funcky.Extensions
            {
                [OrNoneFromTryPattern(typeof(Second), nameof(Second.TryParse))]
                public static partial class ParseExtensions
                {
                }

                public sealed class Second
                {
                    public static bool TryParse(string candidate, out Second result)
                    {
                        result = default!;
                        return false;
                    }
                }
            }
            """;

        return TestHelper.Verify(
            new TestHelper.SourceFile("Alpha/ParseExtensions.cs", first),
            new TestHelper.SourceFile("Beta/ParseExtensions.cs", second),
            new TestHelper.SourceFile("Option.cs", OptionSource));
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
