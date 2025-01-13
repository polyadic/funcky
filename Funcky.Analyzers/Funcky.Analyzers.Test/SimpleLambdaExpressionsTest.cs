using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpAnalyzerVerifier<Funcky.Analyzers.SimpleLambdaExpressionsAnalyzer>;

namespace Funcky.Analyzers.Test;

// TODO: Code Fix: Add cast for null literal
public sealed class SimpleLambdaExpressionsTest
{
    private const string FuncWithAttributeCode = @"
namespace Funcky.CodeAnalysis
{
    internal sealed class InlineSimpleLambdaExpressionsAttribute : System.Attribute
    {
    }
}

public static partial class C
{
    public static void TakesFunc<T>([Funcky.CodeAnalysis.InlineSimpleLambdaExpressions] System.Func<T> func) { }
}";

    [Fact]
    public async Task Literal()
    {
        const string inputCode = @"
public static partial class C
{
    public static void M()
    {
        TakesFunc(() => 10);
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + FuncWithAttributeCode, VerifyCS.Diagnostic().WithSpan(6, 19, 6, 27));
    }

    [Fact]
    public async Task Variable()
    {
        const string inputCode = @"
public static partial class C
{
    public static void M()
    {
        var variable = 10;
        TakesFunc(() => variable);
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + FuncWithAttributeCode, VerifyCS.Diagnostic().WithSpan(7, 19, 7, 33));
    }

    [Fact]
    public async Task ConstantValue()
    {
        const string inputCode = @"
public static partial class C
{
    public const int MemberConstant = 10;

    public static void M()
    {
        const int localConstant = 10;
        TakesFunc(() => localConstant);
        TakesFunc(() => MemberConstant);
        TakesFunc(() => localConstant + 1);
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(
            inputCode + Environment.NewLine + FuncWithAttributeCode,
            VerifyCS.Diagnostic().WithSpan(9, 19, 9, 38),
            VerifyCS.Diagnostic().WithSpan(10, 19, 10, 39),
            VerifyCS.Diagnostic().WithSpan(11, 19, 11, 42));
    }

    [Fact]
    public async Task Parameter()
    {
        const string inputCode = @"
public static partial class C
{
    public static void M(int parameter)
    {
        TakesFunc(() => parameter);
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + FuncWithAttributeCode, VerifyCS.Diagnostic().WithSpan(6, 19, 6, 34));
    }

    [Fact]
    public async Task Cast()
    {
        const string inputCode = @"
public static partial class C
{
    public static void M(int parameter)
    {
        TakesFunc(() => (object)parameter);
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + FuncWithAttributeCode, VerifyCS.Diagnostic().WithSpan(6, 19, 6, 42));
    }

    [Fact]
    public async Task ObjectCreation()
    {
        const string inputCode = @"
public static partial class C
{
    public static void M(int parameter)
    {
        TakesFunc(() => new object());
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + FuncWithAttributeCode, VerifyCS.Diagnostic().WithSpan(6, 19, 6, 37));
    }

    [Fact]
    public async Task ObjectCreationCounterExample()
    {
        const string inputCode = @"
public static partial class C
{
    public static void M(int parameter)
    {
        TakesFunc(() => new Foo(GetBar()));
        TakesFunc(() => new Foo(0) { Bar = GetBar() });
    }

    public static int GetBar() { return 0; }

    public sealed class Foo
    {
        public Foo() { }

        public Foo(int bar) { }

        public int Bar { get; set; }
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + FuncWithAttributeCode);
    }

    [Fact]
    public async Task StaticProperty()
    {
        const string inputCode = @"
public static partial class C
{
    public static void M(int parameter)
    {
        TakesFunc(() => Foo.Value);
    }

    public static class Foo
    {
        public static int Value { get; }
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + FuncWithAttributeCode, VerifyCS.Diagnostic().WithSpan(6, 19, 6, 34));
    }

    [Fact]
    public async Task StaticField()
    {
        const string inputCode = @"
public static partial class C
{
    public static void M(int parameter)
    {
        TakesFunc(() => Foo.Value);
    }

    public static class Foo
    {
        public static readonly int Value;
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + FuncWithAttributeCode, VerifyCS.Diagnostic().WithSpan(6, 19, 6, 34));
    }
}
