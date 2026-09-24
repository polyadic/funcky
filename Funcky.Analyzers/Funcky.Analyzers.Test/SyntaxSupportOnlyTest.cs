using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpAnalyzerVerifier<Funcky.BuiltinAnalyzers.SyntaxSupportOnlyAnalyzer>;

namespace Funcky.Analyzers.Test;

public sealed class SyntaxSupportOnlyTest
{
    // language=csharp
    private const string AttributeSource =
        """
        namespace Funcky.CodeAnalysis
        {
            #pragma warning disable CS9113 // Parameter is unread.
            [System.AttributeUsage(System.AttributeTargets.Property)]
            internal sealed class SyntaxSupportOnlyAttribute(string syntaxFeature) : System.Attribute;
            #pragma warning restore CS9113 // Parameter is unread.
        }
        """;

    [Fact]
    public async Task DisallowsUseOfPropertiesAnnotatedWithAttribute()
    {
        // language=csharp
        const string inputCode =
            """
            public static class C
            {
                public static void M()
                {
                    var option = new Option();
                    _ = option.Count;
                }
            }

            public class Option
            {
                [Funcky.CodeAnalysis.SyntaxSupportOnly("list pattern")]
                public int Count => 0;
            }
            """;

        DiagnosticResult[] expectedDiagnostics = [
            VerifyCS.Diagnostic().WithSpan(6, 13, 6, 25).WithArguments("property", "list pattern"),
        ];
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + AttributeSource, expectedDiagnostics);
    }

    [Fact]
    public async Task DisallowsUseOfIndexersAnnotatedWithAttribute()
    {
        // language=csharp
        const string inputCode =
            """
            public static class C
            {
                public static void M()
                {
                    var option = new Option();
                    _ = option[0];
                }
            }

            public class Option
            {
                [Funcky.CodeAnalysis.SyntaxSupportOnly("foo")]
                public int this[int index] => 0;
            }
            """;

        DiagnosticResult[] expectedDiagnostics = [
            VerifyCS.Diagnostic().WithSpan(6, 13, 6, 22).WithArguments("property", "foo"),
        ];
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + AttributeSource, expectedDiagnostics);
    }
}
