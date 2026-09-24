#pragma warning disable SA1118 // StyleCop support for collection expressions is missing
using Xunit;
using static Funcky.Analyzers.AlternativeMonad.AlternativeMonadAnalyzer;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.AlternativeMonad.AlternativeMonadAnalyzer, Funcky.Analyzers.AlternativeMonad.MatchToOrElseCodeFix>;

namespace Funcky.Analyzers.Test.AlternativeMonad;

public sealed partial class AlternativeMonadAnalyzerTest
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
                public static void M1(Option<int> optionOfInt)
                {
                    optionOfInt.Match(none: 42, some: x => x);
                    optionOfInt.Match(some: x => x, none: 42);
                    optionOfInt.Match(none: 42, some: Identity);
                    optionOfInt.Match(none: () => 42, some: x => x);
                    Option.Some(10).Match(none: 42, some: x => x);
                    Option.Some("foo").Match(none: "bar", some: Identity);
                }

                public static void Generic<TItem>(Option<TItem> option, TItem fallback)
                    where TItem : notnull
                {
                    option.Match(none: fallback, some: x => x);
                }

                public static void M2(Either<string, int> eitherOfInt)
                {
                    eitherOfInt.Match(left: _ => 42, right: x => x);
                }

                public static void M3(Result<int> resultOfInt)
                {
                    resultOfInt.Match(error: _ => 42, ok: x => x);
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
                public static void M1(Option<int> optionOfInt)
                {
                    optionOfInt.GetOrElse(42);
                    optionOfInt.GetOrElse(42);
                    optionOfInt.GetOrElse(42);
                    optionOfInt.GetOrElse(() => 42);
                    Option.Some(10).GetOrElse(42);
                    Option.Some("foo").GetOrElse("bar");
                }

                public static void Generic<TItem>(Option<TItem> option, TItem fallback)
                    where TItem : notnull
                {
                    option.GetOrElse(fallback);
                }

                public static void M2(Either<string, int> eitherOfInt)
                {
                    eitherOfInt.GetOrElse(_ => 42);
                }

                public static void M3(Result<int> resultOfInt)
                {
                    resultOfInt.GetOrElse(_ => 42);
                }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(
            inputCode + Environment.NewLine + OptionStubCode,
            [
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(10, 9, 10, 50),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(11, 9, 11, 50),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(12, 9, 12, 52),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(13, 9, 13, 56),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(14, 9, 14, 54),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(15, 9, 15, 62),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(21, 9, 21, 51),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(26, 9, 26, 56),
                VerifyCS.Diagnostic(PreferGetOrElse).WithSpan(31, 9, 31, 54),
            ],
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
                public static void M1(Option<int> optionOfInt, Option<int> fallback)
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

                public static void M2(Either<string, int> eitherOfInt, Either<string, int> fallback)
                {
                    eitherOfInt.Match(right: Either<string>.Return, left: _ => fallback);
                }

                public static void M3(Result<int> resultOfInt, Result<int> fallback)
                {
                    resultOfInt.Match(ok: Result.Return, error: _ => fallback);
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
                public static void M1(Option<int> optionOfInt, Option<int> fallback)
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

                public static void M2(Either<string, int> eitherOfInt, Either<string, int> fallback)
                {
                    eitherOfInt.OrElse(_ => fallback);
                }

                public static void M3(Result<int> resultOfInt, Result<int> fallback)
                {
                    resultOfInt.OrElse(_ => fallback);
                }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(
            inputCode + Environment.NewLine + OptionStubCode,
            [
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(10, 9, 10, 71),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(11, 9, 11, 69),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(12, 9, 12, 56),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(13, 9, 13, 71),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(14, 9, 14, 63),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(15, 9, 15, 61),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(16, 9, 16, 69),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(22, 9, 22, 58),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(27, 9, 27, 77),
                VerifyCS.Diagnostic(PreferOrElse).WithSpan(32, 9, 32, 67),
            ],
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

                public static void M2(Either<string, int> eitherOfInt)
                {
                    eitherOfInt.Match(left: Either<string, int>.Left, right: x => Either<string>.Return(x * 2));
                }

                public static void M3(Result<int> resultOfInt)
                {
                    resultOfInt.Match(error: Result<int>.Error, ok: x => Result.Return(x * 2));
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

                public static void M2(Either<string, int> eitherOfInt)
                {
                    eitherOfInt.SelectMany(x => Either<string>.Return(x * 2));
                }

                public static void M3(Result<int> resultOfInt)
                {
                    resultOfInt.SelectMany(x => Result.Return(x * 2));
                }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(
            inputCode + Environment.NewLine + OptionStubCode,
            [
                VerifyCS.Diagnostic(PreferSelectMany).WithSpan(10, 9, 10, 84),
                VerifyCS.Diagnostic(PreferSelectMany).WithSpan(11, 9, 11, 84),
                VerifyCS.Diagnostic(PreferSelectMany).WithSpan(17, 9, 17, 63),
                VerifyCS.Diagnostic(PreferSelectMany).WithSpan(22, 9, 22, 100),
                VerifyCS.Diagnostic(PreferSelectMany).WithSpan(27, 9, 27, 83),
            ],
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

    [Fact]
    public async Task DoesNotMistakeReturnOfOuterParameterWithParameterOfAnonymousFunction()
    {
        const string inputCode =
            """
            #nullable enable

            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(Option<int> optionOfInt, int y)
                {
                    optionOfInt.Match(none: 42, some: x => y);
                    optionOfInt.Match(none: optionOfInt, some: x => Option.Return(y));
                    optionOfInt.Match(none: (int?)null, some: x => y);
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + OptionStubCode);
    }
}
