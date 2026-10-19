using Xunit;
using static Funcky.Analyzers.Test.Fallback.FallbackStubs;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.Fallback.ThrowInEagerFallbackAnalyzer, Funcky.Analyzers.Fallback.ThrowInEagerFallbackCodeFix>;

namespace Funcky.Analyzers.Test.Fallback;

public sealed class ThrowInEagerFallbackTest
{
    [Fact]
    public async Task ThrowInNullCoalescingFallbackIsReportedAndWrappedInLambda()
    {
        const string inputCode =
            """
            #nullable enable

            using System;
            using Funcky.Monads;

            public static class C
            {
                public static string M(Option<string> option, string? fallback)
                    => option.GetOrElse({|λ1014:fallback ?? throw new Exception("Unexpected")|});
            }
            """;
        const string fixedCode =
            """
            #nullable enable

            using System;
            using Funcky.Monads;

            public static class C
            {
                public static string M(Option<string> option, string? fallback)
                    => option.GetOrElse(() => fallback ?? throw new Exception("Unexpected"));
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubCode, fixedCode + MonadStubCode);
    }

    [Fact]
    public async Task ThrowInConditionalFallbackIsReported()
    {
        const string inputCode =
            """
            #nullable enable

            using System;
            using Funcky.Monads;

            public static class C
            {
                public static int M(Option<int> option, bool condition)
                    => option.GetOrElse({|λ1014:condition ? 42 : throw new Exception()|});
            }
            """;
        const string fixedCode =
            """
            #nullable enable

            using System;
            using Funcky.Monads;

            public static class C
            {
                public static int M(Option<int> option, bool condition)
                    => option.GetOrElse(() => condition ? 42 : throw new Exception());
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubCode, fixedCode + MonadStubCode);
    }

    [Fact]
    public async Task ResultAndEitherGetALambdaWithADiscardedParameter()
    {
        const string inputCode =
            """
            #nullable enable

            using System;
            using Funcky.Monads;

            public static class C
            {
                public static int R(Result<int> result, int? fallback)
                    => result.GetOrElse({|λ1014:fallback ?? throw new Exception()|});

                public static Either<string, int> E(Either<string, int> either, bool condition)
                    => either.OrElse({|λ1014:condition ? Either<string, int>.Left("left") : throw new Exception()|});
            }
            """;
        const string fixedCode =
            """
            #nullable enable

            using System;
            using Funcky.Monads;

            public static class C
            {
                public static int R(Result<int> result, int? fallback)
                    => result.GetOrElse(_ => fallback ?? throw new Exception());

                public static Either<string, int> E(Either<string, int> either, bool condition)
                    => either.OrElse(_ => condition ? Either<string, int>.Left("left") : throw new Exception());
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubCode, fixedCode + MonadStubCode);
    }

    [Fact]
    public async Task EagerNoneArgumentOfMatchIsReportedAndKeepsItsName()
    {
        const string inputCode =
            """
            #nullable enable

            using System;
            using Funcky.Monads;

            public static class C
            {
                public static string M(Option<int> option, string? fallback)
                    => option.Match(none: {|λ1014:fallback ?? throw new Exception()|}, some: x => x.ToString());
            }
            """;
        const string fixedCode =
            """
            #nullable enable

            using System;
            using Funcky.Monads;

            public static class C
            {
                public static string M(Option<int> option, string? fallback)
                    => option.Match(none: () => fallback ?? throw new Exception(), some: x => x.ToString());
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubCode, fixedCode + MonadStubCode);
    }

    [Fact]
    public async Task ThrowInsideALambdaIsNotReported()
    {
        const string inputCode =
            """
            #nullable enable

            using System;
            using Funcky.Monads;

            public static class C
            {
                public static string M(Option<string> option, string? fallback)
                    => option.GetOrElse(() => fallback ?? throw new Exception());

                public static int N(Result<int> result)
                    => result.GetOrElse(exception => throw exception);

                public static string O(Option<int> option)
                    => option.Match(none: () => throw new Exception(), some: x => x.ToString());
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + MonadStubCode);
    }

    [Fact]
    public async Task ThrowInArgumentOfAMethodWithoutLazyOverloadIsNotReported()
    {
        const string inputCode =
            """
            #nullable enable

            using System;
            using Funcky.Monads;

            public static class C
            {
                public static void M(Option<int> option, int? value)
                {
                    Consume(value ?? throw new Exception());
                    option.AndThen(x => value ?? throw new Exception());
                }

                private static void Consume(int value) { }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + MonadStubCode);
    }
}
