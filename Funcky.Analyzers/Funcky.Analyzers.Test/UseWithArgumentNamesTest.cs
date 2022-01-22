using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.UseWithArgumentNamesAnalyzer, Funcky.Analyzers.AddArgumentNameCodeFix>;

namespace Funcky.Analyzers.Test
{
    public sealed class UseWithArgumentNamesTest
    {
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
    }
}
