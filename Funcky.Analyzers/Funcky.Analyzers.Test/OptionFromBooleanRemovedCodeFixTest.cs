#pragma warning disable SA1118 // StyleCop support for collection expressions is missing
using Microsoft.CodeAnalysis.Testing;
using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Microsoft.CodeAnalysis.Testing.EmptyDiagnosticAnalyzer, Funcky.Analyzers.OptionFromBooleanCodeFix>;

namespace Funcky.Analyzers.Test;

/// <summary>The fix on the compile error CS0117 that a call of <c>Option.FromBoolean</c> reports once the method is gone (Funcky 4.0.0).
/// The compiler resolves no overload there, so the fix has to tell the overloads apart from the arguments.</summary>
public sealed class OptionFromBooleanRemovedCodeFixTest
{
    private const string MissingMember = "CS0117";

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
    public async Task RewritesTheRemovedCallToThen()
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
                    Use(Option.{|CS0117:FromBoolean|}(flag, number));
                    Use(Option.{|CS0117:FromBoolean|}(number % 2 == 0, number));
                    Use(Option.{|CS0117:FromBoolean|}(!flag, () => text));
                    Use(Option.{|CS0117:FromBoolean<object>|}(flag, text));
                    Use(Option.{|CS0117:FromBoolean|}(flag));
                    Use(Option.{|CS0117:FromBoolean|}(item: number, boolean: flag));
                    Use(Option.{|CS0117:FromBoolean|}(flag, Next));
                    Use(Option.{|CS0117:FromBoolean|}(flag, () => { return number; }));
                    Use(Funcky.Monads.Option.{|CS0117:FromBoolean|}(flag, number));
                }

                private static int Next() => 1;

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
                    Use(flag.Then(Next));
                    Use(flag.Then(() => { return number; }));
                    Use(flag.Then(number));
                }

                private static int Next() => 1;

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;

        await CreateTest(inputCode, fixedCode).RunAsync(CancellationToken.None);
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
                    Use(Option.{|CS0117:FromBoolean|}(flag, option).SelectMany(Identity));
                    Use(Option.{|CS0117:FromBoolean|}(flag, () => option).SelectMany(Identity));
                    Option<int> a = Option.{|CS0117:FromBoolean<Option<int>>|}(flag, option).SelectMany(Identity);
                    Option<Option<int>> c = Option.{|CS0117:FromBoolean<Option<int>>|}(flag, option);
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
                    Option<int> a = flag.Then<int>(() => option);
                    Option<Option<int>> c = flag.Then<Option<int>>(option);
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
                    Use(Option.{|CS0117:FromBoolean|}(flag, () => option));
                    Use(Option.{|CS0117:FromBoolean|}(flag, () => { return option; }));
                    Use(Option.{|CS0117:FromBoolean|}(flag, Fetch));
                }

                private static Option<int> Fetch() => default;

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;

        await CreateTest(inputCode, inputCode).RunAsync(CancellationToken.None);
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
                    Use(Option.{|CS0117:FromBoolean|}(flag, number));
                    Use(Option.{|CS0117:FromBoolean|}(flag));
                    Use(Option.{|CS0117:FromBoolean|}(flag, () => number));
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
                    Use(Option.{|CS0117:FromBoolean|}(item: number + 1, boolean: !flag));
                    Use(Option.{|CS0117:FromBoolean|}(selector: () => text, boolean: flag));
                    Use(Option.{|CS0117:FromBoolean|}(item: ++n, boolean: n == 1));
                    Use(Option.{|CS0117:FromBoolean|}(item: Next(), boolean: flag));
                    Use(Option.{|CS0117:FromBoolean|}(boolean: Next() == 1, item: Next()));
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
                    Use((Next() == 1).Then(Next()));
                }

                private static int Next() => 1;

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;

        var test = CreateTest(inputCode, fixedCode);
        test.FixedState.ExpectedDiagnostics.AddRange(
        [
            DiagnosticResult.CompilerError(MissingMember).WithSpan(11, 20, 11, 31),
            DiagnosticResult.CompilerError(MissingMember).WithSpan(12, 20, 12, 31),
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
                    Option<Func<int>> a = Option.{|CS0117:FromBoolean|}(flag, item: factory);
                    Option<Func<int>> b = Option.{|CS0117:FromBoolean<Func<int>>|}(flag, factory);
                    Option<Func<int>> c = Option.{|CS0117:FromBoolean<Func<int>>|}(flag, () => 1);
                    Option<int> d = Option.{|CS0117:FromBoolean|}(flag, factory);
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

        await CreateTest(inputCode, fixedCode).RunAsync(CancellationToken.None);
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
                    Use(Option.{|CS0117:FromBoolean|}(flag, await Fetch()).SelectMany(Identity));
                }

                public void M(bool flag, Option<int> option, ref Option<int> byRef)
                {
                    Use(Option.{|CS0117:FromBoolean|}(flag, Field).SelectMany(Identity));
                    Use(Option.{|CS0117:FromBoolean|}(flag, byRef).SelectMany(Identity));
                    Use(Option.{|CS0117:FromBoolean|}(flag, option).SelectMany(Identity));
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
                    Use(Option.FromBoolean(flag, byRef).SelectMany(Identity));
                    Use(flag.Then(() => option));
                }

                private static Task<Option<int>> Fetch() => Task.FromResult(Option<int>.None);

                private static void Use<T>(Option<T> option) where T : notnull { }
            }
            """;

        var test = CreateTest(inputCode, fixedCode);
        test.FixedState.ExpectedDiagnostics.AddRange(
        [
            DiagnosticResult.CompilerError(MissingMember).WithSpan(12, 20, 12, 31),
            DiagnosticResult.CompilerError(MissingMember).WithSpan(17, 20, 17, 31),
            DiagnosticResult.CompilerError(MissingMember).WithSpan(18, 20, 18, 31),
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
                    Use(Option.{|CS0117:FromBoolean|}(flag, number));
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

        await CreateTest(inputCode, inputCode).RunAsync(CancellationToken.None);
    }

    private static VerifyCS.Test CreateTest(string inputCode, string fixedCode)
        => new()
        {
            TestCode = inputCode + Environment.NewLine + StubCode,
            FixedCode = fixedCode + Environment.NewLine + StubCode,
            DisabledDiagnostics = { MissingXmlComment },
        };
}
