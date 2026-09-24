#pragma warning disable SA1118 // StyleCop support for collection expressions is missing
using Xunit;
using static Funcky.Analyzers.AlternativeMonad.AlternativeMonadAnalyzer;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.AlternativeMonad.AlternativeMonadAnalyzer, Funcky.Analyzers.AlternativeMonad.MatchToOrElseCodeFix>;

namespace Funcky.Analyzers.Test.AlternativeMonad;

public sealed partial class AlternativeMonadAnalyzerTest
{
    [Fact]
    public async Task IssuesWarningForReimplementationOfSelect()
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
                    optionOfInt.Match(none: Option<int>.None, some: x => x + 10);
                    optionOfInt.Match(some: x => x + 10, none: Option<int>.None);
                    optionOfInt.Match(none: Option<string>.None, some: x => x.ToString());
                    optionOfInt.Match(none: Option<string>.None, some: x => { return x.ToString(); });
                }

                public static void M2(Either<string, int> eitherOfInt)
                {
                    eitherOfInt.Match(left: Either<string, int>.Left, right: x => x * 2);
                    eitherOfInt.Match(left: Either<string, string>.Left, right: x => x.ToString());
                }

                public static void M3(Result<int> resultOfInt)
                {
                    resultOfInt.Match(error: Result<int>.Error, ok: x => x * 2);
                    resultOfInt.Match(error: Result<string>.Error, ok: x => x.ToString());
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
                    optionOfInt.Select(x => x + 10);
                    optionOfInt.Select(x => x + 10);
                    optionOfInt.Select(x => x.ToString());
                    optionOfInt.Select(x => { return x.ToString(); });
                }

                public static void M2(Either<string, int> eitherOfInt)
                {
                    eitherOfInt.Select(x => x * 2);
                    eitherOfInt.Select(x => x.ToString());
                }

                public static void M3(Result<int> resultOfInt)
                {
                    resultOfInt.Select(x => x * 2);
                    resultOfInt.Select(x => x.ToString());
                }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(
            inputCode + Environment.NewLine + OptionStubCode,
            [
                VerifyCS.Diagnostic(PreferSelect).WithSpan(10, 9, 10, 69),
                VerifyCS.Diagnostic(PreferSelect).WithSpan(11, 9, 11, 69),
                VerifyCS.Diagnostic(PreferSelect).WithSpan(12, 9, 12, 78),
                VerifyCS.Diagnostic(PreferSelect).WithSpan(13, 9, 13, 90),
                VerifyCS.Diagnostic(PreferSelect).WithSpan(18, 9, 18, 77),
                VerifyCS.Diagnostic(PreferSelect).WithSpan(19, 9, 19, 87),
                VerifyCS.Diagnostic(PreferSelect).WithSpan(24, 9, 24, 68),
                VerifyCS.Diagnostic(PreferSelect).WithSpan(25, 9, 25, 78),
            ],
            fixedCode + Environment.NewLine + OptionStubCode);
    }

    [Fact]
    public async Task IssuesNoSelectWarningWhenSomeFunctionAlreadyReturnsTheMonad()
    {
        const string inputCode =
            """
            #nullable enable

            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(Option<int> optionOfInt, Option<int> fallback, System.Func<int, Option<int>> selector)
                {
                    optionOfInt.Match(none: Option<int>.None, some: x => Option.Return(x + 10));
                    optionOfInt.Match(none: Option<int>.None, some: x => (Option<int>)(x + 10));
                    optionOfInt.Match(none: Option<int>.None, some: selector);
                    optionOfInt.Match(none: fallback, some: x => x + 10);
                    optionOfInt.Match(none: 42, some: x => x + 10);
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(
            inputCode + Environment.NewLine + OptionStubCode,
            [
                VerifyCS.Diagnostic(PreferSelectMany).WithSpan(10, 9, 10, 84),
                VerifyCS.Diagnostic(PreferSelectMany).WithSpan(11, 9, 11, 84),
                VerifyCS.Diagnostic(PreferSelectMany).WithSpan(12, 9, 12, 66),
            ]);
    }
}
