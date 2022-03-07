using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpAnalyzerVerifier<Funcky.BuiltinAnalyzers.TryGetValueAnalyzer>;

namespace Funcky.Analyzers.Test;

public sealed class TryGetValueTest
{
    private const string TryGetValueCode = @"namespace Funcky.Monads
{
    public readonly struct Option<TItem>
    {
        public bool TryGetValue(out TItem item) { item = default!; return false; }
    }
}";

    [Fact]
    public async Task UseOfTryGetValueInRegularMethodIsDisallowed()
    {
        const string inputCode = @"
using Funcky.Monads;

public static class C
{
    public static void M()
    {
        var option = new Option<int>();
        option.TryGetValue(out _);
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode, VerifyCS.Diagnostic().WithSpan(9, 9, 9, 34));
    }

    [Fact]
    public async Task UseOfTryGetValueIsDisallowedInWhileBlock()
    {
        const string inputCode = @"
using Funcky.Monads;

public static class C
{
    public static void M()
    {
        var option = new Option<int>();
        while (true) { option.TryGetValue(out _); }
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode, VerifyCS.Diagnostic().WithSpan(9, 24, 9, 49));
    }

    [Fact]
    public async Task UseOfTryGetValueIsDisallowedInWhileCondition()
    {
        const string inputCode = @"
using Funcky.Monads;

public static class C
{
    public static void M()
    {
        var option = new Option<int>();
        while (option.TryGetValue(out _)) { }
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode, VerifyCS.Diagnostic().WithSpan(9, 16, 9, 41));
    }

    [Fact]
    public async Task UseOfTryGetValueIsDisallowedInDoWhileBlock()
    {
        const string inputCode = @"
using Funcky.Monads;

public static class C
{
    public static void M()
    {
        var option = new Option<int>();
        do { option.TryGetValue(out _); } while (true);
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode, VerifyCS.Diagnostic().WithSpan(9, 14, 9, 39));
    }

    [Fact]
    public async Task UseOfTryGetValueIsDisallowedInDoWhileCondition()
    {
        const string inputCode = @"
using Funcky.Monads;

public static class C
{
    public static void M()
    {
        var option = new Option<int>();
        do { } while (option.TryGetValue(out _));
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode, VerifyCS.Diagnostic().WithSpan(9, 23, 9, 48));
    }

    [Theory]
    [InlineData("IEnumerable<int>")]
    [InlineData("IEnumerator<int>")]
    public async Task UseOfTryGetValueIsAllowedInWhileConditionOfIterator(string returnType)
    {
        var inputCode = $@"
using Funcky.Monads;
using System.Collections.Generic;

public static class C
{{
    public static {returnType} M()
    {{
        var option = new Option<int>();
        while (option.TryGetValue(out _)) {{ yield break; }}
    }}
}}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Theory]
    [InlineData("IAsyncEnumerable<int>")]
    [InlineData("IAsyncEnumerator<int>")]
    public async Task UseOfTryGetValueIsAllowedInWhileConditionOfAsyncIterator(string returnType)
    {
        var inputCode = $@"
using Funcky.Monads;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class C
{{
    public static async {returnType} M()
    {{
        await Task.CompletedTask;
        var option = new Option<int>();
        while (option.TryGetValue(out _)) {{ yield break; }}
    }}
}}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedInDoWhileConditionOfIterator()
    {
        const string inputCode = @"
using Funcky.Monads;
using System.Collections.Generic;

public static class C
{
    public static IEnumerable<int> M()
    {
        var option = new Option<int>();
        do { yield break; } while (option.TryGetValue(out _));
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedInCatchFilterClause()
    {
        const string inputCode = @"
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
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Fact]
    public async Task ErrorIsNotSuppressableWithPragma()
    {
        const string inputCode = @"
using Funcky.Monads;

public static class C
{
    public static void M()
    {
        #pragma warning disable λ0001
        var option = new Option<int>();
        option.TryGetValue(out _);
    }
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode, VerifyCS.Diagnostic().WithSpan(10, 9, 10, 34));
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedAsChildExpressionOfWhileConditionInIterator()
    {
        const string inputCode = @"
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
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedAsChildExpressionOfDoWhileConditionInIterator()
    {
        const string inputCode = @"
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
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }

    [Fact]
    public async Task UseOfTryGetValueIsAllowedInWhileConditionOfIteratorLocalFunction()
    {
        const string inputCode = @"
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
}";
        await VerifyCS.VerifyAnalyzerAsync(inputCode + Environment.NewLine + TryGetValueCode);
    }
}
