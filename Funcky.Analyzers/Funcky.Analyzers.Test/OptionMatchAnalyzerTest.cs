using Xunit;
using static Funcky.Analyzers.OptionMatchAnalyzer;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.OptionMatchAnalyzer, Funcky.Analyzers.OptionMatchToOrElseCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed partial class OptionMatchAnalyzerTest
{
    [Fact]
    public async Task IssuesWarningForReimplementationOfGetOrElse()
    {
        const string inputCode =
            """
            #nullable enable

            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(Option<int> optionOfInt)
                {
                    optionOfInt.Match(none: 42, some: x => x);
                    optionOfInt.Match(some: x => x, none: 42);
                    optionOfInt.Match(none: 42, some: Identity);
                    optionOfInt.Match(none: () => 42, some: x => x);
                    Option.Some(10).Match(none: 42, some: x => x);
                }

                public static void Generic<TItem>(Option<TItem> option, TItem fallback)
                    where TItem : notnull
                {
                    option.Match(none: fallback, some: x => x);
                }
            }
            """;
        const string fixedCode =
            """
            #nullable enable

            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(Option<int> optionOfInt)
                {
                    optionOfInt.GetOrElse(42);
                    optionOfInt.GetOrElse(42);
                    optionOfInt.GetOrElse(42);
                    optionOfInt.GetOrElse(() => 42);
                    Option.Some(10).GetOrElse(42);
                }

                public static void Generic<TItem>(Option<TItem> option, TItem fallback)
                    where TItem : notnull
                {
                    option.GetOrElse(fallback);
                }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(
            inputCode + Environment.NewLine + OptionStubCode,
            new[]
            {
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(10, 9, 10, 50),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(11, 9, 11, 50),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(12, 9, 12, 52),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(13, 9, 13, 56),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(14, 9, 14, 54),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(20, 9, 20, 51),
            },
            fixedCode + Environment.NewLine + OptionStubCode);
    }

    [Fact]
    public async Task IssuesWarningForReimplementationOfOrElse()
    {
        const string inputCode =
            """
            #nullable enable

            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(Option<int> optionOfInt, Option<int> fallback)
                {
                    optionOfInt.Match(none: fallback, some: x => Option.Return(x));
                    optionOfInt.Match(none: fallback, some: x => Option.Some(x));
                    optionOfInt.Match(none: fallback, some: x => x);
                    optionOfInt.Match(some: x => Option.Return(x), none: fallback);
                    optionOfInt.Match(none: fallback, some: Option.Return);
                    optionOfInt.Match(none: fallback, some: Option.Some);
                    optionOfInt.Match(none: () => fallback, some: Option.Return);
                }

                public static void Generic<TItem>(Option<TItem> option, Option<TItem> fallback)
                    where TItem : notnull
                {
                    option.Match(none: fallback, some: Option.Return);
                }
            }
            """;
        const string fixedCode =
            """
            #nullable enable

            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(Option<int> optionOfInt, Option<int> fallback)
                {
                    optionOfInt.OrElse(fallback);
                    optionOfInt.OrElse(fallback);
                    optionOfInt.OrElse(fallback);
                    optionOfInt.OrElse(fallback);
                    optionOfInt.OrElse(fallback);
                    optionOfInt.OrElse(fallback);
                    optionOfInt.OrElse(() => fallback);
                }

                public static void Generic<TItem>(Option<TItem> option, Option<TItem> fallback)
                    where TItem : notnull
                {
                    option.OrElse(fallback);
                }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(
            inputCode + Environment.NewLine + OptionStubCode,
            new[]
            {
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(10, 9, 10, 71),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(11, 9, 11, 69),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(12, 9, 12, 56),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(13, 9, 13, 71),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(14, 9, 14, 63),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(15, 9, 15, 61),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(16, 9, 16, 69),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(22, 9, 22, 58),
            },
            fixedCode + Environment.NewLine + OptionStubCode);
    }

    [Fact]
    public async Task IssuesWarningForReimplementationOfSelectMany()
    {
        const string inputCode =
            """
            #nullable enable

            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(Option<int> optionOfInt)
                {
                    optionOfInt.Match(none: Option<int>.None, some: x => Option.Return(x + 10));
                    optionOfInt.Match(some: x => Option.Return(x + 10), none: Option<int>.None);
                }

                public static void Generic<TItem>(Option<TItem> option, System.Func<TItem, Option<TItem>> selector)
                    where TItem : notnull
                {
                    option.Match(none: Option<TItem>.None, some: selector);
                }
            }
            """;
        const string fixedCode =
            """
            #nullable enable

            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(Option<int> optionOfInt)
                {
                    optionOfInt.SelectMany(x => Option.Return(x + 10));
                    optionOfInt.SelectMany(x => Option.Return(x + 10));
                }

                public static void Generic<TItem>(Option<TItem> option, System.Func<TItem, Option<TItem>> selector)
                    where TItem : notnull
                {
                    option.SelectMany(selector);
                }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(
            inputCode + Environment.NewLine + OptionStubCode,
            new[]
            {
                VerifyCS.Diagnostic(PreferSelectMany).WithSpan(10, 9, 10, 84),
                VerifyCS.Diagnostic(PreferSelectMany).WithSpan(11, 9, 11, 84),
                VerifyCS.Diagnostic(PreferSelectMany).WithSpan(17, 9, 17, 63),
            },
            fixedCode + Environment.NewLine + OptionStubCode);
    }

    [Fact]
    public async Task IssuesNoWarningForRegularUsesOfMatch()
    {
        const string inputCode =
            """
            #nullable enable

            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(Option<int> optionOfInt, Option<string> optionOfString)
                {
                    optionOfInt.Match(none: 42, some: x => x + 1);
                    optionOfInt.Match(none: optionOfInt, some: x => Option.Return(x + 1));
                    optionOfInt.Match(none: optionOfInt, some: x => Option.Return(x + 1));
                    optionOfInt.Match(none: optionOfInt, some: x => x + 1);
                    optionOfInt.Match(none: Option<string>.None, some: x => x.ToString());
                    optionOfString.Match(none: (string?)null, some: x => x + "foo");
                    optionOfInt.Match(none: (int?)null, some: x => x + 1);
                }

                public static void Generic<TItem>(Option<TItem> option, TItem? fallback)
                    where TItem : class
                {
                    option.Match(none: fallback, some: x => x);
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + OptionStubCode);
    }
}
