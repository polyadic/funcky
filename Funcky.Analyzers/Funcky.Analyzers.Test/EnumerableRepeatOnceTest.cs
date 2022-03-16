using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.EnumerableRepeatOnceAnalyzer, Funcky.Analyzers.EnumerableRepeatOnceCodeFix>;

namespace Funcky.Analyzers.Test
{
    public sealed class EnumerableRepeatOnceTest
    {
        [Fact]
        public async Task EnumerableRepeatWithAnyNumberButOneIssuesNoDiagnostic()
        {
            var inputCode = await File.ReadAllTextAsync("TestCode/EnumerableRepeatWithAnyNumber.input");

            await VerifyCS.VerifyAnalyzerAsync(inputCode);
        }

        [Fact]
        public async Task UsingEnumerableRepeatOnceShowsTheSequenceReturnDiagnostic()
        {
            var expectedDiagnostic = VerifyCS
                .Diagnostic(EnumerableRepeatOnceAnalyzer.DiagnosticId)
                .WithSpan(17, 26, 17, 62)
                .WithArguments("\"Hello world!\"");

            await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<EnumerableRepeatOnceAnalyzer, EnumerableRepeatOnceCodeFix>(expectedDiagnostic, "RepeatOnce");
        }

        [Fact]
        public async Task UsingEnumerableRepeatOnceShowsNoDiagnosticWhenSequenceTypeIsNotAvailable()
        {
            var inputCode = File.ReadAllText("TestCode/RepeatOnceMissingSequenceType.input");
            await VerifyCS.VerifyAnalyzerAsync(inputCode);
        }

        [Fact]
        public async Task UsingEnumerableRepeatOnceViaConstantShowsTheSequenceReturnDiagnostic()
        {
            var expectedDiagnostic = VerifyCS
                .Diagnostic(EnumerableRepeatOnceAnalyzer.DiagnosticId)
                .WithSpan(18, 26, 18, 65)
                .WithArguments("\"Hello world!\"");

            await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<EnumerableRepeatOnceAnalyzer, EnumerableRepeatOnceCodeFix>(expectedDiagnostic, "RepeatOnceWithConstant");
        }

        [Fact]
        public async Task CodeFixWorksWithDifferentUsingStyles()
        {
            var expectedDiagnostics = new[]
            {
                VerifyCS.Diagnostic(EnumerableRepeatOnceAnalyzer.DiagnosticId).WithSpan(17, 17, 17, 53).WithArguments("10"),
                VerifyCS.Diagnostic(EnumerableRepeatOnceAnalyzer.DiagnosticId).WithSpan(28, 17, 28, 53).WithArguments("10"),
                VerifyCS.Diagnostic(EnumerableRepeatOnceAnalyzer.DiagnosticId).WithSpan(41, 17, 41, 53).WithArguments("10"),
            };

            await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<EnumerableRepeatOnceAnalyzer, EnumerableRepeatOnceCodeFix>(expectedDiagnostics, "RepeatOnceQualification");
        }
    }
}
