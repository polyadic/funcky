using System.IO;
using System.Threading.Tasks;
using Xunit;
using VerifyCS = Funcky.Analyzer.Test.CSharpCodeFixVerifier<Funcky.Analyzer.EnumerableRepeatNeverAnalyzer, Funcky.Analyzer.EnumerableRepeatNeverCodeFix>;

namespace Funcky.Analyzer.Test
{
    public class EnumerableRepeatNeverTest
    {
        [Fact]
        public async Task EnumerableRepeatWithAnyNumberButZeroIssuesNoDiagnostic()
        {
            var inputCode = File.ReadAllText("TestCode/EnumerableRepeatWithAnyNumber.input");

            await VerifyCS.VerifyAnalyzerAsync(inputCode);
        }

        [Fact]
        public async Task UsingEnumerableRepeatNeverShowsTheSequenceReturnDiagnostic()
        {
            var expectedDiagnostic = VerifyCS
                .Diagnostic(nameof(EnumerableRepeatNeverAnalyzer))
                .WithSpan(10, 26, 10, 62)
                .WithArguments("\"Hello world!\"", "string");

            await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<EnumerableRepeatNeverAnalyzer, EnumerableRepeatNeverCodeFix>(expectedDiagnostic, "RepeatNever");
        }

        [Fact]
        public async Task UsingEnumerableRepeatNeverViaConstantShowsTheSequenceReturnDiagnostic()
        {
            var expectedDiagnostic = VerifyCS
                .Diagnostic(nameof(EnumerableRepeatNeverAnalyzer))
                .WithSpan(11, 26, 11, 66)
                .WithArguments("\"Hello world!\"", "string");

            await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<EnumerableRepeatNeverAnalyzer, EnumerableRepeatNeverCodeFix>(expectedDiagnostic, "RepeatNeverWithConstant");
        }

        [Fact]
        public async Task UsingEnumerableRepeatNeverWorksWithDifferentTypes()
        {
            var expectedDiagnostic = VerifyCS
                .Diagnostic(nameof(EnumerableRepeatNeverAnalyzer))
                .WithSpan(10, 26, 10, 52)
                .WithArguments("1337", "int");

            await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<EnumerableRepeatNeverAnalyzer, EnumerableRepeatNeverCodeFix>(expectedDiagnostic, "RepeatNeverWithInt");
        }
    }
}
