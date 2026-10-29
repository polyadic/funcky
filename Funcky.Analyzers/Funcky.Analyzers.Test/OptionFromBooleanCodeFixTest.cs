#pragma warning disable SA1118 // StyleCop support for collection expressions is missing
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Microsoft.CodeAnalysis.Testing.EmptyDiagnosticAnalyzer, Funcky.Analyzers.OptionFromBooleanCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed class OptionFromBooleanCodeFixTest
{
    private const string ObsoleteMember = "CS0618";

    private const string MissingXmlComment = "CS1591";

    // language=csharp
    private const string StubCode =
        """
        namespace Funcky
        {
            public readonly struct Unit
            {
                public static Unit Value => default;
            }

            public static class Functional
            {
                public static T Identity<T>(T value) => value;
            }
        }

        namespace Funcky.Monads
        {
            public readonly struct Option<TItem>
                where TItem : notnull
            {
                public static Option<TItem> None => default;

                public static implicit operator Option<TItem>(TItem value) => default;

                public Option<TResult> SelectMany<TResult>(System.Func<TItem, Option<TResult>> selector) where TResult : notnull => default;
            }

            public static class Option
            {
                public static Option<TItem> Some<TItem>(TItem value) where TItem : notnull => default;

                [System.Obsolete("Use condition.Then(Unit.Value) instead. This method will be removed in Funcky 4.0.0.")]
                public static Option<Unit> FromBoolean(bool boolean) => default;

                [System.Obsolete("Use condition.Then(item) instead. This method will be removed in Funcky 4.0.0.")]
                public static Option<TItem> FromBoolean<TItem>(bool boolean, TItem item) where TItem : notnull => default;

                [System.Obsolete("Use condition.Then(selector) instead. This method will be removed in Funcky 4.0.0.")]
                public static Option<TItem> FromBoolean<TItem>(bool boolean, System.Func<TItem> selector) where TItem : notnull => default;
            }
        }

        namespace Funcky.Extensions
        {
            using Funcky.Monads;

            public static class BooleanExtensions
            {
                public static Option<TItem> Then<TItem>(this bool condition, TItem value) where TItem : notnull => default;

                public static Option<TItem> Then<TItem>(this bool condition, System.Func<TItem> selector) where TItem : notnull => default;

                public static Option<TItem> Then<TItem>(this bool condition, System.Func<Option<TItem>> selector) where TItem : notnull => default;
            }
        }
        """;

    [Fact]
    public async Task RewritesFromBooleanToThen()
    {
        const string inputCode =
            """
            using Funcky;
            using Funcky.Extensions;
            using Funcky.Monads;

            public static class C
            {
                public static void M(bool flag, int number, string text)
                {
                    Use(Option.FromBoolean(flag, number));
                    Use(Option.FromBoolean(number % 2 == 0, number));
                    Use(Option.FromBoolean(!flag, () => text));
                    Use(Option.FromBoolean<object>(flag, text));
                    Use(Option.FromBoolean(flag));
                    Use(Option.FromBoolean(item: number, boolean: flag));
                }

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;
        const string fixedCode =
            """
            using Funcky;
            using Funcky.Extensions;
            using Funcky.Monads;

            public static class C
            {
                public static void M(bool flag, int number, string text)
                {
                    Use(flag.Then(number));
                    Use((number % 2 == 0).Then(number));
                    Use((!flag).Then(() => text));
                    Use(flag.Then<object>(text));
                    Use(flag.Then(Unit.Value));
                    Use(flag.Then(number));
                }

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;

        var test = CreateTest(inputCode, fixedCode);
        test.ExpectedDiagnostics.AddRange(
        [
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(9, 13, 9, 45),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(10, 13, 10, 56),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(11, 13, 11, 50),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(12, 13, 12, 51),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(13, 13, 13, 37),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(14, 13, 14, 60),
        ]);
        await test.RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task RewritesTheFlatteningIdiomToThenWithAnOptionSelector()
    {
        const string inputCode =
            """
            using Funcky.Extensions;
            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(bool flag, Option<int> option)
                {
                    Use(Option.FromBoolean(flag, option).SelectMany(Identity));
                    Use(Option.FromBoolean(flag, () => option).SelectMany(Identity));
                }

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;
        const string fixedCode =
            """
            using Funcky.Extensions;
            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(bool flag, Option<int> option)
                {
                    Use(flag.Then(() => option));
                    Use(flag.Then(() => option));
                }

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;

        var test = CreateTest(inputCode, fixedCode);
        test.ExpectedDiagnostics.AddRange(
        [
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(9, 13, 9, 45),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(10, 13, 10, 51),
        ]);
        await test.RunAsync(CancellationToken.None);
    }

    // Three or more diagnostics: Fix-All has to rewrite every call in one pass although each fix on its own adds the same using (#1009).
    [Fact]
    public async Task AddsTheUsingForTheExtensionWhenMissing()
    {
        const string inputCode =
            """
            using Funcky.Monads;

            public static class C
            {
                public static void M(bool flag, int number)
                {
                    Use({|CS0618:Option.FromBoolean(flag, number)|});
                    Use({|CS0618:Option.FromBoolean(flag)|});
                    Use({|CS0618:Option.FromBoolean(flag, () => number)|});
                }

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;
        const string fixedCode =
            """
            using Funcky.Monads;
            using Funcky.Extensions;

            public static class C
            {
                public static void M(bool flag, int number)
                {
                    Use(flag.Then(number));
                    Use(flag.Then(Funcky.Unit.Value));
                    Use(flag.Then(() => number));
                }

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;

        await CreateTest(inputCode, fixedCode).RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task OffersNoFixForASelectorThatProducesANestedOption()
    {
        const string inputCode =
            """
            using Funcky.Extensions;
            using Funcky.Monads;

            public static class C
            {
                public static void M(bool flag, Option<int> option)
                {
                    Use(Option.FromBoolean(flag, () => option));
                }

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;

        var test = CreateTest(inputCode, inputCode);
        var expectedDiagnostic = DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(8, 13, 8, 51);
        test.ExpectedDiagnostics.Add(expectedDiagnostic);
        test.FixedState.ExpectedDiagnostics.Add(expectedDiagnostic);
        await test.RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task SpellsOutTheTypeOfADefaultLiteralCondition()
    {
        const string inputCode =
            """
            using Funcky.Extensions;
            using Funcky.Monads;

            public static class C
            {
                public static void M(int number)
                {
                    Use(Option.FromBoolean(default, number));
                }

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;
        const string fixedCode =
            """
            using Funcky.Extensions;
            using Funcky.Monads;

            public static class C
            {
                public static void M(int number)
                {
                    Use(default(bool).Then(number));
                }

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;

        var test = CreateTest(inputCode, fixedCode);
        test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(8, 13, 8, 48));
        await test.RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task ReordersNamedArgumentsOnlyWhenTheyAreFreeOfSideEffects()
    {
        const string inputCode =
            """
            using Funcky.Extensions;
            using Funcky.Monads;

            public static class C
            {
                public static void M(bool flag, int number, string text)
                {
                    var n = 0;
                    Use(Option.FromBoolean(item: number + 1, boolean: !flag));
                    Use(Option.FromBoolean(selector: () => text, boolean: flag));
                    Use(Option.FromBoolean(item: ++n, boolean: n == 1));
                    Use(Option.FromBoolean(item: Next(), boolean: flag));
                    Use(Option.FromBoolean(item: Next(), boolean: Next() == 1));
                    Use(Option.FromBoolean(boolean: Next() == 1, item: Next()));
                }

                private static int Next() => 1;

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;
        const string fixedCode =
            """
            using Funcky.Extensions;
            using Funcky.Monads;

            public static class C
            {
                public static void M(bool flag, int number, string text)
                {
                    var n = 0;
                    Use((!flag).Then(number + 1));
                    Use(flag.Then(() => text));
                    Use(Option.FromBoolean(item: ++n, boolean: n == 1));
                    Use(Option.FromBoolean(item: Next(), boolean: flag));
                    Use(Option.FromBoolean(item: Next(), boolean: Next() == 1));
                    Use((Next() == 1).Then(Next()));
                }

                private static int Next() => 1;

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;

        var test = CreateTest(inputCode, fixedCode);
        test.ExpectedDiagnostics.AddRange(
        [
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(9, 13, 9, 65),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(10, 13, 10, 68),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(11, 13, 11, 59),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(12, 13, 12, 60),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(13, 13, 13, 67),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(14, 13, 14, 67),
        ]);
        test.FixedState.ExpectedDiagnostics.AddRange(
        [
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(11, 13, 11, 59),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(12, 13, 12, 60),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(13, 13, 13, 67),
        ]);
        await test.RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task UnwrapsTheExplicitTypeArgumentWhenFlattening()
    {
        const string inputCode =
            """
            using Funcky.Extensions;
            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(bool flag, Option<int> option)
                {
                    Option<int> a = Option.FromBoolean<Option<int>>(flag, option).SelectMany(Identity);
                    Option<int> b = Option.FromBoolean<Option<int>>(flag, () => option).SelectMany(Identity);
                    Option<Option<int>> c = Option.FromBoolean<Option<int>>(flag, option);
                }
            }
            """;
        const string fixedCode =
            """
            using Funcky.Extensions;
            using Funcky.Monads;
            using static Funcky.Functional;

            public static class C
            {
                public static void M(bool flag, Option<int> option)
                {
                    Option<int> a = flag.Then<int>(() => option);
                    Option<int> b = flag.Then<int>(() => option);
                    Option<Option<int>> c = flag.Then<Option<int>>(option);
                }
            }
            """;

        var test = CreateTest(inputCode, fixedCode);
        test.ExpectedDiagnostics.AddRange(
        [
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(9, 25, 9, 70),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(10, 25, 10, 76),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(11, 33, 11, 78),
        ]);
        await test.RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task OffersNoFlatteningFixWhenTheValueCannotMoveIntoALambda()
    {
        const string inputCode =
            """
            using System.Threading.Tasks;
            using Funcky.Extensions;
            using Funcky.Monads;
            using static Funcky.Functional;

            public struct S
            {
                private Option<int> Field { get; }

                public async Task A(bool flag)
                {
                    Use(Option.FromBoolean(flag, await Fetch()).SelectMany(Identity));
                }

                public void M(bool flag, Option<int> option, ref Option<int> byRef)
                {
                    Use(Option.FromBoolean(flag, Field).SelectMany(Identity));
                    Use(Option.FromBoolean(flag, this.Field).SelectMany(Identity));
                    Use(Option.FromBoolean(flag, byRef).SelectMany(Identity));
                    Use(Option.FromBoolean(flag, option).SelectMany(Identity));
                }

                private static Task<Option<int>> Fetch() => Task.FromResult(Option<int>.None);

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;
        const string fixedCode =
            """
            using System.Threading.Tasks;
            using Funcky.Extensions;
            using Funcky.Monads;
            using static Funcky.Functional;

            public struct S
            {
                private Option<int> Field { get; }

                public async Task A(bool flag)
                {
                    Use(Option.FromBoolean(flag, await Fetch()).SelectMany(Identity));
                }

                public void M(bool flag, Option<int> option, ref Option<int> byRef)
                {
                    Use(Option.FromBoolean(flag, Field).SelectMany(Identity));
                    Use(Option.FromBoolean(flag, this.Field).SelectMany(Identity));
                    Use(Option.FromBoolean(flag, byRef).SelectMany(Identity));
                    Use(flag.Then(() => option));
                }

                private static Task<Option<int>> Fetch() => Task.FromResult(Option<int>.None);

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;

        var test = CreateTest(inputCode, fixedCode);
        test.ExpectedDiagnostics.AddRange(
        [
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(12, 13, 12, 52),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(17, 13, 17, 44),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(18, 13, 18, 49),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(19, 13, 19, 44),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(20, 13, 20, 45),
        ]);
        test.FixedState.ExpectedDiagnostics.AddRange(
        [
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(12, 13, 12, 52),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(17, 13, 17, 44),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(18, 13, 18, 49),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(19, 13, 19, 44),
        ]);
        await test.RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task NamesTheValueParameterWhenTheValueIsADelegate()
    {
        const string inputCode =
            """
            using System;
            using Funcky.Extensions;
            using Funcky.Monads;

            public static class C
            {
                public static void M(bool flag, Func<int> factory)
                {
                    Option<Func<int>> a = Option.FromBoolean(flag, item: factory);
                    Option<Func<int>> b = Option.FromBoolean<Func<int>>(flag, factory);
                    Option<Func<int>> c = Option.FromBoolean<Func<int>>(flag, () => 1);
                    Option<int> d = Option.FromBoolean(flag, factory);
                }
            }
            """;
        const string fixedCode =
            """
            using System;
            using Funcky.Extensions;
            using Funcky.Monads;

            public static class C
            {
                public static void M(bool flag, Func<int> factory)
                {
                    Option<Func<int>> a = flag.Then(value: factory);
                    Option<Func<int>> b = flag.Then<Func<int>>(value: factory);
                    Option<Func<int>> c = flag.Then<Func<int>>(value: () => 1);
                    Option<int> d = flag.Then(factory);
                }
            }
            """;

        var test = CreateTest(inputCode, fixedCode);
        test.ExpectedDiagnostics.AddRange(
        [
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(9, 31, 9, 70),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(10, 31, 10, 75),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(11, 31, 11, 75),
            DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(12, 25, 12, 58),
        ]);
        await test.RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task OffersNoFixWhenThenWouldBindToAnotherExtension()
    {
        const string inputCode =
            """
            using Funcky.Monads;
            using Other;

            public static class C
            {
                public static void M(bool flag, int number)
                {
                    Use(Option.FromBoolean(flag, number));
                }

                private static void Use<T>(Option<T> option) where T : notnull { }
            }

            namespace Other
            {
                public static class OtherExtensions
                {
                    public static Option<T> Then<T>(this bool condition, T value) where T : notnull => default;
                }
            }
            """;

        var test = CreateTest(inputCode, inputCode);
        var expectedDiagnostic = DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(8, 13, 8, 45);
        test.ExpectedDiagnostics.Add(expectedDiagnostic);
        test.FixedState.ExpectedDiagnostics.Add(expectedDiagnostic);
        await test.RunAsync(CancellationToken.None);
    }

    [Fact]
    public async Task OffersNoFixWhenAnExtensionInTheContainingNamespaceShadowsThen()
    {
        const string inputCode =
            """
            using Funcky.Extensions;
            using Funcky.Monads;

            namespace Consumer
            {
                public static class C
                {
                    public static void M(bool flag, int number)
                    {
                        Use(Option.FromBoolean(flag, number));
                    }

                    private static void Use<T>(Option<T> option) where T : notnull { }
                }

                public static class ConsumerExtensions
                {
                    public static Option<T> Then<T>(this bool condition, T value) where T : notnull => default;
                }
            }
            """;

        var test = CreateTest(inputCode, inputCode);
        var expectedDiagnostic = DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(10, 17, 10, 49);
        test.ExpectedDiagnostics.Add(expectedDiagnostic);
        test.FixedState.ExpectedDiagnostics.Add(expectedDiagnostic);
        await test.RunAsync(CancellationToken.None);
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
