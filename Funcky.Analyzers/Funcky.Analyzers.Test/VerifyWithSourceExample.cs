using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Funcky.Analyzers.Test
{
    internal sealed class VerifyWithSourceExample
    {
        public static async Task VerifyDiagnosticAndCodeFix<TAnalyzer, TCodeFix>(DiagnosticResult expectedDiagnostic, string testCode)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var inputCode = File.ReadAllText($"TestCode/{testCode}.input");

            await CSharpCodeFixVerifier<TAnalyzer, TCodeFix>.VerifyAnalyzerAsync(inputCode, expectedDiagnostic);

            var expectedCode = File.ReadAllText($"TestCode/{testCode}.expected");

            await CSharpCodeFixVerifier<TAnalyzer, TCodeFix>.VerifyCodeFixAsync(inputCode, expectedDiagnostic, expectedCode);
        }
    }
}
