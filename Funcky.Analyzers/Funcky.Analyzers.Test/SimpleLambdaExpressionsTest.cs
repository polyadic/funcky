using Microsoft.CodeAnalysis;
using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpAnalyzerVerifier<Funcky.Analyzers.SimpleLambdaExpressionsAnalyzer>;

namespace Funcky.Analyzers.Test;

// TODO: Code Fix: Add cast for null literal
public sealed class SimpleLambdaExpressionsTest
{
    private const string FuncWithAttributeCode =
        """
        public static class F<T>
        {
            public static void TakesFunc(T value) { }
            public static void TakesFunc(System.Func<T> func) { }
            public static void TakesFunc(System.Func<T, T> func) { }
        }
        """;

    [Fact]
    public async Task Literal()
    {
        const string inputCode =
            """
            public static class C
            {
                public static void M()
                {
                    F<int>.TakesFunc(() => 10);
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + FuncWithAttributeCode, VerifyCS.Diagnostic().WithSpan(5, 19, 5, 27));
    }

    [Fact]
    public async Task Variable()
    {
        const string inputCode =
            """
            public static class C
            {
                public static void M()
                {
                    var variable = 10;
                    F<int>.TakesFunc(() => variable);
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + FuncWithAttributeCode, VerifyCS.Diagnostic().WithSpan(6, 19, 6, 33));
    }

    [Fact]
    public async Task ConstantValue()
    {
        const string inputCode =
            """
            public static class C
            {
                public const int MemberConstant = 10;

                public static void M()
                {
                    const int localConstant = 10;
                    F<int>.TakesFunc(() => localConstant);
                    F<int>.TakesFunc(() => MemberConstant);
                    F<int>.TakesFunc(() => localConstant + 1);
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(
            inputCode + Environment.NewLine + FuncWithAttributeCode,
            VerifyCS.Diagnostic().WithSpan(8, 19, 8, 38),
            VerifyCS.Diagnostic().WithSpan(9, 19, 9, 39),
            VerifyCS.Diagnostic().WithSpan(10, 19, 10, 42));
    }

    [Fact]
    public async Task Parameter()
    {
        const string inputCode =
            """
            public static class C
            {
                public static void M(int parameter)
                {
                    F<int>.TakesFunc(() => parameter);
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + FuncWithAttributeCode, VerifyCS.Diagnostic().WithSpan(5, 19, 5, 34));
    }

    [Fact]
    public async Task FuncWithParameter()
    {
        const string inputCode =
            """
            public static class C
            {
                public static void M(int parameter)
                {
                    F<int>.TakesFunc(_ => parameter);
                    F<int>.TakesFunc(lambdaParameter => lambdaParameter);
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + FuncWithAttributeCode, VerifyCS.Diagnostic().WithSpan(5, 19, 5, 39));
    }

    [Fact]
    public async Task Cast()
    {
        const string inputCode =
            """
            public static class C
            {
                public static void M(int parameter)
                {
                    F<object>.TakesFunc(() => (object)parameter);
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + FuncWithAttributeCode, VerifyCS.Diagnostic().WithSpan(5, 19, 5, 42));
    }

    [Fact]
    public async Task ObjectCreation()
    {
        const string inputCode =
            """
            public static class C
            {
                public static void M(int parameter)
                {
                    F<object>.TakesFunc(() => new object());
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + FuncWithAttributeCode, VerifyCS.Diagnostic().WithSeverity(DiagnosticSeverity.Info).WithSpan(5, 19, 5, 37));
    }

    [Fact]
    public async Task AnonymousObjectCreation()
    {
        const string inputCode =
            """
            public static class C
            {
                public static void M(int parameter)
                {
                    F<object>.TakesFunc(() => new { X = 10 });
                    F<object>.TakesFunc(() => new { X = 10, Y = "foo" });
                    F<object>.TakesFunc(() => new { });
                    F<object>.TakesFunc(() => new { X = new object() });
                    F<object>.TakesFunc(() => new { X = 10, Y = GetBar() });
                }

                public static int GetBar() { return 0; }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(
            inputCode + Environment.NewLine + FuncWithAttributeCode,
            VerifyCS.Diagnostic().WithSpan(5, 19, 5, 39),
            VerifyCS.Diagnostic().WithSpan(6, 19, 6, 50),
            VerifyCS.Diagnostic().WithSpan(7, 19, 7, 32),
            VerifyCS.Diagnostic().WithSeverity(DiagnosticSeverity.Info).WithSpan(8, 19, 8, 49));
    }

    [Fact]
    public async Task ObjectCreationCounterExample()
    {
        const string inputCode =
            """
            public static class C
            {
                public static void M(int parameter)
                {
                    F<Foo>.TakesFunc(() => new Foo(GetBar()));
                    F<Foo>.TakesFunc(() => new Foo(0) { Bar = GetBar() });
                }

                public static int GetBar() { return 0; }

                public sealed class Foo
                {
                    public Foo() { }

                    public Foo(int bar) { }

                    public int Bar { get; set; }
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + FuncWithAttributeCode);
    }

    [Fact]
    public async Task StaticProperty()
    {
        const string inputCode =
            """
            public static partial class C
            {
                public static void M(int parameter)
                {
                    F<int>.TakesFunc(() => Foo.Value);
                }

                public static class Foo
                {
                    public static int Value { get; }
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + FuncWithAttributeCode, VerifyCS.Diagnostic().WithSeverity(DiagnosticSeverity.Info).WithSpan(5, 19, 5, 34));
    }

    [Fact]
    public async Task StaticField()
    {
        const string inputCode =
            """
            public static partial class C
            {
                public static void M(int parameter)
                {
                    F<int>.TakesFunc(() => Foo.Value);
                }

                public static class Foo
                {
                    public static readonly int Value;
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + FuncWithAttributeCode, VerifyCS.Diagnostic().WithSpan(5, 19, 5, 34));
    }
}
