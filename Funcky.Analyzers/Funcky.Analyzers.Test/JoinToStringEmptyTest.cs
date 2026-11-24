using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.JoinToStringEmptyAnalyzer, Funcky.Analyzers.JoinToStringEmptyCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed class JoinToStringEmptyTest
{
    private const string ChainedReceiver = "modes.GroupBy(Query).Select(FormatByQuery)";

    [Fact]
    public async Task ValidUseOfJoinToStringDoesNotIssueADiagnostic()
    {
        var inputCode = await File.ReadAllTextAsync("TestCode/ValidUseJoinToString.input");

        await VerifyCS.VerifyAnalyzerAsync(inputCode);
    }

    [Fact]
    public async Task AJoinToStringOfAnOrdinaryClassNestedInEnumerableExtensionsDoesNotIssueADiagnostic()
    {
        var inputCode = await File.ReadAllTextAsync("TestCode/JoinToStringNestedClass.input");

        await VerifyCS.VerifyAnalyzerAsync(inputCode);
    }

    [Fact]
    public async Task UsingAJoinToStringWithAnEmptyStringShowsTheDiagnostic()
    {
        var expectedDiagnostic = VerifyCS
            .Diagnostic(JoinToStringEmptyAnalyzer.DiagnosticId)
            .WithSpan(29, 26, 29, 60)
            .WithArguments("strings", "string.Empty");

        await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<JoinToStringEmptyAnalyzer, JoinToStringEmptyCodeFix>(expectedDiagnostic, "JoinToStringEmpty");
    }

    [Fact]
    public async Task UsingAJoinToStringWithAnEmptyStringConstantShowsTheDiagnostic()
    {
        var expectedDiagnostic = VerifyCS
            .Diagnostic(JoinToStringEmptyAnalyzer.DiagnosticId)
            .WithSpan(31, 26, 31, 53)
            .WithArguments("strings", "Empty");

        await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<JoinToStringEmptyAnalyzer, JoinToStringEmptyCodeFix>(expectedDiagnostic, "JoinToStringEmptyConstant");
    }

    [Fact]
    public async Task TheCodeFixReplacesTheJoinToStringAtTheEndOfAMethodChain()
    {
        var expectedDiagnostic = VerifyCS
            .Diagnostic(JoinToStringEmptyAnalyzer.DiagnosticId)
            .WithSpan(34, 16, 37, 44)
            .WithArguments(ChainedReceiver, "string.Empty");

        await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<JoinToStringEmptyAnalyzer, JoinToStringEmptyCodeFix>(expectedDiagnostic, "JoinToStringEmptyChained");
    }

    [Fact]
    public async Task UsingAJoinToStringFromAnExtensionBlockWithAnEmptyStringShowsTheDiagnostic()
    {
        var expectedDiagnostic = VerifyCS
            .Diagnostic(JoinToStringEmptyAnalyzer.DiagnosticId)
            .WithSpan(37, 16, 40, 44)
            .WithArguments(ChainedReceiver, "string.Empty");

        await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<JoinToStringEmptyAnalyzer, JoinToStringEmptyCodeFix>(expectedDiagnostic, "JoinToStringEmptyExtensionBlock");
    }

    [Fact]
    public async Task UsingAJoinToStringThroughAConditionalAccessShowsTheDiagnostic()
    {
        var expectedDiagnostic = VerifyCS
            .Diagnostic(JoinToStringEmptyAnalyzer.DiagnosticId)
            .WithSpan(29, 34, 29, 61)
            .WithArguments("strings", "string.Empty");

        await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<JoinToStringEmptyAnalyzer, JoinToStringEmptyCodeFix>(expectedDiagnostic, "JoinToStringEmptyConditionalAccess");
    }

    [Fact]
    public async Task UsingAJoinToStringFromAnExtensionBlockThroughAConditionalAccessShowsTheDiagnostic()
    {
        var expectedDiagnostic = VerifyCS
            .Diagnostic(JoinToStringEmptyAnalyzer.DiagnosticId)
            .WithSpan(38, 17, 40, 44)
            .WithArguments(".GroupBy(Query).Select(FormatByQuery)", "string.Empty");

        await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<JoinToStringEmptyAnalyzer, JoinToStringEmptyCodeFix>(expectedDiagnostic, "JoinToStringEmptyExtensionBlockConditionalAccess");
    }

    [Fact]
    public async Task AStaticJoinToStringCallShowsTheDiagnosticButOffersNoCodeFix()
    {
        var inputCode = await File.ReadAllTextAsync("TestCode/JoinToStringEmptyStaticCall.input");
        var expectedDiagnostic = VerifyCS
            .Diagnostic(JoinToStringEmptyAnalyzer.DiagnosticId)
            .WithSpan(29, 26, 29, 82)
            .WithArguments("strings", "string.Empty");

        // The fixed code equals the input, so the verifier fails if a code fix is offered.
        await VerifyCS.VerifyCodeFixAsync(inputCode, expectedDiagnostic, inputCode);
    }
}
