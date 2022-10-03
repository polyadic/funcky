using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpAnalyzerVerifier<Funcky.Analyzers.OptionMatchAnalyzer>;

namespace Funcky.Analyzers.Test;

public sealed class OptionMatchAnalyzerTest
{
    private const string OptionCode = """
        namespace Funcky.Monads
        {
            public readonly struct Option<TItem>
                where TItem : notnull
            {
                public TResult Match<TResult>(TResult none, System.Func<TItem, TResult> some) => default!;

                public TResult Match<TResult>(System.Func<TResult> none, System.Func<TItem, TResult> some) => default!;
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
    public async Task IssuesWarningForReimplementationOfTryGetValue()
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
                }

                public static void Generic<TItem>(Option<TItem> option, TItem fallback)
                    where TItem : notnull
                {
                    option.Match(none: fallback, some: x => x);
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(
            inputCode + Environment.NewLine + OptionCode,
            VerifyCS.Diagnostic().WithSpan(10, 9, 10, 50),
            VerifyCS.Diagnostic().WithSpan(11, 9, 11, 50),
            VerifyCS.Diagnostic().WithSpan(12, 9, 12, 52),
            VerifyCS.Diagnostic().WithSpan(13, 9, 13, 56),
            VerifyCS.Diagnostic().WithSpan(19, 9, 19, 51));
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
