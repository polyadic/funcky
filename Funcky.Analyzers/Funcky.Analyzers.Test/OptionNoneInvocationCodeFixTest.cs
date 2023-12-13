#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
#pragma warning disable SA1118 // StyleCop support for collection expressions is missing
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.Test.OptionNoneInvocationCodeFixTest.NullDiagnosticAnalyzer, Funcky.BuiltinAnalyzers.OptionNoneInvocationCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed class OptionNoneInvocationCodeFixTest
{
    private const string OptionCode = """
        namespace Funcky.Monads
        {
            public readonly struct Option<TItem>
            {
                public static Option<TItem> None { get; } = default;
            }
        }
        """;

    [Fact]
    public async Task ReplacesInvocationWithPropertyAccess()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static void M()
                {
                    var option = Option<int>.None();
                }
            }
            """;

        const string fixedCode = """
            using Funcky.Monads;

            public static class C
            {
                public static void M()
                {
                    var option = Option<int>.None;
                }
            }
            """;

        await VerifyCS.VerifyCodeFixAsync(
            inputCode + Environment.NewLine + OptionCode,
            DiagnosticResult.CompilerError("CS1955").WithSpan(7, 34, 7, 38).WithArguments("Funcky.Monads.Option<int>.None"),
            fixedCode + Environment.NewLine + OptionCode);
    }

    [Fact]
    public async Task PreservesTrivia()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static void M()
                {
                    var option = /* trivia 1*/Option<int>.None/* trivia 2*/()/* trivia 3*/;
                    var option2 = GetBool() ? Option<int>.None() : throw null!;
                }

                public static bool GetBool() => true;
            }
            """;

        const string fixedCode = """
            using Funcky.Monads;

            public static class C
            {
                public static void M()
                {
                    var option = /* trivia 1*/Option<int>.None/* trivia 2*//* trivia 3*/;
                    var option2 = GetBool() ? Option<int>.None : throw null!;
                }

                public static bool GetBool() => true;
            }
            """;

        await VerifyCS.VerifyCodeFixAsync(
            inputCode + Environment.NewLine + OptionCode,
            [
                DiagnosticResult.CompilerError("CS1955").WithSpan(7, 47, 7, 51).WithArguments("Funcky.Monads.Option<int>.None"),
                DiagnosticResult.CompilerError("CS1955").WithSpan(8, 47, 8, 51).WithArguments("Funcky.Monads.Option<int>.None"),
            ],
            fixedCode + Environment.NewLine + OptionCode);
    }

    [Fact]
    public async Task DoesNotFixInvocationOfOtherProperties()
    {
        const string inputCode = """
            public static class C
            {
                private static int P => 0;

                public static void M()
                {
                    var x = P();
                }
            }
            """;

        await new VerifyCS.Test
        {
            TestState =
            {
                Sources = { inputCode + Environment.NewLine + OptionCode },
                ExpectedDiagnostics = { DiagnosticResult.CompilerError("CS1955").WithSpan(7, 17, 7, 18).WithArguments("C.P"), },
            },
            FixedState =
            {
                Sources = { inputCode + Environment.NewLine + OptionCode },
                ExpectedDiagnostics = { DiagnosticResult.CompilerError("CS1955").WithSpan(7, 17, 7, 18).WithArguments("C.P"), },
            },
        }.RunAsync();
    }

    [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1001:Missing diagnostic analyzer attribute")]
    internal sealed class NullDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [];

        [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1025:Configure generated code analysis")]
        [SuppressMessage("MicrosoftCodeAnalysisCorrectness", "RS1026:Enable concurrent execution")]
        public override void Initialize(AnalysisContext context)
        {
        }
    }
}
