using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.EnumerableRepeatOnceAnalyzer, Funcky.Analyzers.EnumerableRepeatOnceCodeFix>;

namespace Funcky.Analyzers.Test
{
    public sealed class EnumerableRepeatOnceTest
    {
        [Fact]
        public async Task EnumerableRepeatWithAnyNumberButOneIssuesNoDiagnostic()
        {
            var inputCode = File.ReadAllText("TestCode/EnumerableRepeatWithAnyNumber.input");

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
        public async Task UsingEnumerableRepeatOnceViaConstantShowsTheSequenceReturnDiagnostic()
        {
            var expectedDiagnostic = VerifyCS
                .Diagnostic(EnumerableRepeatOnceAnalyzer.DiagnosticId)
                .WithSpan(18, 26, 18, 65)
                .WithArguments("\"Hello world!\"");

            await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<EnumerableRepeatOnceAnalyzer, EnumerableRepeatOnceCodeFix>(expectedDiagnostic, "RepeatOnceWithConstant");
        }
    }
}
