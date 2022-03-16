using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.JoinToStringEmptyAnalyzer, Funcky.Analyzers.JoinToStringEmptyCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed class JoinToStringEmptyTest
{
    [Fact]
    public async Task ValidUseOfJoinToStringDoesNotIssueADiagnostic()
    {
        var inputCode = await File.ReadAllTextAsync("TestCode/ValidUseJoinToString.input");

        await VerifyCS.VerifyAnalyzerAsync(inputCode);
    }

    [Fact]
    public async Task UsingAJoinToStringWithAnEmptyStringShowsTheDiagnostic()
    {
        var expectedDiagnostic = VerifyCS
            .Diagnostic(JoinToStringEmptyAnalyzer.DiagnosticId)
            .WithSpan(28, 26, 28, 60)
            .WithArguments("strings", "string.Empty");

        await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<JoinToStringEmptyAnalyzer, JoinToStringEmptyCodeFix>(expectedDiagnostic, "JoinToStringEmpty");
    }

    [Fact]
    public async Task UsingAJoinToStringWithAnEmptyStringConstantShowsTheDiagnostic()
    {
        var expectedDiagnostic = VerifyCS
            .Diagnostic(JoinToStringEmptyAnalyzer.DiagnosticId)
            .WithSpan(30, 26, 30, 53)
            .WithArguments("strings", "Empty");

        await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<JoinToStringEmptyAnalyzer, JoinToStringEmptyCodeFix>(expectedDiagnostic, "JoinToStringEmptyConstant");
    }
}
