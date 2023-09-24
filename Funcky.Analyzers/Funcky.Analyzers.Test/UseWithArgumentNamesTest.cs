using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.UseWithArgumentNamesAnalyzer, Funcky.Analyzers.AddArgumentNameCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed class UseWithArgumentNamesTest
{
    private const string AttributeSource =
        """
        namespace Funcky.CodeAnalysis
        {
            [System.AttributeUsage(System.AttributeTargets.Method)]
            internal sealed class UseWithArgumentNamesAttribute : System.Attribute { }
        }
        """;

    [Fact]
    public async Task ArgumentsThatAlreadyUseArgumentNamesGetNoDiagnostic()
    {
        var inputCode = await File.ReadAllTextAsync("TestCode/ValidUseWithArgumentNames.input");
        await VerifyCS.VerifyAnalyzerAsync(inputCode);
    }

    [Fact]
    public async Task ArgumentsForCallsToMethodsWithoutAttributeGetNoDiagnostic()
    {
        var inputCode = await File.ReadAllTextAsync("TestCode/ValidUseWithArgumentNamesNoAttribute.input");
        await VerifyCS.VerifyAnalyzerAsync(inputCode);
    }

    [Fact]
    public async Task UsagesOfMethodsAnnotatedWithShouldUseNamedArgumentsAttributeGetWarningAndAreFixed()
    {
        var expectedDiagnostics = new[]
        {
            VerifyCS.Diagnostic().WithSpan(11, 27, 11, 29).WithArguments("y"),
            VerifyCS.Diagnostic().WithSpan(12, 20, 12, 22).WithArguments("x"),
            VerifyCS.Diagnostic().WithSpan(12, 24, 12, 26).WithArguments("y"),
            VerifyCS.Diagnostic().WithSpan(14, 17, 14, 19).WithArguments("x"),
            VerifyCS.Diagnostic().WithSpan(14, 21, 14, 23).WithArguments("y"),
            VerifyCS.Diagnostic().WithSpan(16, 17, 16, 19).WithArguments("x"),
            VerifyCS.Diagnostic().WithSpan(17, 17, 17, 19).WithArguments("y"),
            VerifyCS.Diagnostic().WithSpan(18, 41, 18, 43).WithArguments("int"),
        };

        await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<UseWithArgumentNamesAnalyzer, AddArgumentNameCodeFix>(expectedDiagnostics, "UseWithArgumentNames");
    }

    [Fact]
    public async Task IgnoresCallsToMethodsInsideExpressionTrees()
    {
        var inputCode =
            $$"""
            using System;
            using System.Linq.Expressions;
            using Funcky.CodeAnalysis;

            class Test
            {
                private void Syntax()
                {
                    Expression<Action> expr = () => Method(10, 20);
                }

                [UseWithArgumentNames]
                private void Method(int x, int y) { }
            }

            {{AttributeSource}}
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode);
    }
}
