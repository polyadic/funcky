using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;

namespace Funcky.Analyzers.Test
{
    public static class CSharpVerifier<TAnalyzer, TCodeFix, TVerifier>
        where TAnalyzer : DiagnosticAnalyzer, new()
        where TCodeFix : CodeFixProvider, new()
        where TVerifier : IVerifier, new()
    {
        public static DiagnosticResult Diagnostic()
            => AnalyzerVerifier<TAnalyzer, CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier>, TVerifier>.Diagnostic();

        public static DiagnosticResult Diagnostic(string diagnosticId)
            => AnalyzerVerifier<TAnalyzer, CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier>, TVerifier>.Diagnostic(diagnosticId);

        public static DiagnosticResult Diagnostic(DiagnosticDescriptor descriptor)
            => AnalyzerVerifier<TAnalyzer, CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier>, TVerifier>.Diagnostic(descriptor);

        public static Task VerifyAnalyzerAsync(string source, params DiagnosticResult[] expected)
            => AnalyzerVerifier<TAnalyzer, CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier>, TVerifier>.VerifyAnalyzerAsync(source, expected);

        public static Task VerifyCodeFixAsync(string source, string fixedSource)
            => VerifyCodeFixAsync(source, DiagnosticResult.EmptyDiagnosticResults, fixedSource);

        public static Task VerifyCodeFixAsync(string source, DiagnosticResult expected, string fixedSource)
            => VerifyCodeFixAsync(source, new[] { expected }, fixedSource);

        public static Task VerifyCodeFixAsync(string source, DiagnosticResult[] expected, string fixedSource)
        {
            var test = new CSharpCodeFixTest<TAnalyzer, TCodeFix, TVerifier>
            {
                TestCode = source,
                FixedCode = fixedSource,
                ReferenceAssemblies = ReferenceAssemblies.Default
                    .WithPackages(ImmutableArray.Create(new PackageIdentity("Funcky", "2.2.0"))),
            };

            test.ExpectedDiagnostics.AddRange(expected);
            return test.RunAsync(CancellationToken.None);
        }
    }
}
