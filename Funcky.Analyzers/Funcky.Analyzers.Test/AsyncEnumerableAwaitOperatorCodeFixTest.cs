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
                public static void M(AsyncSource<int> source, int x)
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
                public static void M(AsyncSource<int> source, int x)
                {
                    _ = source.Inspect((x1, _) => Log(x1));
                }

                private static Task Log(int value) => Task.CompletedTask;
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
        };
}
