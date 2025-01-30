using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpAnalyzerVerifier<Funcky.Analyzers.NonDefaultableAnalyzer>;

namespace Funcky.Analyzers.Test;

public sealed class NonDefaultableTest
{
    private const string AttributeSource =
        """
        namespace Funcky.CodeAnalysis
        {
            [System.AttributeUsage(System.AttributeTargets.Struct)]
            internal sealed class NonDefaultableAttribute : System.Attribute { }
        }
        """;

    [Fact]
    public async Task DefaultInstantiationsOfRegularStructsGetNoDiagnostic()
    {
        const string inputCode =
            """
            class Test
            {
                private void Usage()
                {
                    _ = default(Foo);
                    _ = new Foo();
                }
            }

            struct Foo { }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + AttributeSource);
    }

    [Fact]
    public async Task DefaultInstantiationsOfAnnotatedStructsGetError()
    {
        const string inputCode =
            """
            using Funcky.CodeAnalysis;

            class Test
            {
                private void Usage()
                {
                    _ = default(Foo);
                    _ = default(Funcky.Generic<int>);
                }
            }

            [NonDefaultable]
            struct Foo { }

            namespace Funcky
            {
                [NonDefaultable]
                struct Generic<T> { }
            }
            """;

        DiagnosticResult[] expectedDiagnostics =
        [
            VerifyCS.Diagnostic().WithSpan(7, 13, 7, 25).WithArguments("Foo"),
            VerifyCS.Diagnostic().WithSpan(8, 13, 8, 41).WithArguments("Generic<int>"),
        ];

        await VerifyCS.VerifyAnalyzerAsync(inputCode + AttributeSource, expectedDiagnostics);
    }

    [Fact]
    public async Task ParameterlessConstructorInstantiationsOfAnnotatedStructsGetError()
    {
        const string inputCode =
            """
            using Funcky.CodeAnalysis;

            class Test
            {
                private void Usage()
                {
                    _ = new Foo();
                    _ = new Funcky.Generic<int>();
                }
            }

            [NonDefaultable]
            struct Foo { }

            namespace Funcky
            {
                [NonDefaultable]
                struct Generic<T> { }
            }
            """;

        DiagnosticResult[] expectedDiagnostics =
        [
            VerifyCS.Diagnostic().WithSpan(7, 13, 7, 22).WithArguments("Foo"),
            VerifyCS.Diagnostic().WithSpan(8, 13, 8, 38).WithArguments("Generic<int>"),
        ];

        await VerifyCS.VerifyAnalyzerAsync(inputCode + AttributeSource, expectedDiagnostics);
    }
}
