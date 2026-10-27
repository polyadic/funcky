#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
using Funcky.Extensions;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeRefactoringVerifier<Funcky.Analyzers.OptionSomeWhereToThenRefactoring>;

namespace Funcky.Analyzers.Test;

public partial class OptionSomeWhereToThenRefactoringTest
{
    private const string OptionCode = """
        namespace Funcky.Monads
        {
            using System;

            public readonly struct Option<TItem>
            {
                public Option<TItem> Where(Func<TItem, bool> predicate) => default;
            }

            public static class Option
            {
                public static Option<TItem> Return<TItem>(TItem value) => default;

                public static Option<TItem> Some<TItem>(TItem value) => default;
            }
        }

        namespace Funcky.Extensions
        {
            using System;
            using Funcky.Monads;

            public static class BooleanExtensions
            {
                public static Option<TItem> Then<TItem>(this bool condition, TItem value) => default;

                public static Option<TItem> Then<TItem>(this bool condition, Func<TItem> selector) => default;

                public static Option<TItem> Then<TItem>(this bool condition, Func<Option<TItem>> selector) => default;
            }
        }

        namespace Other
        {
            using Funcky.Monads;

            public static class OtherExtensions
            {
                public static Option<TItem> Then<TItem>(this bool condition, TItem value) => default;
            }
        }
        """;

    private const string OptionCodeWithoutThen = """
        namespace Funcky.Monads
        {
            using System;

            public readonly struct Option<TItem>
            {
                public Option<TItem> Where(Func<TItem, bool> predicate) => default;
            }

            public static class Option
            {
                public static Option<TItem> Return<TItem>(TItem value) => default;

                public static Option<TItem> Some<TItem>(TItem value) => default;
            }
        }
        """;

    private static readonly IEnumerable<string> DefaultUsings = ["using Funcky.Extensions;", "using Funcky.Monads;"];

    private static async Task VerifyRefactoring(string source, string fixedSource, string supportSource, IEnumerable<string>? usings = null, IEnumerable<string>? fixedUsings = null)
    {
        usings ??= DefaultUsings;
        await VerifyCS.VerifyRefactoringAsync(
            WrapInMethod(source, usings).ReplaceLineEndings() + Environment.NewLine + supportSource,
            WrapInMethod(fixedSource, fixedUsings ?? usings).ReplaceLineEndings() + Environment.NewLine + supportSource);
    }

    private static string WrapInMethod(string statement, IEnumerable<string>? usings)
        => $$"""
            {{usings?.JoinToString(Environment.NewLine) ?? string.Empty}}
            public static class C
            {
                public static void M()
                {
            {{Indent(statement, 8)}}
                }

                public static bool Predicate<TItem>(TItem item) => false;
            }
            """;

    private static string Indent(string input, int indentation)
        => input.SplitLines().Select(l => new string(' ', indentation) + l).JoinToString(Environment.NewLine);
}
