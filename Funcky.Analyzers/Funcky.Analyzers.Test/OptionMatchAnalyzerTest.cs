using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.OptionMatchAnalyzer, Funcky.Analyzers.OptionMatchToGetOrElseCodeFix>;

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
            }

            public static class Option
            {
                public static Option<TItem> Some<TItem>(TItem value) where TItem : notnull => default;
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
                public static void M(Option<int> optionOfInt, Option<string> optionOfString)
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
                public static void M(Option<int> optionOfInt, Option<string> optionOfString)
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
                VerifyCS.Diagnostic().WithSpan(10, 9, 10, 50),
                VerifyCS.Diagnostic().WithSpan(11, 9, 11, 50),
                VerifyCS.Diagnostic().WithSpan(12, 9, 12, 52),
                VerifyCS.Diagnostic().WithSpan(13, 9, 13, 56),
                VerifyCS.Diagnostic().WithSpan(14, 9, 14, 54),
                VerifyCS.Diagnostic().WithSpan(20, 9, 20, 51),
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
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + OptionCode);
    }
}
