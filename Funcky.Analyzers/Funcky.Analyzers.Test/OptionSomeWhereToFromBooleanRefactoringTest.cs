using Xunit;

namespace Funcky.Analyzers.Test;

public partial class OptionSomeWhereToFromBooleanRefactoringTest
{
    [Fact]
    public async Task DoesNotSuggestRefactoringWhenOptionFromBooleanIsMissing()
    {
        const string source = "Option<int> b = Option.Return(10).[||]Where(_ => true);";
        await VerifyRefactoring(
            source,
            source,
            OptionCodeWithoutFromBoolean);
    }

    [Fact]
    public async Task DoesNotSuggestRefactoringWhenPredicateUsesBlockBody()
    {
        const string source = "Option<int> b = Option.Return(10).[||]Where(_ => { return true; });";
        await VerifyRefactoring(
            source,
            source,
            OptionCode);
    }

    [Theory]
    [InlineData("Return")]
    [InlineData("Some")]
    public async Task WorksWithRegularReturn(string returnMethodName)
    {
        await VerifyRefactoring(
            $"Option<int> x = Option.{returnMethodName}(10).[||]Where(_ => true);",
            "Option<int> x = Option.FromBoolean(true, 10);",
            OptionCode);
    }

    [Fact]
    public async Task WorksWithGenericTypeSpecified()
    {
        await VerifyRefactoring(
            "Option<IEnumerable<char>> x = Option.Return<IEnumerable<char>>(\"foo\").[||]Where(_ => true);",
            "Option<IEnumerable<char>> x = Option.FromBoolean<IEnumerable<char>>(true, \"foo\");",
            OptionCode,
            usings: Sequence.Return("using Funcky.Monads;", "using System.Collections.Generic;"));
    }

    [Fact]
    public async Task WorksWithMethodGroup()
    {
        await VerifyRefactoring(
            "Option<int> x = Option.Return(10).[||]Where(Predicate);",
            "Option<int> x = Option.FromBoolean(Predicate(10), 10);",
            OptionCode);
    }

    [Fact]
    public async Task WorksWithLambdaPredicateDependentOnParameter()
    {
        await VerifyRefactoring(
            "var x = Option.Return(\"foo\").[||]Where(v => v.Length == 3);",
            "var x = Option.FromBoolean(\"foo\".Length == 3, \"foo\");",
            OptionCode);
    }

    [Fact]
    public async Task WorksWithVariableInReturnAndLambdaPredicateDependentOnParameter()
    {
        await VerifyRefactoring(
            "var input = \"foo\"; var x = Option.Return(input).[||]Where(v => v.Length == 3);",
            "var input = \"foo\"; var x = Option.FromBoolean(input.Length == 3, input);",
            OptionCode);
    }

    [Fact]
    public async Task WorksWhenReturnMethodIsStaticallyImported()
    {
        await VerifyRefactoring(
            "var x = Return(10).[||]Where(_ => true);",
            "var x = FromBoolean(true, 10);",
            OptionCode,
            usings: Sequence.Return("using static Funcky.Monads.Option;"));
    }

    [Fact]
    public async Task WorksWhenReturnMethodIsFullyQualified()
    {
        await VerifyRefactoring(
            "var x = Funcky.Monads.Option.Return(10).[||]Where(_ => true);",
            "var x = Funcky.Monads.Option.FromBoolean(true, 10);",
            OptionCode,
            usings: Enumerable.Empty<string>());
    }

    [Fact]
    public async Task PreservesLeadingTrivia()
    {
        // TODO: find out why we get a newline in the leading trivia
        await VerifyRefactoring(
            "/* leading */ Option.Some(10).[||]Where(_ => true);",
            $"/* leading */{Environment.NewLine}Option.FromBoolean(true, 10);",
            OptionCode);
    }
}
