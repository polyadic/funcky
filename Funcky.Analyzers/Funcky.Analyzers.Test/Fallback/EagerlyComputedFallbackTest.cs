using Xunit;
using static Funcky.Analyzers.Test.Fallback.FallbackStubs;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.Fallback.EagerlyComputedFallbackAnalyzer, Funcky.Analyzers.Fallback.EagerlyComputedFallbackCodeFix>;

namespace Funcky.Analyzers.Test.Fallback;

public sealed class EagerlyComputedFallbackTest
{
    [Fact]
    public async Task MethodCallAsFallbackIsReportedAndWrappedInLambda()
    {
        const string inputCode =
            """
            using Funcky.Monads;

            public static class C
            {
                public static int M(Option<int> option)
                    => option.GetOrElse({|λ1015:Compute()|});

                private static int Compute() => 42;
            }
            """;
        const string fixedCode =
            """
            using Funcky.Monads;

            public static class C
            {
                public static int M(Option<int> option)
                    => option.GetOrElse(() => Compute());

                private static int Compute() => 42;
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubCode, fixedCode + MonadStubCode);
    }

    [Fact]
    public async Task ObjectCreationInterpolationAndNestedCallsAreReported()
    {
        const string inputCode =
            """
            using System.Collections.Generic;
            using Funcky.Monads;

            public static class C
            {
                public static void M(Option<List<int>> option, Option<string> text, Option<int> number, int value)
                {
                    option.GetOrElse({|λ1015:new List<int>()|});
                    text.GetOrElse({|λ1015:$"value: {value}"|});
                    number.GetOrElse({|λ1015:value + Compute()|});
                    number.OrElse({|λ1015:Option.Some(Compute())|});
                    number.Match(none: {|λ1015:Compute()|}, some: x => x);
                }

                private static int Compute() => 42;
            }
            """;
        const string fixedCode =
            """
            using System.Collections.Generic;
            using Funcky.Monads;

            public static class C
            {
                public static void M(Option<List<int>> option, Option<string> text, Option<int> number, int value)
                {
                    option.GetOrElse(() => new List<int>());
                    text.GetOrElse(() => $"value: {value}");
                    number.GetOrElse(() => value + Compute());
                    number.OrElse(() => Option.Some(Compute()));
                    number.Match(none: () => Compute(), some: x => x);
                }

                private static int Compute() => 42;
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubCode, fixedCode + MonadStubCode);
    }

    [Fact]
    public async Task ResultAndEitherGetALambdaWithADiscardedParameter()
    {
        const string inputCode =
            """
            using Funcky.Monads;

            public static class C
            {
                public static int R(Result<int> result)
                    => result.GetOrElse({|λ1015:Compute()|});

                public static Either<string, int> E(Either<string, int> either)
                    => either.OrElse({|λ1015:Either<string, int>.Left(Describe())|});

                private static int Compute() => 42;

                private static string Describe() => "left";
            }
            """;
        const string fixedCode =
            """
            using Funcky.Monads;

            public static class C
            {
                public static int R(Result<int> result)
                    => result.GetOrElse(_ => Compute());

                public static Either<string, int> E(Either<string, int> either)
                    => either.OrElse(_ => Either<string, int>.Left(Describe()));

                private static int Compute() => 42;

                private static string Describe() => "left";
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubCode, fixedCode + MonadStubCode);
    }

    [Fact]
    public async Task PlainValuesAndCheapConstructorsAreNotReported()
    {
        const string inputCode =
            """
            using System;
            using System.Linq;
            using System.Collections.Generic;
            using Funcky.Monads;

            public static class C
            {
                private const int Constant = 1;

                private static readonly List<int> Shared = new();

                public static void M(Option<int> option, Option<IEnumerable<int>> sequence, Option<int[]> array, Option<List<int>> list, int parameter, string text)
                {
                    var local = 42;
                    option.GetOrElse(42);
                    option.GetOrElse(Constant);
                    option.GetOrElse(local);
                    option.GetOrElse(parameter);
                    option.GetOrElse(text.Length);
                    option.GetOrElse(parameter + local);
                    option.OrElse(Option<int>.None);
                    option.OrElse(Option.Some(parameter));
                    option.OrElse(42);
                    sequence.GetOrElse(Enumerable.Empty<int>());
                    array.GetOrElse(Array.Empty<int>());
                    list.GetOrElse(Shared);
                    option.Match(none: local, some: x => x);
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + MonadStubCode);
    }

    [Fact]
    public async Task LazyOverloadsAndThrowExpressionsAreNotReported()
    {
        const string inputCode =
            """
            #nullable enable

            using System;
            using Funcky.Monads;

            public static class C
            {
                public static void M(Option<int> option, int? nullable)
                {
                    option.GetOrElse(() => Compute());
                    option.GetOrElse(Compute);
                    option.OrElse(() => Option.Some(Compute()));
                    option.GetOrElse(nullable ?? throw new Exception());
                    option.AndThen(x => Compute());
                }

                private static int Compute() => 42;
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + MonadStubCode);
    }
}
