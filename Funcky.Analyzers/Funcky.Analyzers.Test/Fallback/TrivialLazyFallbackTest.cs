using Xunit;
using static Funcky.Analyzers.Test.Fallback.FallbackStubs;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.Fallback.TrivialLazyFallbackAnalyzer, Funcky.Analyzers.Fallback.TrivialLazyFallbackCodeFix>;

namespace Funcky.Analyzers.Test.Fallback;

public sealed class TrivialLazyFallbackTest
{
    [Fact]
    public async Task LambdaReturningALiteralIsReportedAndUnwrapped()
    {
        const string inputCode =
            """
            using Funcky.Monads;

            public static class C
            {
                public static float M(Option<float> option)
                    => option.GetOrElse({|λ1016:() => 0f|});
            }
            """;
        const string fixedCode =
            """
            using Funcky.Monads;

            public static class C
            {
                public static float M(Option<float> option)
                    => option.GetOrElse(0f);
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubCode, fixedCode + MonadStubCode);
    }

    [Fact]
    public async Task LambdasReturningVariablesMembersAndConstantsAreReported()
    {
        const string inputCode =
            """
            using Funcky.Monads;

            public sealed class C
            {
                private const int Constant = 1;

                private static readonly string Shared = "shared";

                private int _field;

                public int Property { get; }

                public void M(Option<int> option, Option<string> text, int parameter)
                {
                    var local = 42;
                    option.GetOrElse({|λ1016:() => local|});
                    option.GetOrElse({|λ1016:() => parameter|});
                    option.GetOrElse({|λ1016:() => Constant|});
                    option.GetOrElse({|λ1016:() => _field|});
                    option.GetOrElse({|λ1016:() => this.Property|});
                    text.GetOrElse({|λ1016:() => Shared|});
                    text.GetOrElse({|λ1016:() => string.Empty|});
                    option.OrElse({|λ1016:() => Option<int>.None|});
                    option.OrElse({|λ1016:() => 42|});
                    option.Match(none: {|λ1016:() => { return 42; }|}, some: x => x);
                }
            }
            """;
        const string fixedCode =
            """
            using Funcky.Monads;

            public sealed class C
            {
                private const int Constant = 1;

                private static readonly string Shared = "shared";

                private int _field;

                public int Property { get; }

                public void M(Option<int> option, Option<string> text, int parameter)
                {
                    var local = 42;
                    option.GetOrElse(local);
                    option.GetOrElse(parameter);
                    option.GetOrElse(Constant);
                    option.GetOrElse(_field);
                    option.GetOrElse(this.Property);
                    text.GetOrElse(Shared);
                    text.GetOrElse(string.Empty);
                    option.OrElse(Option<int>.None);
                    option.OrElse(42);
                    option.Match(none: 42, some: x => x);
                }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubCode, fixedCode + MonadStubCode);
    }

    [Fact]
    public async Task LambdasOnResultAndEitherIgnoringTheirParameterAreReported()
    {
        const string inputCode =
            """
            using Funcky.Monads;

            public static class C
            {
                public static int R(Result<int> result)
                    => result.GetOrElse({|λ1016:_ => 42|});

                public static int E(Either<string, int> either, int fallback)
                    => either.GetOrElse({|λ1016:left => fallback|});
            }
            """;
        const string fixedCode =
            """
            using Funcky.Monads;

            public static class C
            {
                public static int R(Result<int> result)
                    => result.GetOrElse(42);

                public static int E(Either<string, int> either, int fallback)
                    => either.GetOrElse(fallback);
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + MonadStubCode, fixedCode + MonadStubCode);
    }

    [Fact]
    public async Task LambdasWithWorkOrUsingTheirParameterAreNotReported()
    {
        const string inputCode =
            """
            #nullable enable

            using System;
            using System.Collections.Generic;
            using Funcky.Monads;

            public static class C
            {
                public static void M(Option<int> option, Option<List<int>> list, Option<string> text, Result<int> result, Either<string, int> either, int? nullable, int[] values)
                {
                    option.GetOrElse(() => Compute());
                    option.GetOrElse(Compute);
                    option.GetOrElse(() => nullable ?? throw new Exception());
                    option.GetOrElse(() => values[0]);
                    list.GetOrElse(() => new List<int>());
                    text.GetOrElse(() => $"{option}");
                    result.GetOrElse(exception => throw exception);
                    either.GetOrElse(left => left.Length);
                    option.AndThen(x => 42);
                    option.Match(none: () => Compute(), some: x => x);
                }

                private static int Compute() => 42;
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + MonadStubCode);
    }

    [Fact]
    public async Task LambdasReturningNullOrDefaultAreNotReportedBecauseTheValueWouldBeAmbiguous()
    {
        const string inputCode =
            """
            #nullable enable

            using Funcky.Monads;

            public static class C
            {
                public static void M(Option<int> option, Option<string> text)
                {
                    option.GetOrElse(() => default);
                    text.GetOrElse(() => null!);
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + MonadStubCode);
    }
}
