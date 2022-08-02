using Funcky.Extensions;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeRefactoringVerifier<Funcky.Analyzers.OptionSomeWhereToFromBooleanRefactoring>;

namespace Funcky.Analyzers.Test;

public partial class OptionSomeWhereToFromBooleanRefactoringTest
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

                public static Option<TItem> FromBoolean<TItem>(bool boolean, TItem item) => default;
            }
        }
        """;

    private const string OptionCodeWithoutFromBoolean = """
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

    private static readonly IEnumerable<string> DefaultUsings = Sequence.Return("using Funcky.Monads;");

    private static async Task VerifyRefactoring(string source, string fixedSource, string supportSource, IEnumerable<string>? usings = null)
    {
        usings ??= DefaultUsings;
        await VerifyCS.VerifyRefactoringAsync(
            WrapInMethod(source, usings).ReplaceLineEndings() + Environment.NewLine + supportSource,
            WrapInMethod(fixedSource, usings).ReplaceLineEndings() + Environment.NewLine + supportSource);
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
