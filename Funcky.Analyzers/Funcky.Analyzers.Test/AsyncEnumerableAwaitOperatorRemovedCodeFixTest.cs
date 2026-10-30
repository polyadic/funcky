using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Microsoft.CodeAnalysis.Testing.EmptyDiagnosticAnalyzer, Funcky.Analyzers.AsyncEnumerableAwaitOperatorCodeFix>;

namespace Funcky.Analyzers.Test;

/// <summary>The fix on the compile error CS1061 that a call of an <c>…Await</c> operator reports once the operator is gone (Funcky 4.0.0).
/// Nothing binds the name to Funcky there, so the fix goes by the name, the receiver and the shape of the arguments.</summary>
public sealed class AsyncEnumerableAwaitOperatorRemovedCodeFixTest
{
    private const string MissingXmlComment = "CS1591";

    // A stand-in for Funcky.Async 2.0 and the net10.0 asset of Funcky 4.0: only the replacements are left.
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

                public static AsyncSource<TAccumulate> ExclusiveScan<T, TAccumulate>(this AsyncSource<T> source, TAccumulate seed, Func<TAccumulate, T, CancellationToken, Task<TAccumulate>> accumulator) => default!;

                public static AsyncSource<TResult> AdjacentGroupBy<T, TKey, TResult>(this AsyncSource<T> source, Func<T, CancellationToken, Task<TKey>> keySelector, Func<TKey, T[], CancellationToken, Task<TResult>> resultSelector) => default!;

                public static Task<T> FirstOrNoneAsync<T>(this AsyncSource<T> source, Func<T, bool> predicate, CancellationToken cancellationToken = default) => default!;

                public static Task<T> FirstOrNoneAsync<T>(this AsyncSource<T> source, Func<T, CancellationToken, Task<bool>> predicate, CancellationToken cancellationToken = default) => default!;

                public static Task<TResult> PartitionAsync<T, TResult>(this AsyncSource<T> source, Func<T, CancellationToken, Task<bool>> predicate, Func<T[], T[], CancellationToken, Task<TResult>> resultSelector, CancellationToken cancellationToken = default) => default!;
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
                    _ = source.{|CS1061:InspectAwaitWithCancellation|}((x, cancellationToken) => Task.CompletedTask);
                    _ = source.{|CS1061:InspectAwaitWithCancellation<int>|}(async (x, cancellationToken) => await Task.Yield());
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
                    _ = source.{|CS1061:InspectAwait|}(x => Task.CompletedTask);
                    _ = source.{|CS1061:InspectAwait|}(async x => await Task.Yield());
                    _ = source.{|CS1061:InspectAwait|}((int x) => Task.CompletedTask);
                    _ = source.{|CS1061:InspectAwait|}(x =>
                    {
                        return Task.CompletedTask;
                    });
                    _ = source.{|CS1061:ExclusiveScanAwait|}(0, (sum, x) => Task.FromResult(sum + x));
                    _ = source.{|CS1061:AdjacentGroupByAwait|}(x => Task.FromResult(x), (key, group) => Task.FromResult(key));
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
                    _ = source.{|CS1061:InspectAwait|}(Log);
                    _ = source.{|CS1061:ExclusiveScanAwait|}(0, accumulate);
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
                }

                private static Task Log(int x) => Task.CompletedTask;
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
                    _ = await source.{|CS1061:FirstOrNoneAwaitAsync|}(x => Task.FromResult(x > 1));
                    _ = await source.{|CS1061:FirstOrNoneAwaitAsync|}(x => Task.FromResult(x > 1), cancellationToken);
                    _ = await source.{|CS1061:FirstOrNoneAwaitWithCancellationAsync|}((x, token) => Task.FromResult(x > 1), cancellationToken);
                    _ = await source.{|CS1061:PartitionAwaitAsync|}(x => Task.FromResult(x > 1), (left, right) => Task.FromResult(left.Length));
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
    public async Task OffersNoFixForACallableThatWouldBeEvaluatedPerElement()
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
                    _ = await strings.{|CS1061:FirstOrNoneAwaitAsync|}(predicate);
                    _ = await strings.{|CS1061:FirstOrNoneAwaitAsync|}(CreatePredicate());
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
                    _ = await strings.FirstOrNoneAsync((x, _) => predicate(x));
                    _ = await strings.{|CS1061:FirstOrNoneAwaitAsync|}(CreatePredicate());
                }

                private static Func<string, Task<bool>> CreatePredicate() => _ => Task.FromResult(true);
            }
            """;

        await CreateTest(inputCode, fixedCode).RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task OffersNoFixWhenTheCallIsNotOneOfTheRemovedOperators()
    {
        const string inputCode =
            """
            using System.Threading.Tasks;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(AsyncSource<int> source, string text)
                {
                    _ = text.{|CS1061:InspectAwait|}(x => Task.CompletedTask);
                    _ = source.{|CS1061:LegacyAwait|}(x => Task.CompletedTask);
                    _ = source.{|CS1061:InspectAwait|}(inspector: x => Task.CompletedTask);
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
            DisabledDiagnostics = { MissingXmlComment },
            FixedState = { MarkupHandling = MarkupMode.Allow },
        };
}
