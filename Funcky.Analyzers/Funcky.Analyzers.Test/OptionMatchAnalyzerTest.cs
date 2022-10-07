using Xunit;
using static Funcky.Analyzers.OptionMatchAnalyzer;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.OptionMatchAnalyzer, Funcky.Analyzers.OptionMatchToOrElseCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed class OptionMatchAnalyzerTest
{
    private const string OptionCode =
        """
        namespace Funcky.Monads
        {
            public readonly struct Option<TItem>
                where TItem : notnull
            {
                public TResult Match<TResult>(TResult none, System.Func<TItem, TResult> some) => default!;

                public TResult Match<TResult>(System.Func<TResult> none, System.Func<TItem, TResult> some) => default!;

                public TItem GetOrElse(TItem fallback) => default!;
        
                public TItem GetOrElse(System.Func<TItem> fallback) => default!;
        
                public Option<TItem> OrElse(Option<TItem> fallback) => default!;
        
                public Option<TItem> OrElse(System.Func<Option<TItem>> fallback) => default!;
            }

            public static class Option
            {
                public static Option<TItem> Some<TItem>(TItem value) where TItem : notnull => default;

                public static Option<TItem> Return<TItem>(TItem value) where TItem : notnull => default;
            }
        }

        namespace Funcky
        {
            public static class Functional
            {
                public static T Identity<T>(T x) => x;
            }
        }
        """;

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
            inputCode + Environment.NewLine + OptionCode,
            new[]
            {
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(10, 9, 10, 50),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(11, 9, 11, 50),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(12, 9, 12, 52),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(13, 9, 13, 56),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(14, 9, 14, 54),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(20, 9, 20, 51),
            },
            fixedCode + Environment.NewLine + OptionCode);
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
            inputCode + Environment.NewLine + OptionCode,
            new[]
            {
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(10, 9, 10, 71),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(11, 9, 11, 69),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(12, 9, 12, 71),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(13, 9, 13, 63),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(14, 9, 14, 61),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(15, 9, 15, 69),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(21, 9, 21, 58),
            },
            fixedCode + Environment.NewLine + OptionCode);
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
                    optionOfInt.Match((int?)null, some: x => x);
                    optionOfString.Match((string?)null, some: x => x);
                    optionOfString.Match((string?)null, some: Identity);
                    optionOfInt.Match(none: optionOfInt, some: x => Option.Return(x + 1));
                    optionOfInt.Match(none: optionOfInt, some: x => Option.Return(x + 1));
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + OptionCode);
    }
}
