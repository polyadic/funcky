using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.EnumerableRepeatOnceAnalyzer, Funcky.Analyzers.EnumerableRepeatOnceCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed class EnumerableRepeatOnceTest
{
    [Fact]
    public async Task EnumerableRepeatWithAnyNumberButOneIssuesNoDiagnostic()
    {
        var inputCode = await File.ReadAllTextAsync("TestCode/EnumerableRepeatWithAnyNumber.input");

        await VerifyCS.VerifyAnalyzerAsync(inputCode);
    }

    [Fact]
    public async Task UsingEnumerableRepeatOnceShowsTheEnumerableReturnDiagnostic()
    {
        var expectedDiagnostic = VerifyCS
            .Diagnostic(EnumerableRepeatOnceAnalyzer.DiagnosticId)
            .WithSpan(17, 26, 17, 62)
            .WithArguments("\"Hello world!\"");

        await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<EnumerableRepeatOnceAnalyzer, EnumerableRepeatOnceCodeFix>(expectedDiagnostic, "RepeatOnce");
    }

    [Fact]
    public async Task UsingEnumerableRepeatOnceShowsTheEnumerableReturnDiagnosticWhenArgumentsAreFlipped()
    {
        var expectedDiagnostic = VerifyCS
            .Diagnostic(EnumerableRepeatOnceAnalyzer.DiagnosticId)
            .WithSpan(19, 26, 19, 78)
            .WithArguments("\"Hello world!\"");

        await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<EnumerableRepeatOnceAnalyzer, EnumerableRepeatOnceCodeFix>(expectedDiagnostic, "RepeatOnceFlipped");
    }

    [Fact]
    public async Task UsingEnumerableRepeatOnceShowsNoDiagnosticWhenEnumerableExtensionsAreNotAvailable()
    {
        var inputCode = File.ReadAllText("TestCode/RepeatOnceMissingEnumerableExtensions.input");
        await VerifyCS.VerifyAnalyzerAsync(inputCode);
    }

    [Fact]
    public async Task UsingEnumerableRepeatOnceViaConstantShowsTheEnumerableReturnDiagnostic()
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
        DiagnosticResult[] expectedDiagnostics =
        [
            VerifyCS.Diagnostic(EnumerableRepeatOnceAnalyzer.DiagnosticId).WithSpan(21, 17, 21, 41).WithArguments("10"),
            VerifyCS.Diagnostic(EnumerableRepeatOnceAnalyzer.DiagnosticId).WithSpan(34, 17, 34, 53).WithArguments("10"),
            VerifyCS.Diagnostic(EnumerableRepeatOnceAnalyzer.DiagnosticId).WithSpan(48, 17, 48, 34).WithArguments("10"),
        ];

        await VerifyWithSourceExample.VerifyDiagnosticAndCodeFix<EnumerableRepeatOnceAnalyzer, EnumerableRepeatOnceCodeFix>(expectedDiagnostics, "RepeatOnceQualification");
    }
}
