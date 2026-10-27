using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.ExtensionForms.PreferEnumerableSequenceConstructorsAnalyzer, Funcky.Analyzers.PreferEnumerableSequenceConstructorsCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed class PreferEnumerableSequenceConstructorsTest
{
    // language=csharp
    private const string StubCode =
        """
        namespace Funcky
        {
            using System;
            using System.Collections.Generic;

            public static class Sequence
            {
                public static IReadOnlyList<TResult> Return<TResult>(TResult element) => new[] { element };

                public static IReadOnlyList<TResult> Return<TResult>(params TResult[] elements) => elements;

                public static IEnumerable<TResult> Successors<TResult>(TResult first, Func<TResult, TResult> successor) => new[] { first };

                public static IEnumerable<TResult> Generate<TResult>(Func<TResult> generator) => new[] { generator() };
            }
        }

        namespace Funcky.Extensions
        {
            using System;
            using System.Collections.Generic;
            using System.Linq;

            public static class EnumerableExtensions
            {
                extension(Enumerable)
                {
                    public static IReadOnlyList<TResult> Return<TResult>(TResult element) => new[] { element };

                    public static IReadOnlyList<TResult> Return<TResult>(params TResult[] elements) => elements;

                    public static IEnumerable<TResult> Successors<TResult>(TResult first, Func<TResult, TResult> successor) => new[] { first };
                }

                extension<TSource>(IEnumerable<TSource> source)
                {
                    public IEnumerable<TSource> Generate() => source;
                }
            }
        }
        """;

    [Fact]
    public async Task ReportsTheSequenceConstructorsAndRewritesThemToEnumerable()
    {
        const string inputCode =
            """
            using System.Collections.Generic;
            using System.Linq;
            using Funcky;
            using Funcky.Extensions;
            using static Funcky.Sequence;

            public static class C
            {
                public static void M()
                {
                    IReadOnlyList<int> a = {|λ1202:Sequence.Return(1)|};
                    IReadOnlyList<object> b = {|λ1202:Sequence.Return<object>(1, 2)|};
                    IEnumerable<int> c = {|λ1202:Funcky.Sequence.Successors(1, n => n + 1)|};
                    IReadOnlyList<int> d = {|λ1202:Return(1)|};
                    IEnumerable<int> e = Sequence.Generate(() => 1);
                }
            }
            """;
        const string fixedCode =
            """
            using System.Collections.Generic;
            using System.Linq;
            using Funcky;
            using Funcky.Extensions;
            using static Funcky.Sequence;

            public static class C
            {
                public static void M()
                {
                    IReadOnlyList<int> a = Enumerable.Return(1);
                    IReadOnlyList<object> b = Enumerable.Return<object>(1, 2);
                    IEnumerable<int> c = Enumerable.Successors(1, n => n + 1);
                    IReadOnlyList<int> d = Enumerable.Return(1);
                    IEnumerable<int> e = Sequence.Generate(() => 1);
                }
            }
            """;

        await VerifyCS.VerifyCodeFixAsync(inputCode + Environment.NewLine + StubCode, fixedCode + Environment.NewLine + StubCode);
    }

    [Fact]
    public async Task AddsTheUsingForTheExtensionWhenMissing()
    {
        const string inputCode =
            """
            using System.Collections.Generic;
            using Funcky;

            public static class C
            {
                public static IReadOnlyList<int> M() => {|λ1202:Sequence.Return(1)|};
            }
            """;
        const string fixedCode =
            """
            using System.Collections.Generic;
            using Funcky;
            using Funcky.Extensions;

            public static class C
            {
                public static IReadOnlyList<int> M() => System.Linq.Enumerable.Return(1);
            }
            """;

        await VerifyCS.VerifyCodeFixAsync(inputCode + Environment.NewLine + StubCode, fixedCode + Environment.NewLine + StubCode);
    }
}
