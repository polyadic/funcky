using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.BuiltinAnalyzers.OptionNoneMethodGroupAnalyzer, Funcky.BuiltinAnalyzers.OptionNoneCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed class OptionNoneMethodGroupTest
{
    private const string OptionCode = @"namespace Funcky.Monads
{
    public readonly struct Option<TItem>
    {
        public static Option<TItem> None() => default;
    }
}";

    [Fact]
    public async Task ReplacesOptionOfTNoneWithLambda()
    {
        const string inputCode = @"
using Funcky.Monads;

public static class C
{
    public static void M()
    {
        System.Func<Option<int>> func = Option<int>.None;
    }
}";

        const string fixedCode = @"
using Funcky.Monads;

public static class C
{
    public static void M()
    {
        System.Func<Option<int>> func = () => Option<int>.None();
    }
}";

        await VerifyCS.VerifyCodeFixAsync(
            inputCode + Environment.NewLine + OptionCode,
            VerifyCS.Diagnostic().WithSpan(8, 41, 8, 57).WithArguments("int"),
            fixedCode + Environment.NewLine + OptionCode);
    }

    [Fact]
    public async Task ReportsNoDiagnosticForInvocation()
    {
        const string inputCode = @"
using Funcky.Monads;

public static class C
{
    public static void M()
    {
        Option<int> x = Option<int>.None();
    }
}";

        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + OptionCode);
    }
}
