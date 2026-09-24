using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpAnalyzerVerifier<Funcky.Analyzers.OptionListPatternAnalyzer>;

namespace Funcky.Analyzers.Test;

public sealed class OptionListPatternTest
{
    // language=csharp
    private const string OptionStub =
        """
        namespace Funcky.Monads
        {
            public readonly struct Option<T>
            {
                public int Count => throw null!;

                public T this[int index] => throw null!;
            }
        }
        """;

    [Fact]
    public async Task ErrorsWhenListPatternHasMoreThanOneElement()
    {
        // language=csharp
        const string inputCode =
            """
            using Funcky.Monads;

            class C
            {
                private void M(Option<string> option)
                {
                    _ = option is ["foo", "bar"];
                    _ = option is [var foo, var bar, var baz];
                    _ = option is [var one, var two, var three, var four];
                }
            }
            """;

        DiagnosticResult[] expectedDiagnostics = [
            VerifyCS.Diagnostic().WithSpan(7, 23, 7, 37),
            VerifyCS.Diagnostic().WithSpan(8, 23, 8, 50),
            VerifyCS.Diagnostic().WithSpan(9, 23, 9, 62),
        ];

        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + OptionStub, expectedDiagnostics);
    }

    [Fact]
    public async Task DoesNotErrorWhenUsingAListPatternWithZeroOrOneElements()
    {
        // language=csharp
        const string inputCode =
            """
            using Funcky.Monads;

            class C
            {
                private void M(Option<string> option)
                {
                    _ = option is ["foo"];
                    _ = option is [];
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + OptionStub);
    }

    [Fact]
    public async Task UsingASlicePatternIsACompileError()
    {
        // language=csharp
        const string inputCode =
            """
            using Funcky.Monads;

            class C
            {
                private void M(Option<string> option)
                {
                    _ = option is [..var slice];
                }
            }
            """;

        DiagnosticResult[] expectedDiagnostics = [
            DiagnosticResult.CompilerError("CS1503").WithSpan(7, 24, 7, 35).WithArguments("1", "System.Range", "int"),
        ];

        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + OptionStub, expectedDiagnostics);
    }
}
