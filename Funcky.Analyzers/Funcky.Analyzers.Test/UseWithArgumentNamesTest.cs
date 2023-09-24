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
        var inputCode =
            """
            using Funcky.CodeAnalysis;

            class Test
            {
                private void Syntax()
                {
                    Method(x: 10, y: 20);
                }

                [UseWithArgumentNames]
                private void Method(int x, int y) { }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + AttributeSource);
    }

    [Fact]
    public async Task ArgumentsForCallsToMethodsWithoutAttributeGetNoDiagnostic()
    {
        var inputCode =
            """
            using Funcky.CodeAnalysis;

            class Test
            {
                private void Syntax()
                {
                    Method(10, 20);
                }

                private void Method(int x, int y) { }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + AttributeSource);
    }

    [Fact]
    public async Task UsagesOfMethodsAnnotatedWithShouldUseNamedArgumentsAttributeGetWarningAndAreFixed()
    {
        const string inputCode =
            """
            using Funcky.CodeAnalysis;

            class Test
            {
                private void Syntax()
                {
                    Method(x: 10, 20);
                    Method(10, 20);
                    Method(
                        10, 20);
                    Method(
                        10,
                        20);
                    MethodWithKeywordAsArgument(10);
                }

                [UseWithArgumentNames]
                private void Method(int x, int y) { }

                [UseWithArgumentNames]
                private void MethodWithKeywordAsArgument(int @int) { }
            }
            """;

        const string fixedCode =
            """
            using Funcky.CodeAnalysis;

            class Test
            {
                private void Syntax()
                {
                    Method(x: 10, y: 20);
                    Method(x: 10, y: 20);
                    Method(
                        x: 10, y: 20);
                    Method(
                        x: 10,
                        y: 20);
                    MethodWithKeywordAsArgument(@int: 10);
                }

                [UseWithArgumentNames]
                private void Method(int x, int y) { }

                [UseWithArgumentNames]
                private void MethodWithKeywordAsArgument(int @int) { }
            }
            """;

        var expectedDiagnostics = new[]
        {
            VerifyCS.Diagnostic().WithSpan(7, 23, 7, 25).WithArguments("y"),
            VerifyCS.Diagnostic().WithSpan(8, 16, 8, 18).WithArguments("x"),
            VerifyCS.Diagnostic().WithSpan(8, 20, 8, 22).WithArguments("y"),
            VerifyCS.Diagnostic().WithSpan(10, 13, 10, 15).WithArguments("x"),
            VerifyCS.Diagnostic().WithSpan(10, 17, 10, 19).WithArguments("y"),
            VerifyCS.Diagnostic().WithSpan(12, 13, 12, 15).WithArguments("x"),
            VerifyCS.Diagnostic().WithSpan(13, 13, 13, 15).WithArguments("y"),
            VerifyCS.Diagnostic().WithSpan(14, 37, 14, 39).WithArguments("int"),
        };

        await VerifyCS.VerifyAnalyzerAsync(inputCode + AttributeSource, expectedDiagnostics);
        await VerifyCS.VerifyCodeFixAsync(inputCode + AttributeSource, expectedDiagnostics, fixedCode + AttributeSource);
    }

    [Fact]
    public async Task IgnoresCallsToMethodsInsideExpressionTrees()
    {
        var inputCode =
            """
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
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + AttributeSource);
    }
}
