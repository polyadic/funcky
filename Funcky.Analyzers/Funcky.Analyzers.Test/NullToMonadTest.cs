using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.BuiltinAnalyzers.NullToMonadAnalyzer, Funcky.BuiltinAnalyzers.NullToOptionNoneCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed class NullToMonadTest
{
    private const string MonadStubs = """

        namespace Funcky.Monads
        {
            public readonly struct Option<TItem>
            {
                public static Option<TItem> None { get; } = default;
                public static implicit operator Option<TItem>(TItem item) => default;
                public static bool operator ==(Option<TItem> left, Option<TItem> right) => true;
                public static bool operator !=(Option<TItem> left, Option<TItem> right) => false;
                public override bool Equals(object obj) => true;
                public override int GetHashCode() => 0;
            }

            public static class Option
            {
                public static Option<TItem> Some<TItem>(TItem item) => default;
                public static Option<TItem> Return<TItem>(TItem item) => default;
                public static Option<TItem> FromNullable<TItem>(TItem item) where TItem : class => default;
            }

            public readonly struct Result<TValidResult>
            {
                public static implicit operator Result<TValidResult>(TValidResult result) => default;
                public static Result<TValidResult> Error(System.Exception exception) => default;
                public static bool operator ==(Result<TValidResult> left, Result<TValidResult> right) => true;
                public static bool operator !=(Result<TValidResult> left, Result<TValidResult> right) => false;
                public override bool Equals(object obj) => true;
                public override int GetHashCode() => 0;
            }

            public static class Result
            {
                public static Result<TValidResult> Ok<TValidResult>(TValidResult result) => default;
                public static Result<TValidResult> Return<TValidResult>(TValidResult result) => default;
            }

            public readonly struct Either<TLeft, TRight>
            {
                public static implicit operator Either<TLeft, TRight>(TRight right) => default;
                public static Either<TLeft, TRight> Left(TLeft left) => default;
                public static Either<TLeft, TRight> Right(TRight right) => default;
            }

            public static class Either<TLeft>
            {
                public static Either<TLeft, TRight> Return<TRight>(TRight right) => default;
            }
        }
        """;

    [Fact]
    public async Task NullAssignedToOptionIsReportedAndFixed()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<string> M()
                {
                    Option<string> option = {|λ0005:null|};
                    return option;
                }
            }
            """;
        const string fixedCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<string> M()
                {
                    Option<string> option = Option<string>.None;
                    return option;
                }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubs, fixedCode + MonadStubs);
    }

    [Fact]
    public async Task NullReturnedAndPassedAsArgumentIsReported()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<string> M() => {|λ0005:null|};

                public static void N() => Consume({|λ0005:null|});

                private static void Consume(Option<string> option) { }
            }
            """;
        const string fixedCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<string> M() => Option<string>.None;

                public static void N() => Consume(Option<string>.None);

                private static void Consume(Option<string> option) { }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubs, fixedCode + MonadStubs);
    }

    [Fact]
    public async Task ComparisonWithNullIsReportedAndFixed()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static bool M(Option<string> option) => option == {|λ0005:null|} || {|λ0005:null|} != option;
            }
            """;
        const string fixedCode = """
            using Funcky.Monads;

            public static class C
            {
                public static bool M(Option<string> option) => option == Option<string>.None || Option<string>.None != option;
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubs, fixedCode + MonadStubs);
    }

    [Fact]
    public async Task NullInConditionalBranchIsReportedAndFixed()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<string> M(bool condition) => condition ? "hello" : {|λ0005:null|};

                public static Option<string> N(bool condition) => condition ? ({|λ0005:null|}) : (string)"hello";
            }
            """;
        const string fixedCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<string> M(bool condition) => condition ? "hello" : Option<string>.None;

                public static Option<string> N(bool condition) => condition ? (Option<string>.None) : (string)"hello";
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubs, fixedCode + MonadStubs);
    }

    [Fact]
    public async Task NullInSwitchExpressionArmAndNullCoalescingFallbackIsReported()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<string> M(int value)
                    => value switch
                    {
                        1 => "one",
                        _ => {|λ0005:null|},
                    };

                public static Option<string> N(string value) => value ?? {|λ0005:null|};
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
                        _ => Option<string>.None,
                    };

                public static Option<string> N(string value) => value ?? Option<string>.None;
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubs, fixedCode + MonadStubs);
    }

    [Fact]
    public async Task NullPassedToOptionSomeOrReturnIsReportedAndTheCallReplaced()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<string> M() => Option.Some<string>({|λ0005:null|});

                public static Option<string> N() => Option.Return<string>({|λ0005:null|});
            }
            """;
        const string fixedCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<string> M() => Option<string>.None;

                public static Option<string> N() => Option<string>.None;
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubs, fixedCode + MonadStubs);
    }

    [Fact]
    public async Task NullInConditionalArgumentOfOptionSomeIsReportedWithoutFix()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<string> M(bool condition) => Option.Some(condition ? "hello" : {|λ0005:null|});
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + MonadStubs);
    }

    [Fact]
    public async Task NullConvertedToResultAndEitherIsReportedWithoutFix()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Result<string> R(bool condition) => condition ? "hello" : {|λ0005:null|};

                public static Either<int, string> E() => {|λ0005:null|};

                public static Result<string> F() => Result.Ok<string>({|λ0005:null|});

                public static Result<string> G() => Result<string>.Error({|λ0005:null|});

                public static Either<string, int> H() => Either<string, int>.Left({|λ0005:null|});

                public static Either<int, string> I() => Either<int, string>.Right({|λ0005:null|});

                public static Either<int, string> J() => Either<int>.Return<string>({|λ0005:null|});
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubs, inputCode + MonadStubs);
    }

    [Fact]
    public async Task NullBehindACastIsReportedWithoutFix()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static void M()
                {
                    Option<string> text = (string){|λ0005:null|};
                    Option<object> boxed = (object){|λ0005:null|};
                }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubs, inputCode + MonadStubs);
    }

    [Fact]
    public async Task NullConvertedToResultInsideAnOptionIsReportedWithoutFix()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<bool> M(Result<string> result) => result == {|λ0005:null|};
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubs, inputCode + MonadStubs);
    }

    [Fact]
    public async Task LiftedConversionToANullableMonadIsNotReported()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static void M()
                {
                    Option<int>? option = (int?)null;
                    Result<int>? result = (int?)null;
                    Either<string, int>? either = (int?)null;
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + MonadStubs);
    }

    [Fact]
    public async Task NullFromAVariableOrAnotherMethodIsNotReported()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Option<string> M(string value) => value;

                public static Option<string> N() => Option.Some(Text());

                public static Option<string> O() => Option.FromNullable<string>(null);

                public static Option<string> P() => default;

                public static Option<string> Q(bool condition) => condition ? Option<string>.None : "hello";

                public static Option<string> R(Option<string> option) => option == Option<string>.None ? "hello" : option;

                private static string Text() => null;
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + MonadStubs);
    }

    [Fact]
    public async Task NullConvertedToAnUnrelatedTypeIsNotReported()
    {
        const string inputCode = """
            public static class C
            {
                public static string M() => null;

                public static Wrapper N() => null;

                public static Wrapper O(bool condition) => condition ? "hello" : null;
            }

            public sealed class Wrapper
            {
                public static implicit operator Wrapper(string value) => new Wrapper();
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + MonadStubs);
    }
}
