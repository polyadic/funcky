#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
#pragma warning disable SA1118 // StyleCop support for collection expressions is missing
using Xunit;

namespace Funcky.Analyzers.Test;

public partial class OptionSomeWhereToThenRefactoringTest
{
    [Fact]
    public async Task DoesNotSuggestRefactoringWhenThenIsMissing()
    {
        const string source = "Option<int> b = Option.Return(10).[||]Where(_ => true);";
        await VerifyRefactoring(
            source,
            source,
            OptionCodeWithoutThen,
            usings: ["using Funcky.Monads;"]);
    }

    [Fact]
    public async Task DoesNotSuggestRefactoringWhenPredicateUsesBlockBody()
    {
        const string source = "Option<int> b = Option.Return(10).[||]Where(_ => { var x = 1; return true; });";
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
            "Option<int> x = true.Then(10);",
            OptionCode);
    }

    [Fact]
    public async Task WorksWithGenericTypeSpecified()
    {
        await VerifyRefactoring(
            "Option<IEnumerable<char>> x = Option.Return<IEnumerable<char>>(\"foo\").[||]Where(_ => true);",
            "Option<IEnumerable<char>> x = true.Then<IEnumerable<char>>(\"foo\");",
            OptionCode,
            usings: ["using Funcky.Extensions;", "using Funcky.Monads;", "using System.Collections.Generic;"]);
    }

    [Fact]
    public async Task WorksWithMethodGroup()
    {
        await VerifyRefactoring(
            "Option<int> x = Option.Return(10).[||]Where(Predicate);",
            "Option<int> x = Predicate(10).Then(10);",
            OptionCode);
    }

    [Fact]
    public async Task WorksWhenLambdaUsesBlockBodyWithSingleReturnStatement()
    {
        await VerifyRefactoring(
            "Option<int> x = Option.Return(10).[||]Where(x => { return x == 10; });",
            "Option<int> x = (10 == 10).Then(10);",
            OptionCode);
    }

    [Fact]
    public async Task WorksWithLambdaPredicateDependentOnParameter()
    {
        await VerifyRefactoring(
            "var x = Option.Return(\"foo\").[||]Where(v => v.Length == 3);",
            "var x = (\"foo\".Length == 3).Then(\"foo\");",
            OptionCode);
    }

    [Fact]
    public async Task WorksWithVariableInReturnAndLambdaPredicateDependentOnParameter()
    {
        await VerifyRefactoring(
            "var input = \"foo\"; var x = Option.Return(input).[||]Where(v => v.Length == 3);",
            "var input = \"foo\"; var x = (input.Length == 3).Then(input);",
            OptionCode);
    }

    [Fact]
    public async Task WorksWhenReturnMethodIsStaticallyImported()
    {
        await VerifyRefactoring(
            "var x = Return(10).[||]Where(_ => true);",
            "var x = true.Then(10);",
            OptionCode,
            usings: ["using Funcky.Extensions;", "using static Funcky.Monads.Option;"]);
    }

    [Fact]
    public async Task WorksWhenReturnMethodIsFullyQualified()
    {
        await VerifyRefactoring(
            "var x = Funcky.Monads.Option.Return(10).[||]Where(_ => true);",
            "var x = true.Then(10);",
            OptionCode,
            usings: ["using Funcky.Extensions;"]);
    }

    [Fact]
    public async Task AddsTheUsingForTheExtensionWhenMissing()
    {
        await VerifyRefactoring(
            "var x = Option.Return(10).[||]Where(_ => true);",
            "var x = true.Then(10);",
            OptionCode,
            usings: ["using Funcky.Monads;"],
            fixedUsings: ["using Funcky.Monads;", "using Funcky.Extensions;"]);
    }

    [Fact]
    public async Task PreservesLeadingTrivia()
    {
        await VerifyRefactoring(
            "/* leading */ Option.Some(10).[||]Where(_ => true);",
            "/* leading */ true.Then(10);",
            OptionCode);
    }
}
