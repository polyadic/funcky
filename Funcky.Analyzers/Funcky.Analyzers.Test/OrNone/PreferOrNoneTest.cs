using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.OrNone.PreferOrNoneAnalyzer, Funcky.Analyzers.OrNone.PreferOrNoneCodeFix>;

namespace Funcky.Analyzers.Test.OrNone;

public sealed class PreferOrNoneTest
{
    private const string FunckyStubs = """

        namespace Funcky.Monads
        {
            public readonly struct Option<TItem>
            {
                public TItem GetOrElse(TItem fallback) => fallback;
                public TItem GetOrElse(System.Func<TItem> fallback) => fallback();
            }
        }

        namespace Funcky.Extensions
        {
            using System;
            using System.Collections.Generic;
            using Funcky.Monads;

            public static class EnumerableExtensions
            {
                public static Option<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source) => default;
                public static Option<TSource> FirstOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) => default;
                public static Option<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source) => default;
                public static Option<TSource> LastOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) => default;
                public static Option<TSource> SingleOrNone<TSource>(this IEnumerable<TSource> source) => default;
                public static Option<TSource> SingleOrNone<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate) => default;
                public static Option<TSource> ElementAtOrNone<TSource>(this IEnumerable<TSource> source, int index) => default;
            }
        }
        """;

    [Fact]
    public async Task CoalesceAfterFirstOrDefaultIsReportedAndRewrittenToGetOrElseWithUsing()
    {
        const string inputCode = """
            using System.Collections.Generic;
            using System.Linq;

            public static class C
            {
                public static string M(IEnumerable<string> values) => {|λ1012:values.FirstOrDefault()|} ?? "fallback";
            }
            """;
        const string fixedCode = """
            using System.Collections.Generic;
            using System.Linq;
            using Funcky.Extensions;

            public static class C
            {
                public static string M(IEnumerable<string> values) => values.FirstOrNone().GetOrElse("fallback");
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunckyStubs, fixedCode + FunckyStubs);
    }

    [Fact]
    public async Task EveryOrDefaultMethodAndOverloadIsRewritten()
    {
        const string inputCode = """
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using Funcky.Extensions;

            public static class C
            {
                public static string First(IEnumerable<string> values) => {|λ1012:values.FirstOrDefault(value => value.Length > 2)|} ?? "fallback";

                public static string Last(IEnumerable<string> values) => {|λ1012:values.LastOrDefault()|} ?? "fallback";

                public static string Single(IEnumerable<string> values) => {|λ1012:values.SingleOrDefault(value => value.Length > 2)|} ?? "fallback";

                public static string ElementAt(IEnumerable<string> values) => {|λ1012:values.ElementAtOrDefault(2)|} ?? "fallback";

                public static string Parenthesized(IEnumerable<string> values) => ({|λ1012:values.FirstOrDefault()|}) ?? "fallback";

                public static string Throwing(IEnumerable<string> values) => {|λ1012:values.FirstOrDefault()|} ?? throw new InvalidOperationException("empty");
            }
            """;
        const string fixedCode = """
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using Funcky.Extensions;

            public static class C
            {
                public static string First(IEnumerable<string> values) => values.FirstOrNone(value => value.Length > 2).GetOrElse("fallback");

                public static string Last(IEnumerable<string> values) => values.LastOrNone().GetOrElse("fallback");

                public static string Single(IEnumerable<string> values) => values.SingleOrNone(value => value.Length > 2).GetOrElse("fallback");

                public static string ElementAt(IEnumerable<string> values) => values.ElementAtOrNone(2).GetOrElse("fallback");

                public static string Parenthesized(IEnumerable<string> values) => values.FirstOrNone().GetOrElse("fallback");

                public static string Throwing(IEnumerable<string> values) => values.FirstOrNone().GetOrElse(() => throw new InvalidOperationException("empty"));
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunckyStubs, fixedCode + FunckyStubs);
    }

    [Fact]
    public async Task UsingIsAddedInsideTheNamespaceWhenTheUsingsAreThere()
    {
        const string inputCode = """
            namespace Example
            {
                using System.Collections.Generic;
                using System.Linq;

                public static class C
                {
                    public static string M(IEnumerable<string> values) => {|λ1012:values.FirstOrDefault()|} ?? "fallback";
                }
            }
            """;
        const string fixedCode = """
            namespace Example
            {
                using System.Collections.Generic;
                using System.Linq;
                using Funcky.Extensions;

                public static class C
                {
                    public static string M(IEnumerable<string> values) => values.FirstOrNone().GetOrElse("fallback");
                }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunckyStubs, fixedCode + FunckyStubs);
    }

    [Fact]
    public async Task ThreeFixesThatAllNeedTheUsingAreAppliedInOnePass()
    {
        const string inputCode = """
            using System.Collections.Generic;
            using System.Linq;

            public static class C
            {
                public static string First(IEnumerable<string> values) => {|λ1012:values.FirstOrDefault()|} ?? "fallback";

                public static string Last(IEnumerable<string> values) => {|λ1012:values.LastOrDefault()|} ?? "fallback";

                public static string Single(IEnumerable<string> values) => {|λ1012:values.SingleOrDefault()|} ?? "fallback";

                public static string ElementAt(IEnumerable<string> values) => {|λ1012:values.ElementAtOrDefault(2)|} ?? "fallback";
            }
            """;
        const string fixedCode = """
            using System.Collections.Generic;
            using System.Linq;
            using Funcky.Extensions;

            public static class C
            {
                public static string First(IEnumerable<string> values) => values.FirstOrNone().GetOrElse("fallback");

                public static string Last(IEnumerable<string> values) => values.LastOrNone().GetOrElse("fallback");

                public static string Single(IEnumerable<string> values) => values.SingleOrNone().GetOrElse("fallback");

                public static string ElementAt(IEnumerable<string> values) => values.ElementAtOrNone(2).GetOrElse("fallback");
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunckyStubs, fixedCode + FunckyStubs);
    }

    [Fact]
    public async Task ThreeFixesInANamespaceThatAllNeedTheUsingAreAppliedInOnePass()
    {
        const string inputCode = """
            namespace Example
            {
                using System.Collections.Generic;
                using System.Linq;

                public static class C
                {
                    public static string First(IEnumerable<string> values) => {|λ1012:values.FirstOrDefault()|} ?? "fallback";

                    public static string Last(IEnumerable<string> values) => {|λ1012:values.LastOrDefault()|} ?? "fallback";

                    public static string Single(IEnumerable<string> values) => {|λ1012:values.SingleOrDefault()|} ?? "fallback";
                }
            }
            """;
        const string fixedCode = """
            namespace Example
            {
                using System.Collections.Generic;
                using System.Linq;
                using Funcky.Extensions;

                public static class C
                {
                    public static string First(IEnumerable<string> values) => values.FirstOrNone().GetOrElse("fallback");

                    public static string Last(IEnumerable<string> values) => values.LastOrNone().GetOrElse("fallback");

                    public static string Single(IEnumerable<string> values) => values.SingleOrNone().GetOrElse("fallback");
                }
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunckyStubs, fixedCode + FunckyStubs);
    }

    [Fact]
    public async Task OtherNullChecksAreReportedWithoutFix()
    {
        const string inputCode = """
            using System.Collections.Generic;
            using System.Linq;

            public static class C
            {
                public static bool IsNull(IEnumerable<string> values) => {|λ1012:values.FirstOrDefault()|} is null;

                public static bool IsNotNull(IEnumerable<string> values) => {|λ1012:values.FirstOrDefault()|} is not null;

                public static bool EqualsNull(IEnumerable<string> values) => {|λ1012:values.FirstOrDefault()|} == null;

                public static bool NullNotEquals(IEnumerable<string> values) => null != {|λ1012:values.LastOrDefault()|};

                public static int Length(IEnumerable<string> values) => {|λ1012:values.SingleOrDefault()|}?.Length ?? 0;

                public static int Pattern(IEnumerable<string> values)
                    => {|λ1012:values.FirstOrDefault()|} is { } first ? first.Length : -1;

                public static int NegatedPattern(IEnumerable<string> values)
                    => {|λ1012:values.FirstOrDefault()|} is not { } first ? -1 : first.Length;

                public static int TypePattern(IEnumerable<object> values)
                    => {|λ1012:values.FirstOrDefault()|} is string text ? text.Length : -1;
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + FunckyStubs);
    }

    [Fact]
    public async Task OrDefaultWithoutNullCheckIsNotReported()
    {
        const string inputCode = """
            using System.Collections.Generic;
            using System.Linq;

            public static class C
            {
                public static string M(IEnumerable<string> values) => values.FirstOrDefault();

                public static int N(IEnumerable<string> values)
                {
                    var first = values.FirstOrDefault();
                    return first is null ? -1 : first.Length;
                }

                public static bool O(IEnumerable<string> values) => values.FirstOrDefault() == "hello";

                public static bool P(IEnumerable<string> values) => values.FirstOrDefault() is { Length: > 2 };
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + FunckyStubs);
    }

    [Fact]
    public async Task ValueTypesNullableElementsAndQueryablesAreNotReported()
    {
        const string inputCode = """
            #nullable enable
            using System.Collections.Generic;
            using System.Linq;

            public static class C
            {
                public static int Int(IEnumerable<int> values) => values.FirstOrDefault() == 0 ? -1 : 1;

                public static int NullableInt(IEnumerable<int?> values) => values.FirstOrDefault() ?? -1;

                public static string NullableString(IEnumerable<string?> values) => values.FirstOrDefault() ?? "fallback";

                public static string Queryable(IQueryable<string> values) => values.FirstOrDefault() ?? "fallback";

                public static string Static(IEnumerable<string> values) => Enumerable.FirstOrDefault(values) ?? "fallback";
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + FunckyStubs);
    }

    [Fact]
    public async Task ExplicitTypeArgumentsStayWhenRenaming()
    {
        const string inputCode = """
            using System.Collections.Generic;
            using System.Linq;
            using Funcky.Extensions;

            public static class C
            {
                public static object M(IEnumerable<string> values) => {|λ1012:values.FirstOrDefault<object>()|} ?? new object();
            }
            """;
        const string fixedCode = """
            using System.Collections.Generic;
            using System.Linq;
            using Funcky.Extensions;

            public static class C
            {
                public static object M(IEnumerable<string> values) => values.FirstOrNone<object>().GetOrElse(() => new object());
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunckyStubs, fixedCode + FunckyStubs);
    }

    [Fact]
    public async Task CoalesceWithAWiderResultTypeIsReportedWithoutFix()
    {
        const string inputCode = """
            using System.Collections.Generic;
            using System.Linq;
            using Funcky.Extensions;

            public static class C
            {
                public static object M(IEnumerable<string> values) => {|λ1012:values.FirstOrDefault()|} ?? new object();

                public static object N(IEnumerable<string> values, object fallback) => {|λ1012:values.FirstOrDefault()|} ?? fallback;
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunckyStubs, inputCode + FunckyStubs);
    }

    [Fact]
    public async Task FallbackThatDoesWorkIsPassedLazily()
    {
        const string inputCode = """
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using Funcky.Extensions;

            public static class C
            {
                public static string Call(IEnumerable<string> values) => {|λ1012:values.FirstOrDefault()|} ?? CreateFallback();

                public static string Creation(IEnumerable<string> values) => {|λ1012:values.FirstOrDefault()|} ?? new string('x', 3);

                public static string Interpolated(IEnumerable<string> values, int count) => {|λ1012:values.FirstOrDefault()|} ?? $"fallback {count}";

                public static string NestedThrow(IEnumerable<string> values, string other) => {|λ1012:values.FirstOrDefault()|} ?? (other ?? throw new InvalidOperationException());

                public static string Variable(IEnumerable<string> values, string other) => {|λ1012:values.FirstOrDefault()|} ?? other;

                public static string Field(IEnumerable<string> values) => {|λ1012:values.FirstOrDefault()|} ?? string.Empty;

                private static string CreateFallback() => "fallback";
            }
            """;
        const string fixedCode = """
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using Funcky.Extensions;

            public static class C
            {
                public static string Call(IEnumerable<string> values) => values.FirstOrNone().GetOrElse(() => CreateFallback());

                public static string Creation(IEnumerable<string> values) => values.FirstOrNone().GetOrElse(() => new string('x', 3));

                public static string Interpolated(IEnumerable<string> values, int count) => values.FirstOrNone().GetOrElse(() => $"fallback {count}");

                public static string NestedThrow(IEnumerable<string> values, string other) => values.FirstOrNone().GetOrElse(() => other ?? throw new InvalidOperationException());

                public static string Variable(IEnumerable<string> values, string other) => values.FirstOrNone().GetOrElse(other);

                public static string Field(IEnumerable<string> values) => values.FirstOrNone().GetOrElse(string.Empty);

                private static string CreateFallback() => "fallback";
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunckyStubs, fixedCode + FunckyStubs);
    }

    [Fact]
    public async Task FallbackThatCannotMoveIntoALambdaIsReportedWithoutFix()
    {
        const string inputCode = """
            using System.Collections.Generic;
            using System.Linq;
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static async Task<string> Awaited(IEnumerable<string> values) => {|λ1012:values.FirstOrDefault()|} ?? await Task.FromResult("fallback");

                public static string RefParameter(IEnumerable<string> values, ref string seed) => {|λ1012:values.FirstOrDefault()|} ?? Create(seed);

                private static string Create(string seed) => seed;
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunckyStubs, inputCode + FunckyStubs);
    }

    [Fact]
    public async Task UsingIsAddedOutsideAConditionalRegion()
    {
        const string inputCode = """
            using System.Collections.Generic;
            using System.Linq;
            #if DEBUG
            using System.Diagnostics;
            #endif

            public static class C
            {
                public static string M(IEnumerable<string> values) => {|λ1012:values.FirstOrDefault()|} ?? "fallback";
            }
            """;
        const string fixedCode = """
            using System.Collections.Generic;
            using System.Linq;
            using Funcky.Extensions;
            #if DEBUG
            using System.Diagnostics;
            #endif

            public static class C
            {
                public static string M(IEnumerable<string> values) => values.FirstOrNone().GetOrElse("fallback");
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunckyStubs, fixedCode + FunckyStubs);
    }

    [Fact]
    public async Task NonNullableElementsAreReportedWithNullableAnnotatedLinq()
    {
        const string inputCode = """
            #nullable enable
            using System.Collections.Generic;
            using System.Linq;
            using Funcky.Extensions;

            public static class C
            {
                public static string M(IEnumerable<string> values) => {|λ1012:values.FirstOrDefault()|} ?? "fallback";

                public static string N(IEnumerable<string?> values) => values.FirstOrDefault() ?? "fallback";
            }
            """;
        const string fixedCode = """
            #nullable enable
            using System.Collections.Generic;
            using System.Linq;
            using Funcky.Extensions;

            public static class C
            {
                public static string M(IEnumerable<string> values) => values.FirstOrNone().GetOrElse("fallback");

                public static string N(IEnumerable<string?> values) => values.FirstOrDefault() ?? "fallback";
            }
            """;
        await new VerifyCS.Test
        {
            ReferenceAssemblies = ReferenceAssemblies.Net.Net80,
            TestCode = inputCode + FunckyStubs,
            FixedCode = fixedCode + FunckyStubs,
        }.RunAsync();
    }
}
