using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.ExtensionForms.PreferStaticParseOrNoneAnalyzer, Funcky.Analyzers.PreferStaticParseOrNoneCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed class PreferStaticParseOrNoneTest
{
    // language=csharp
    private const string StubCode =
        """
        namespace Funcky.Monads
        {
            public readonly struct Option<TItem>
                where TItem : notnull
            {
            }
        }

        namespace Funcky.Extensions
        {
            using System;
            using Funcky.Monads;

            public static class ParseExtensions
            {
                public static Option<int> ParseInt32OrNone(this string candidate) => default;

                public static Option<int> ParseInt32OrNone(this ReadOnlySpan<char> candidate) => default;

                public static Option<DateTime> ParseExactDateTimeOrNone(this string candidate, string format, IFormatProvider? provider) => default;

                public static Option<TEnum> ParseEnumOrNone<TEnum>(this string candidate, bool ignoreCase = false) where TEnum : struct, Enum => default;

                public static Option<Guid> ParseGuidOrNone(this string candidate) => default;
            }

            public static class Int32ParseExtensions
            {
                extension(int)
                {
                    public static Option<int> ParseOrNone(string candidate) => default;

                    public static Option<int> ParseOrNone(ReadOnlySpan<char> candidate) => default;
                }
            }

            public static class DateTimeParseExtensions
            {
                extension(DateTime)
                {
                    public static Option<DateTime> ParseExactOrNone(string candidate, string format, IFormatProvider? provider) => default;
                }
            }

            public static class EnumParseExtensions
            {
                extension<TEnum>(TEnum)
                    where TEnum : struct, Enum
                {
                    public static Option<TEnum> ParseOrNone(string candidate, bool ignoreCase = false) => default;
                }
            }
        }
        """;

    [Fact]
    public async Task ReportsTheStringExtensionsAndRewritesThemToTheStaticMember()
    {
        const string inputCode =
            """
            #nullable enable
            using System;
            using System.Globalization;
            using Funcky.Extensions;
            using Funcky.Monads;

            public enum Color { Red, Green }

            public static class C
            {
                public static void M(string text, ReadOnlySpan<char> span)
                {
                    Option<int> a = {|λ1201:text.ParseInt32OrNone()|};
                    Option<int> b = {|λ1201:span.ParseInt32OrNone()|};
                    Option<DateTime> c = {|λ1201:text.ParseExactDateTimeOrNone("O", CultureInfo.InvariantCulture)|};
                    Option<DateTime> d = {|λ1201:text.ParseExactDateTimeOrNone(provider: null, format: "O")|};
                    Option<Color> e = {|λ1201:text.ParseEnumOrNone<Color>(ignoreCase: true)|};
                    Option<Color> f = {|λ1201:text.ParseEnumOrNone<Color>()|};
                    Option<int> g = {|λ1201:ParseExtensions.ParseInt32OrNone(text)|};
                    Option<int> h = {|λ1201:text.Trim().ParseInt32OrNone()|};
                }
            }
            """;
        const string fixedCode =
            """
            #nullable enable
            using System;
            using System.Globalization;
            using Funcky.Extensions;
            using Funcky.Monads;

            public enum Color { Red, Green }

            public static class C
            {
                public static void M(string text, ReadOnlySpan<char> span)
                {
                    Option<int> a = int.ParseOrNone(text);
                    Option<int> b = int.ParseOrNone(span);
                    Option<DateTime> c = DateTime.ParseExactOrNone(text, "O", CultureInfo.InvariantCulture);
                    Option<DateTime> d = DateTime.ParseExactOrNone(text, provider: null, format: "O");
                    Option<Color> e = Color.ParseOrNone(text, ignoreCase: true);
                    Option<Color> f = Color.ParseOrNone(text);
                    Option<int> g = int.ParseOrNone(text);
                    Option<int> h = int.ParseOrNone(text.Trim());
                }
            }
            """;

        await VerifyCS.VerifyCodeFixAsync(inputCode + Environment.NewLine + StubCode, fixedCode + Environment.NewLine + StubCode);
    }

    [Fact]
    public async Task DoesNotReportWithoutAStaticCounterpartOrForMethodGroups()
    {
        const string inputCode =
            """
            #nullable enable
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using Funcky.Extensions;
            using Funcky.Monads;

            public static class C
            {
                public static void M(string? text, IEnumerable<string> texts)
                {
                    Option<Guid> a = text!.ParseGuidOrNone();
                    IEnumerable<Option<int>> b = texts.Select(ParseExtensions.ParseInt32OrNone);
                    Option<int>? c = text?.ParseInt32OrNone();
                    Option<int>? d = text?.Trim().ParseInt32OrNone();
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + StubCode);
    }

    [Fact]
    public async Task KeepsTheCommentsInsideTheArgumentList()
    {
        const string inputCode =
            """
            #nullable enable
            using System;
            using System.Globalization;
            using Funcky.Extensions;
            using Funcky.Monads;

            public static class C
            {
                public static void M(string text, IFormatProvider provider)
                {
                    Option<int> a = {|λ1201:text.ParseInt32OrNone(/* explanation */)|};
                    Option<DateTime> b = {|λ1201:text.ParseExactDateTimeOrNone("O", /* provider rationale */ provider)|};
                    Option<DateTime> c = {|λ1201:text.ParseExactDateTimeOrNone(
                        "O", // the round-trip format
                        provider)|};
                    Option<int> d = {|λ1201:ParseExtensions.ParseInt32OrNone(/* the candidate */ text)|};
                }
            }
            """;
        const string fixedCode =
            """
            #nullable enable
            using System;
            using System.Globalization;
            using Funcky.Extensions;
            using Funcky.Monads;

            public static class C
            {
                public static void M(string text, IFormatProvider provider)
                {
                    Option<int> a = int.ParseOrNone(/* explanation */text);
                    Option<DateTime> b = DateTime.ParseExactOrNone(text, "O", /* provider rationale */ provider);
                    Option<DateTime> c = DateTime.ParseExactOrNone(
                        text, "O", // the round-trip format
                        provider);
                    Option<int> d = int.ParseOrNone(/* the candidate */ text);
                }
            }
            """;

        await VerifyCS.VerifyCodeFixAsync(inputCode + Environment.NewLine + StubCode, fixedCode + Environment.NewLine + StubCode);
    }

    // Two diagnostics per test: with three or more fixes that all add the using, the batch fixer needs a second pass.
    [Fact]
    public async Task AddsTheUsingWhenTheExtensionIsCalledFullyQualified()
    {
        const string inputCode =
            """
            #nullable enable
            using System;
            using Funcky.Monads;

            public static class C
            {
                public static void M(string text)
                {
                    Option<int> a = {|λ1201:Funcky.Extensions.ParseExtensions.ParseInt32OrNone(text)|};
                    Option<int> b = {|λ1201:Funcky.Extensions.ParseExtensions.ParseInt32OrNone(candidate: text)|};
                }
            }
            """;
        const string fixedCode =
            """
            #nullable enable
            using System;
            using Funcky.Monads;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(string text)
                {
                    Option<int> a = int.ParseOrNone(text);
                    Option<int> b = int.ParseOrNone(text);
                }
            }
            """;

        await VerifyCS.VerifyCodeFixAsync(inputCode + Environment.NewLine + StubCode, fixedCode + Environment.NewLine + StubCode);
    }

    [Fact]
    public async Task AddsTheUsingWhenTheExtensionIsImportedWithUsingStatic()
    {
        const string inputCode =
            """
            #nullable enable
            using System;
            using Funcky.Monads;
            using static Funcky.Extensions.ParseExtensions;

            public enum Color { Red, Green }

            public static class C
            {
                public static void M(string text)
                {
                    Option<Color> a = {|λ1201:text.ParseEnumOrNone<Color>(ignoreCase: true)|};
                    Option<DateTime> b = {|λ1201:text.ParseExactDateTimeOrNone("O", null)|};
                }
            }
            """;
        const string fixedCode =
            """
            #nullable enable
            using System;
            using Funcky.Monads;
            using static Funcky.Extensions.ParseExtensions;
            using Funcky.Extensions;

            public enum Color { Red, Green }

            public static class C
            {
                public static void M(string text)
                {
                    Option<Color> a = Color.ParseOrNone(text, ignoreCase: true);
                    Option<DateTime> b = DateTime.ParseExactOrNone(text, "O", null);
                }
            }
            """;

        await VerifyCS.VerifyCodeFixAsync(inputCode + Environment.NewLine + StubCode, fixedCode + Environment.NewLine + StubCode);
    }

    [Fact]
    public async Task DoesNotReportWithoutAStaticCounterpartWhenTheNamespaceIsNotImported()
    {
        const string inputCode =
            """
            #nullable enable
            using System;
            using Funcky.Monads;

            public static class C
            {
                public static void M(string text)
                {
                    Option<Guid> a = Funcky.Extensions.ParseExtensions.ParseGuidOrNone(text);
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + StubCode);
    }

    [Fact]
    public async Task DoesNotReportBelowCSharp14()
    {
        const string inputCode =
            """
            using Funcky.Extensions;
            using Funcky.Monads;

            public static class C
            {
                public static void M(string text)
                {
                    Option<int> a = text.ParseInt32OrNone();
                }
            }

            namespace Funcky.Monads
            {
                public readonly struct Option<TItem>
                    where TItem : notnull
                {
                }
            }

            namespace Funcky.Extensions
            {
                using Funcky.Monads;

                public static class ParseExtensions
                {
                    public static Option<int> ParseInt32OrNone(this string candidate) => default;
                }

                public static class Int32ParseExtensions
                {
                    public static Option<int> ParseOrNone(string candidate) => default;
                }
            }
            """;

        await new VerifyCS.Test { TestCode = inputCode, LanguageVersion = LanguageVersion.CSharp13 }.RunAsync(CancellationToken.None);
    }
}
