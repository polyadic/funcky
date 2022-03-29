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
            var inputCode = await File.ReadAllTextAsync($"TestCode/{testCode}.input");

            await CSharpCodeFixVerifier<TAnalyzer, TCodeFix>.VerifyAnalyzerAsync(inputCode, expectedDiagnostic);

            var expectedCode = await File.ReadAllTextAsync($"TestCode/{testCode}.expected");

            await CSharpCodeFixVerifier<TAnalyzer, TCodeFix>.VerifyCodeFixAsync(inputCode, expectedDiagnostic, expectedCode);
        }

        public static async Task VerifyDiagnosticAndCodeFix<TAnalyzer, TCodeFix>(DiagnosticResult[] expectedDiagnostics, string testCode)
            where TAnalyzer : DiagnosticAnalyzer, new()
            where TCodeFix : CodeFixProvider, new()
        {
            var inputCode = await File.ReadAllTextAsync($"TestCode/{testCode}.input");

            await CSharpCodeFixVerifier<TAnalyzer, TCodeFix>.VerifyAnalyzerAsync(inputCode, expectedDiagnostics);

            var expectedCode = await File.ReadAllTextAsync($"TestCode/{testCode}.expected");

            await CSharpCodeFixVerifier<TAnalyzer, TCodeFix>.VerifyCodeFixAsync(inputCode, expectedDiagnostics, expectedCode);
        }
    }
}
