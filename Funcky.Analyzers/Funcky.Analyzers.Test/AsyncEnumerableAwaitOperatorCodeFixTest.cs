using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Microsoft.CodeAnalysis.Testing.EmptyDiagnosticAnalyzer, Funcky.Analyzers.AsyncEnumerableAwaitOperatorCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed class AsyncEnumerableAwaitOperatorCodeFixTest
{
    private const string MissingXmlComment = "CS1591";

    // A stand-in for Funcky.Async: the fix only looks at the declaring type, the names and which parameters are delegates.
    // language=csharp
    private const string StubCode =
        """
        namespace Funcky.Extensions
        {
            using System;
            using System.Threading;
            using System.Threading.Tasks;

            public sealed class AsyncSource<T>
            {
            }

            public static class AsyncEnumerableExtensions
            {
                public static AsyncSource<T> Inspect<T>(this AsyncSource<T> source, Action<T> inspector) => source;

                public static AsyncSource<T> Inspect<T>(this AsyncSource<T> source, Func<T, CancellationToken, Task> inspector) => source;

                [Obsolete("Use Inspect with a (item, cancellationToken) delegate instead.")]
                public static AsyncSource<T> InspectAwait<T>(this AsyncSource<T> source, Func<T, Task> inspector) => source;

                [Obsolete("Use Inspect instead, which takes the same delegate.")]
                public static AsyncSource<T> InspectAwaitWithCancellation<T>(this AsyncSource<T> source, Func<T, CancellationToken, Task> inspector) => source;

                public static AsyncSource<TAccumulate> ExclusiveScan<T, TAccumulate>(this AsyncSource<T> source, TAccumulate seed, Func<TAccumulate, T, CancellationToken, Task<TAccumulate>> accumulator) => default!;

                [Obsolete("Use ExclusiveScan with a (accumulate, item, cancellationToken) delegate instead.")]
                public static AsyncSource<TAccumulate> ExclusiveScanAwait<T, TAccumulate>(this AsyncSource<T> source, TAccumulate seed, Func<TAccumulate, T, Task<TAccumulate>> accumulator) => default!;

                public static AsyncSource<TResult> AdjacentGroupBy<T, TKey, TResult>(this AsyncSource<T> source, Func<T, CancellationToken, Task<TKey>> keySelector, Func<TKey, T[], CancellationToken, Task<TResult>> resultSelector) => default!;

                [Obsolete("Use AdjacentGroupBy with (item, cancellationToken) selectors instead.")]
                public static AsyncSource<TResult> AdjacentGroupByAwait<T, TKey, TResult>(this AsyncSource<T> source, Func<T, Task<TKey>> keySelector, Func<TKey, T[], Task<TResult>> resultSelector) => default!;

                public static Task<T> FirstOrNoneAsync<T>(this AsyncSource<T> source, Func<T, bool> predicate, CancellationToken cancellationToken = default) => default!;

                public static Task<T> FirstOrNoneAsync<T>(this AsyncSource<T> source, Func<T, CancellationToken, Task<bool>> predicate, CancellationToken cancellationToken = default) => default!;

                [Obsolete("Use FirstOrNoneAsync with a (item, cancellationToken) delegate instead.")]
                public static Task<T> FirstOrNoneAwaitAsync<T>(this AsyncSource<T> source, Func<T, Task<bool>> predicate, CancellationToken cancellationToken = default) => default!;

                [Obsolete("Use FirstOrNoneAsync instead, which takes the same delegate.")]
                public static Task<T> FirstOrNoneAwaitWithCancellationAsync<T>(this AsyncSource<T> source, Func<T, CancellationToken, Task<bool>> predicate, CancellationToken cancellationToken = default) => default!;

                public static Task<TResult> PartitionAsync<T, TResult>(this AsyncSource<T> source, Func<T, CancellationToken, Task<bool>> predicate, Func<T[], T[], CancellationToken, Task<TResult>> resultSelector, CancellationToken cancellationToken = default) => default!;

                [Obsolete("Use PartitionAsync with (…, cancellationToken) delegates instead.")]
                public static Task<TResult> PartitionAwaitAsync<T, TResult>(this AsyncSource<T> source, Func<T, Task<bool>> predicate, Func<T[], T[], Task<TResult>> resultSelector, CancellationToken cancellationToken = default) => default!;

                [Obsolete("Not an operator of this family.")]
                public static AsyncSource<T> Legacy<T>(this AsyncSource<T> source) => source;
            }
        }
        """;

    [Fact]
    public async Task RenamesTheWithCancellationOverload()
    {
        const string inputCode =
            """
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source)
                {
                    _ = {|CS0618:source.InspectAwaitWithCancellation((x, cancellationToken) => Task.CompletedTask)|};
                    _ = {|CS0618:source.InspectAwaitWithCancellation<int>(async (x, cancellationToken) => await Task.Yield())|};
                }
            }
            """;
        const string fixedCode =
            """
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source)
                {
                    _ = source.Inspect((x, cancellationToken) => Task.CompletedTask);
                    _ = source.Inspect<int>(async (x, cancellationToken) => await Task.Yield());
                }
            }
            """;

        await CreateTest(inputCode, fixedCode).RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task AddsTheDiscardedTokenParameterToLambdas()
    {
        const string inputCode =
            """
            using System.Threading;
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source)
                {
                    _ = {|CS0618:source.InspectAwait(x => Task.CompletedTask)|};
                    _ = {|CS0618:source.InspectAwait(async x => await Task.Yield())|};
                    _ = {|CS0618:source.InspectAwait((x) => Task.CompletedTask)|};
                    _ = {|CS0618:source.InspectAwait((int x) => Task.CompletedTask)|};
                    _ = {|CS0618:source.InspectAwait(x =>
                    {
                        return Task.CompletedTask;
                    })|};
                    _ = {|CS0618:source.ExclusiveScanAwait(0, (sum, x) => Task.FromResult(sum + x))|};
                    _ = {|CS0618:source.AdjacentGroupByAwait(x => Task.FromResult(x), (key, group) => Task.FromResult(key))|};
                }
            }
            """;
        const string fixedCode =
            """
            using System.Threading;
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source)
                {
                    _ = source.Inspect((x, _) => Task.CompletedTask);
                    _ = source.Inspect(async (x, _) => await Task.Yield());
                    _ = source.Inspect((x, _) => Task.CompletedTask);
                    _ = source.Inspect((int x, CancellationToken _) => Task.CompletedTask);
                    _ = source.Inspect((x, _) =>
                    {
                        return Task.CompletedTask;
                    });
                    _ = source.ExclusiveScan(0, (sum, x, _) => Task.FromResult(sum + x));
                    _ = source.AdjacentGroupBy((x, _) => Task.FromResult(x), (key, group, _) => Task.FromResult(key));
                }
            }
            """;

        await CreateTest(inputCode, fixedCode).RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task WrapsAMethodGroupOrADelegateInAForwardingLambda()
    {
        const string inputCode =
            """
            using System;
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source, Func<int, int, Task<int>> accumulate)
                {
                    _ = {|CS0618:source.InspectAwait(Log)|};
                    _ = {|CS0618:source.ExclusiveScanAwait(0, accumulate)|};
                    _ = {|CS0618:AsyncEnumerableExtensions.InspectAwait(source, Log)|};
                }

                private static Task Log(int x) => Task.CompletedTask;
            }
            """;
        const string fixedCode =
            """
            using System;
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source, Func<int, int, Task<int>> accumulate)
                {
                    _ = source.Inspect((x, _) => Log(x));
                    _ = source.ExclusiveScan(0, (x, y, _) => accumulate(x, y));
                    _ = AsyncEnumerableExtensions.Inspect(source, (x, _) => Log(x));
                }

                private static Task Log(int x) => Task.CompletedTask;
            }
            """;

        await CreateTest(inputCode, fixedCode).RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task AvoidsParameterNamesThatAreInScope()
    {
        const string inputCode =
            """
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source, int x, string x1)
                {
                    _ = {|CS0618:source.InspectAwait(Log)|};
                }

                private static Task Log(int value) => Task.CompletedTask;
            }
            """;
        const string fixedCode =
            """
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source, int x, string x1)
                {
                    _ = source.Inspect((x2, _) => Log(x2));
                }

                private static Task Log(int value) => Task.CompletedTask;
            }
            """;

        await CreateTest(inputCode, fixedCode).RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task ChoosesATokenParameterNameThatPreservesDiscardsAndExistingParameters()
    {
        const string inputCode =
            """
            using System.Threading;
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source, AsyncSource<string> strings)
                {
                    _ = {|CS0618:source.InspectAwait(_ => Log(_))|};
                    _ = {|CS0618:source.InspectAwait((int _) => Log(_))|};
                    _ = {|CS0618:source.InspectAwait(x =>
                    {
                        _ = Log(x);
                        return Task.CompletedTask;
                    })|};
                    _ = {|CS0618:strings.InspectAwait(s => Task.FromResult(int.TryParse(s, out _)))|};
                }

                private static Task Log(int x) => Task.CompletedTask;
            }
            """;
        const string fixedCode =
            """
            using System.Threading;
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source, AsyncSource<string> strings)
                {
                    _ = source.Inspect((_, __) => Log(_));
                    _ = source.Inspect((int _, CancellationToken __) => Log(_));
                    _ = source.Inspect((x, __) =>
                    {
                        _ = Log(x);
                        return Task.CompletedTask;
                    });
                    _ = strings.Inspect((s, __) => Task.FromResult(int.TryParse(s, out _)));
                }

                private static Task Log(int x) => Task.CompletedTask;
            }
            """;

        await CreateTest(inputCode, fixedCode).RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task LooksThroughParenthesesAroundTheArgument()
    {
        const string inputCode =
            """
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source)
                {
                    _ = {|CS0618:source.InspectAwait((x => Log(x)))|};
                    _ = {|CS0618:source.InspectAwait(((Log)))|};
                    _ = {|CS0618:source.InspectAwait((delegate (int x) { return Task.CompletedTask; }))|};
                }

                private static Task Log(int x) => Task.CompletedTask;
            }
            """;
        const string fixedCode =
            """
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source)
                {
                    _ = source.Inspect(((x, _) => Log(x)));
                    _ = source.Inspect((x, _) => Log(x));
                    _ = {|CS0618:source.InspectAwait((delegate (int x) { return Task.CompletedTask; }))|};
                }

                private static Task Log(int x) => Task.CompletedTask;
            }
            """;

        await CreateTest(inputCode, fixedCode).RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task ParenthesizesACallableThatIsNotAPrimaryExpression()
    {
        const string inputCode =
            """
            using System;
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source)
                {
                    _ = {|CS0618:source.InspectAwait((Func<int, Task>)Log)|};
                }

                private static Task Log(int x) => Task.CompletedTask;
            }
            """;
        const string fixedCode =
            """
            using System;
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source)
                {
                    _ = source.Inspect((x, _) => ((Func<int, Task>)Log)(x));
                }

                private static Task Log(int x) => Task.CompletedTask;
            }
            """;

        await CreateTest(inputCode, fixedCode).RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task OffersNoFixForACallableThatWouldBeEvaluatedPerElement()
    {
        const string inputCode =
            """
            using System;
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source, Func<int, Task>[] inspectors, Factory factory)
                {
                    _ = {|CS0618:source.InspectAwait(CreateInspector())|};
                    _ = {|CS0618:source.InspectAwait(inspectors[0])|};
                    _ = {|CS0618:source.InspectAwait(Factory.Create().Inspector)|};
                    _ = {|CS0618:source.InspectAwait(factory.Inspector)|};
                    _ = {|CS0618:source.InspectAwait(factory.Nested.Inspector)|};
                }

                private static Func<int, Task> CreateInspector() => _ => Task.CompletedTask;
            }

            public sealed class Factory
            {
                public Func<int, Task> Inspector { get; } = _ => Task.CompletedTask;

                public Factory Nested => this;

                public static Factory Create() => new();
            }
            """;
        const string fixedCode =
            """
            using System;
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source, Func<int, Task>[] inspectors, Factory factory)
                {
                    _ = {|CS0618:source.InspectAwait(CreateInspector())|};
                    _ = {|CS0618:source.InspectAwait(inspectors[0])|};
                    _ = {|CS0618:source.InspectAwait(Factory.Create().Inspector)|};
                    _ = source.Inspect((x, _) => factory.Inspector(x));
                    _ = source.Inspect((x, _) => factory.Nested.Inspector(x));
                }

                private static Func<int, Task> CreateInspector() => _ => Task.CompletedTask;
            }

            public sealed class Factory
            {
                public Func<int, Task> Inspector { get; } = _ => Task.CompletedTask;

                public Factory Nested => this;

                public static Factory Create() => new();
            }
            """;

        await CreateTest(inputCode, fixedCode).RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task RenamesTheTerminalAggregations()
    {
        const string inputCode =
            """
            using System.Threading;
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static async Task M(AsyncSource<int> source, CancellationToken cancellationToken)
                {
                    _ = await {|CS0618:source.FirstOrNoneAwaitAsync(x => Task.FromResult(x > 1))|};
                    _ = await {|CS0618:source.FirstOrNoneAwaitAsync(x => Task.FromResult(x > 1), cancellationToken)|};
                    _ = await {|CS0618:source.FirstOrNoneAwaitWithCancellationAsync((x, token) => Task.FromResult(x > 1), cancellationToken)|};
                    _ = await {|CS0618:source.PartitionAwaitAsync(x => Task.FromResult(x > 1), (left, right) => Task.FromResult(left.Length))|};
                }
            }
            """;
        const string fixedCode =
            """
            using System.Threading;
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static async Task M(AsyncSource<int> source, CancellationToken cancellationToken)
                {
                    _ = await source.FirstOrNoneAsync((x, _) => Task.FromResult(x > 1));
                    _ = await source.FirstOrNoneAsync((x, _) => Task.FromResult(x > 1), cancellationToken);
                    _ = await source.FirstOrNoneAsync((x, token) => Task.FromResult(x > 1), cancellationToken);
                    _ = await source.PartitionAsync((x, _) => Task.FromResult(x > 1), (left, right, _) => Task.FromResult(left.Length));
                }
            }
            """;

        await CreateTest(inputCode, fixedCode).RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task AppliesTheSameRewritingRulesToTheTerminalAggregations()
    {
        const string inputCode =
            """
            using System;
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static async Task M(AsyncSource<string> strings, Func<string, Task<bool>> predicate)
                {
                    _ = await {|CS0618:strings.FirstOrNoneAwaitAsync(s => Task.FromResult(int.TryParse(s, out _)))|};
                    _ = await {|CS0618:strings.FirstOrNoneAwaitAsync(_ => Task.FromResult(_.Length > 1))|};
                    _ = await {|CS0618:strings.FirstOrNoneAwaitAsync(predicate)|};
                    _ = await {|CS0618:strings.FirstOrNoneAwaitAsync(CreatePredicate())|};
                }

                private static Func<string, Task<bool>> CreatePredicate() => _ => Task.FromResult(true);
            }
            """;
        const string fixedCode =
            """
            using System;
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static async Task M(AsyncSource<string> strings, Func<string, Task<bool>> predicate)
                {
                    _ = await strings.FirstOrNoneAsync((s, __) => Task.FromResult(int.TryParse(s, out _)));
                    _ = await strings.FirstOrNoneAsync((_, __) => Task.FromResult(_.Length > 1));
                    _ = await strings.FirstOrNoneAsync((x, _) => predicate(x));
                    _ = await {|CS0618:strings.FirstOrNoneAwaitAsync(CreatePredicate())|};
                }

                private static Func<string, Task<bool>> CreatePredicate() => _ => Task.FromResult(true);
            }
            """;

        await CreateTest(inputCode, fixedCode).RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task OffersNoFixForOtherObsoleteMembers()
    {
        const string inputCode =
            """
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source)
                {
                    _ = {|CS0618:source.Legacy()|};
                }
            }
            """;

        await CreateTest(inputCode, inputCode).RunAsync(CancellationToken.None);
    }

    private static VerifyCS.Test CreateTest(string inputCode, string fixedCode)
        => new()
        {
            TestCode = inputCode + Environment.NewLine + StubCode,
            FixedCode = fixedCode + Environment.NewLine + StubCode,
            CompilerDiagnostics = CompilerDiagnostics.Warnings,
            DisabledDiagnostics = { MissingXmlComment },
            FixedState = { MarkupHandling = MarkupMode.Allow },
        };
}
