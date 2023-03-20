using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpAnalyzerVerifier<Funcky.BuiltinAnalyzers.TryGetValueAnalyzer>;

namespace Funcky.Analyzers.Test;

public sealed class TryGetValueTest
{
    private const string TryGetValueCode = """
        namespace Funcky.Monads
        {
            public readonly struct Option<TItem>
            {
                public bool TryGetValue(out TItem item) { item = default!; return false; }
            }
        }
        """;

    [Fact]
    public async Task UseOfTryGetValueInRegularMethodIsDisallowed()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static void M()
                {
                    var option = new Option<int>();
                    option.TryGetValue(out _);
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode, VerifyCS.Diagnostic().WithSpan(8, 9, 8, 34));
    }

    [Fact]
    public async Task UseOfTryGetValueIsDisallowedInWhileBlock()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static void M()
                {
                    var option = new Option<int>();
                    while (true) { option.TryGetValue(out _); }
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode, VerifyCS.Diagnostic().WithSpan(8, 24, 8, 49));
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedInWhileCondition()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static void M()
                {
                    var option = new Option<int>();
                    while (option.TryGetValue(out _)) { }
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Fact]
    public async Task UseOfTryGetValueIsDisallowedInDoWhileBlock()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static void M()
                {
                    var option = new Option<int>();
                    do { option.TryGetValue(out _); } while (true);
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode, VerifyCS.Diagnostic().WithSpan(8, 14, 8, 39));
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedInDoWhileCondition()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static void M()
                {
                    var option = new Option<int>();
                    do { } while (option.TryGetValue(out _));
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Fact]
    public async Task UseOfTryGetValueIsDisallowedInForLoopBlock()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static void M()
                {
                    for (var option = new Option<int>();;)
                    {
                        option.TryGetValue(out _);
                    }
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode, VerifyCS.Diagnostic().WithSpan(9, 13, 9, 38));
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedInForLoopCondition()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static void M()
                {
                    for (var option = new Option<int>(); option.TryGetValue(out var v);)
                    {
                    }
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Theory]
    [InlineData("IEnumerable<int>")]
    [InlineData("IEnumerator<int>")]
    public async Task UseOfTryGetValueIsAllowedInWhileConditionOfIterator(string returnType)
    {
        var inputCode = $$"""
            using Funcky.Monads;
            using System.Collections.Generic;

            public static class C
            {
                public static {{returnType}} M()
                {
                    var option = new Option<int>();
                    while (option.TryGetValue(out _)) { yield break; }
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Theory]
    [InlineData("IAsyncEnumerable<int>")]
    [InlineData("IAsyncEnumerator<int>")]
    public async Task UseOfTryGetValueIsAllowedInWhileConditionOfAsyncIterator(string returnType)
    {
        var inputCode = $$"""
            using Funcky.Monads;
            using System.Collections.Generic;
            using System.Threading.Tasks;

            public static class C
            {
                public static async {{returnType}} M()
                {
                    await Task.CompletedTask;
                    var option = new Option<int>();
                    while (option.TryGetValue(out _)) { yield break; }
                }
            }
            """;

        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedInDoWhileConditionOfIterator()
    {
        const string inputCode = """
            using Funcky.Monads;
            using System.Collections.Generic;

            public static class C
            {
                public static IEnumerable<int> M()
                {
                    var option = new Option<int>();
                    do { yield break; } while (option.TryGetValue(out _));
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedInForLoopConditionOfIterator()
    {
        const string inputCode = """
            using Funcky.Monads;
            using System.Collections.Generic;

            public static class C
            {
                public static IEnumerable<int> M()
                {
                    for (var option = new Option<int>(); option.TryGetValue(out var v);)
                    {
                        yield break;
                    }
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedInCatchFilterClause()
    {
        const string inputCode = """
            using Funcky.Monads;
            using System.Collections.Generic;

            public static class C
            {
                public static void M()
                {
                    var option = new Option<int>();
                    try { }
                    catch when (option.TryGetValue(out _)) { }
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Fact]
    public async Task ErrorIsNotSuppressableWithPragma()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static void M()
                {
                    #pragma warning disable λ0001
                    var option = new Option<int>();
                    option.TryGetValue(out _);
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode, VerifyCS.Diagnostic().WithSpan(9, 9, 9, 34));
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedAsChildExpressionOfWhileConditionInIterator()
    {
        const string inputCode = """
            using Funcky.Monads;
            using System.Collections.Generic;

            public static class C
            {
                public static IEnumerable<int> M()
                {
                    var option = new Option<int>();
                    while (option.TryGetValue(out _) || True()) { yield break; }
                }

                private static bool True() => true;
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedAsChildExpressionOfDoWhileConditionInIterator()
    {
        const string inputCode = """
            using Funcky.Monads;
            using System.Collections.Generic;

            public static class C
            {
                public static IEnumerable<int> M()
                {
                    var option = new Option<int>();
                    do { yield break; } while (option.TryGetValue(out _) || True());
                }

                private static bool True() => true;
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedInWhileConditionOfIteratorLocalFunction()
    {
        const string inputCode = """
            using Funcky.Monads;
            using System.Collections.Generic;

            public static class C
            {
                public static void M()
                {
                    IEnumerable<int> I()
                    {
                        var option = new Option<int>();
                        while (option.TryGetValue(out _)) { yield break; }
                    }
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedInIfConditionOfIteratorWhenIfContainsYieldReturn()
    {
        const string inputCode = """
            using Funcky.Monads;
            using System.Collections.Generic;

            public static class C
            {
                public static void M()
                {
                    IEnumerable<int> I()
                    {
                        var option = new Option<int>();
                        if (option.TryGetValue(out var item)) yield return item;
                    }
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedInIfConditionOfIteratorWhenIfContainsYieldBreak()
    {
        const string inputCode = """
            using Funcky.Monads;
            using System.Collections.Generic;

            public static class C
            {
                public static void M()
                {
                    IEnumerable<int> I()
                    {
                        var option = new Option<int>();
                        if (!option.TryGetValue(out var item)) yield break;
                        yield return item;
                    }
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedInIfConditionOfIteratorWhenElseContainsYieldReturn()
    {
        const string inputCode = """
            using Funcky.Monads;
            using System.Collections.Generic;

            public static class C
            {
                public static void M()
                {
                    IEnumerable<int> I()
                    {
                        var option = new Option<int>();
                        if (!option.TryGetValue(out var item)) { }
                        else { yield return item; }
                    }
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Fact]
    public async Task UseOfTryGetValueIsDisallowedInIfConditionOfIteratorWhenItContainsNoYield()
    {
        const string inputCode = """
            using Funcky.Monads;
            using System.Collections.Generic;

            public static class C
            {
                public static void M()
                {
                    IEnumerable<int> I()
                    {
                        var option = new Option<int>();
                        if (option.TryGetValue(out var item)) { }
                        yield break;
                    }
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode, VerifyCS.Diagnostic().WithSpan(11, 17, 11, 49));
    }

    [Fact]
    public async Task UseOfTryGetValueAsMethodReferenceIsDisallowed()
    {
        const string inputCode = """
            #nullable enable

            using Funcky.Monads;
            using System.Collections.Generic;

            public static class C
            {
                public delegate bool TryGetValueFunc<T>(out T? item);

                public static void M(Option<int> option)
                {
                    TryGetValueFunc<int> func = option.TryGetValue;
                }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode, VerifyCS.Diagnostic().WithSpan(12, 37, 12, 55));
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedInRazorComponents()
    {
        const string inputCode = """
            #nullable enable

            using Funcky.Monads;
            using System.Collections.Generic;

            public class C : Microsoft.AspNetCore.Components.ComponentBase
            {
                public static void M(Option<int> option)
                {
                    #line 10 "Example.razor"
                    if (option.TryGetValue(out _))
                    {
                    }
                }
            }

            public class D : OtherComponentBase
            {
                public static void M(Option<int> option)
                {
                    #line 10 "Example.razor"
                    if (option.TryGetValue(out _))
                    {
                    }
                }
            }

            public class OtherComponentBase : Microsoft.AspNetCore.Components.ComponentBase { }

            namespace Microsoft.AspNetCore.Components
            {
                public class ComponentBase { }
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }
}
