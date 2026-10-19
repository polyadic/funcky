using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.Flatten.PreferFlattenAnalyzer, Funcky.Analyzers.Flatten.PreferFlattenCodeFix>;

namespace Funcky.Analyzers.Test.Flatten;

public sealed class PreferFlattenTest
{
    private const string FunckyStubs = """

        namespace Funcky
        {
            public static class Functional
            {
                public static T Identity<T>(T x) => x;
            }
        }

        namespace Funcky.Monads
        {
            using System;

            public readonly struct Option<TItem>
            {
                public Option<TResult> SelectMany<TResult>(Func<TItem, Option<TResult>> selector) => default;
                public Option<TResult> SelectMany<TOption, TResult>(Func<TItem, Option<TOption>> selector, Func<TItem, TOption, TResult> resultSelector) => default;
                public Option<TResult> Select<TResult>(Func<TItem, TResult> selector) => default;
            }

            public readonly struct Result<TValidResult>
            {
                public Result<TResult> SelectMany<TResult>(Func<TValidResult, Result<TResult>> selector) => default;
            }

            public readonly struct Either<TLeft, TRight>
            {
                public Either<TLeft, TResult> SelectMany<TResult>(Func<TRight, Either<TLeft, TResult>> selector) => default;
            }

            public static class OptionExtensions
            {
                public static Option<T> Flatten<T>(this Option<Option<T>> option) => default;
            }

            public static class ResultExtensions
            {
                public static Result<T> Flatten<T>(this Result<Result<T>> result) => default;
            }

            public static class EitherExtensions
            {
                public static Either<TLeft, TRight> Flatten<TLeft, TRight>(this Either<TLeft, Either<TLeft, TRight>> either) => default;
            }
        }

        namespace Funcky.Extensions
        {
            using System.Collections.Generic;

            public static class EnumerableExtensions
            {
                public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable) => default!;
            }
        }
        """;

    [Fact]
    public async Task SelectManyWithIdentityOnOptionIsReportedAndReplaced()
    {
        const string inputCode = """
            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static Option<int> Lambda(Option<Option<int>> option) => {|λ1018:option.SelectMany(x => x)|};

                public static Option<int> Block(Option<Option<int>> option) => {|λ1018:option.SelectMany(x => { return x; })|};

                public static Option<int> MethodGroup(Option<Option<int>> option) => {|λ1018:option.SelectMany(Identity)|};

                public static Option<int> Chained(Option<Option<Option<int>>> option) => {|λ1018:{|λ1018:option.SelectMany(x => x)|}.SelectMany(x => x)|};
            }
            """;
        const string fixedCode = """
            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static Option<int> Lambda(Option<Option<int>> option) => option.Flatten();

                public static Option<int> Block(Option<Option<int>> option) => option.Flatten();

                public static Option<int> MethodGroup(Option<Option<int>> option) => option.Flatten();

                public static Option<int> Chained(Option<Option<Option<int>>> option) => option.Flatten().Flatten();
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunckyStubs, fixedCode + FunckyStubs);
    }

    [Fact]
    public async Task SelectManyWithIdentityOnResultAndEitherIsReportedAndReplaced()
    {
        const string inputCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Result<int> R(Result<Result<int>> result) => {|λ1018:result.SelectMany(x => x)|};

                public static Either<string, int> E(Either<string, Either<string, int>> either) => {|λ1018:either.SelectMany(x => x)|};
            }
            """;
        const string fixedCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Result<int> R(Result<Result<int>> result) => result.Flatten();

                public static Either<string, int> E(Either<string, Either<string, int>> either) => either.Flatten();
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunckyStubs, fixedCode + FunckyStubs);
    }

    [Fact]
    public async Task SelectManyWithIdentityOnEnumerableIsReplacedAndTheUsingAdded()
    {
        const string inputCode = """
            using System.Collections.Generic;
            using System.Linq;

            public static class C
            {
                public static IEnumerable<int> M(IEnumerable<IEnumerable<int>> values) => {|λ1018:values.SelectMany(x => x)|};

                public static IEnumerable<int> N(List<int[]> values) => {|λ1018:values.SelectMany(x => x)|}.Where(x => x > 0);
            }
            """;
        const string fixedCode = """
            using System.Collections.Generic;
            using System.Linq;
            using Funcky.Extensions;

            public static class C
            {
                public static IEnumerable<int> M(IEnumerable<IEnumerable<int>> values) => values.Flatten();

                public static IEnumerable<int> N(List<int[]> values) => values.Flatten().Where(x => x > 0);
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunckyStubs, fixedCode + FunckyStubs);
    }

    [Fact]
    public async Task UsingIsAddedForTheMonadNamespaceWhenTheTypeIsFullyQualified()
    {
        const string inputCode = """
            public static class C
            {
                public static Funcky.Monads.Option<int> M(Funcky.Monads.Option<Funcky.Monads.Option<int>> option) => {|λ1018:option.SelectMany(x => x)|};
            }
            """;
        const string fixedCode = """
            using Funcky.Monads;

            public static class C
            {
                public static Funcky.Monads.Option<int> M(Funcky.Monads.Option<Funcky.Monads.Option<int>> option) => option.Flatten();
            }
            """;
        await VerifyCS.VerifyCodeFixAsync(inputCode + FunckyStubs, fixedCode + FunckyStubs);
    }

    [Fact]
    public async Task OtherSelectorsAndFormsAreNotReported()
    {
        const string inputCode = """
            using System;
            using System.Collections.Generic;
            using System.Linq;
            using System.Linq.Expressions;
            using Funcky.Monads;

            public static class C
            {
                public static Option<int> Selector(Option<Option<int>> option) => option.SelectMany(x => x.Select(y => y + 1));

                public static Option<int> TwoSelectors(Option<Option<int>> option) => option.SelectMany(x => x, (x, y) => y);

                public static Option<int> Query(Option<Option<int>> option) => from x in option from y in x select y;

                public static IEnumerable<int> Static(IEnumerable<IEnumerable<int>> values) => Enumerable.SelectMany(values, x => x);

                public static IEnumerable<int> Indexed(IEnumerable<IEnumerable<int>> values) => values.SelectMany((x, _) => x);

                public static Expression<Func<IEnumerable<IEnumerable<int>>, IEnumerable<int>>> Tree() => values => values.SelectMany(x => x);

                public static Option<int> NotNested(Option<int> option) => option.SelectMany(x => Some(x));

                private static Option<int> Some(int x) => default;
            }
            """;
        await VerifyCS.VerifyAnalyzerAsync(inputCode + FunckyStubs);
    }
}
