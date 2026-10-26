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
                    Use(Option.FromBoolean(flag, number));
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
                }

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;

        var test = CreateTest(inputCode, fixedCode);
        test.ExpectedDiagnostics.Add(DiagnosticResult.CompilerWarning(ObsoleteMember).WithSpan(7, 13, 7, 45));
        await test.RunAsync(CancellationToken.None);
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

    private static VerifyCS.Test CreateTest(string inputCode, string fixedCode)
        => new()
        {
            TestCode = inputCode + Environment.NewLine + StubCode,
            FixedCode = fixedCode + Environment.NewLine + StubCode,
            CompilerDiagnostics = CompilerDiagnostics.Warnings,
            DisabledDiagnostics = { MissingXmlComment },
        };
}
