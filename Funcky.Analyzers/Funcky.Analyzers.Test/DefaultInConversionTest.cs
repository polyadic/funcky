using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.BuiltinAnalyzers.DefaultInConversionAnalyzer, Funcky.BuiltinAnalyzers.DefaultToOptionNoneCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed class DefaultInConversionTest
{
    private const string MonadStubs = """

        namespace Funcky.Monads
        {
            public readonly struct Option<TItem>
            {
                public static Option<TItem> None { get; } = default;
                public static implicit operator Option<TItem>(TItem item) => default;
            }

            public readonly struct Result<TValidResult>
            {
                public static implicit operator Result<TValidResult>(TValidResult result) => default;
            }

            public readonly struct Either<TLeft, TRight>
            {
                public static implicit operator Either<TLeft, TRight>(TRight right) => default;
            }
        }
        """;

    [Fact]
    public async Task DefaultInConditionalExpressionConvertedToOptionIsReportedAndFixed()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<(string, int)> M(bool condition) => condition ? ("hello world", 42) : {|λ0004:default|};
            }
            """;
        const string fixedCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<(string, int)> M(bool condition) => condition ? ("hello world", 42) : Option<(string, int)>.None;
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubs, fixedCode + MonadStubs);
    }

    [Fact]
    public async Task DefaultInEitherBranchIsReported()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<int> M(bool condition)
                {
                    Option<int> option = condition ? {|λ0004:default|} : 42;
                    return option;
                }
            }
            """;
        const string fixedCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<int> M(bool condition)
                {
                    Option<int> option = condition ? Option<int>.None : 42;
                    return option;
                }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubs, fixedCode + MonadStubs);
    }

    [Fact]
    public async Task DefaultInNestedConditionalExpressionIsReported()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<int> M(bool a, bool b) => a ? (b ? 1 : {|λ0004:default|}) : 2;
            }
            """;
        const string fixedCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<int> M(bool a, bool b) => a ? (b ? 1 : Option<int>.None) : 2;
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubs, fixedCode + MonadStubs);
    }

    [Fact]
    public async Task DefaultInSwitchExpressionArmIsReported()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<string> M(int value)
                    => value switch
                    {
                        1 => "one",
                        2 => "two",
                        _ => {|λ0004:default|},
                    };
            }
            """;
        const string fixedCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<string> M(int value)
                    => value switch
                    {
                        1 => "one",
                        2 => "two",
                        _ => Option<string>.None,
                    };
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubs, fixedCode + MonadStubs);
    }

    [Fact]
    public async Task DefaultAsNullCoalescingFallbackIsReported()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<int> M(int? value) => value ?? {|λ0004:default|};
            }
            """;
        const string fixedCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<int> M(int? value) => value ?? Option<int>.None;
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubs, fixedCode + MonadStubs);
    }

    [Fact]
    public async Task DefaultPassedAsArgumentIsReported()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static void M(bool condition) => Consume(condition ? 42 : {|λ0004:default|});

                private static void Consume(Option<int> option) { }
            }
            """;
        const string fixedCode = """
            using Funcky.Monads;

            public static class C
            {
                public static void M(bool condition) => Consume(condition ? 42 : Option<int>.None);

                private static void Consume(Option<int> option) { }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubs, fixedCode + MonadStubs);
    }

    [Fact]
    public async Task DefaultConvertedToResultAndEitherIsReportedWithoutFix()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Result<int> R(bool condition) => condition ? 42 : {|λ0004:default|};

                public static Either<string, int> E(bool condition) => condition ? 42 : {|λ0004:default|};
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubs, inputCode + MonadStubs);
    }

    [Fact]
    public async Task DefaultDirectlyConvertedToOptionIsNotReported()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<int> M() => default;

                public static Option<int> N(bool condition)
                {
                    if (condition) { return 42; }
                    else { return default; }
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + MonadStubs);
    }

    [Fact]
    public async Task ConditionalExpressionWithOptionTypedBranchesIsNotReported()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<int> M(bool condition) => condition ? Option<int>.None : default;

                public static Option<int> N(bool condition) => condition ? 42 : Option<int>.None;
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + MonadStubs);
    }

    [Fact]
    public async Task TypedDefaultExpressionIsNotReported()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<int> M(bool condition) => condition ? 42 : default(int);
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + MonadStubs);
    }

    [Fact]
    public async Task DefaultInConditionalExpressionWithoutConversionToMonadIsNotReported()
    {
        const string inputCode = """
            public static class C
            {
                public static int M(bool condition) => condition ? 42 : default;

                public static long N(bool condition) => condition ? 42 : default;
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + MonadStubs);
    }
}
