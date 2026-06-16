using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.SequenceReturnCollectionExpressionAnalyzer, Funcky.Analyzers.SequenceReturnCollectionExpressionCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed class SequenceReturnCollectionExpressionTest
{
    [Fact]
    public async Task SuggestsCollectionExpressionForExplicitlyTypedLocal()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                IReadOnlyList<int> M()
                {
                    IReadOnlyList<int> result = {|λ1011:Sequence.Return(0)|};
                    return result;
                }
            }
            """);

        var fixedSource = WithSequenceStub(
            """
            class C
            {
                IReadOnlyList<int> M()
                {
                    IReadOnlyList<int> result = [0];
                    return result;
                }
            }
            """);

        await VerifyCS.VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public async Task SuggestsCollectionExpressionWhenTargetTypeIsIEnumerable()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                IEnumerable<int> M() => {|λ1011:Sequence.Return(0)|};
            }
            """);

        var fixedSource = WithSequenceStub(
            """
            class C
            {
                IEnumerable<int> M() => [0];
            }
            """);

        await VerifyCS.VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public async Task SuggestsCollectionExpressionInArgumentPosition()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                void Consume(IReadOnlyList<int> values) { }

                void M() => Consume({|λ1011:Sequence.Return(0)|});
            }
            """);

        var fixedSource = WithSequenceStub(
            """
            class C
            {
                void Consume(IReadOnlyList<int> values) { }

                void M() => Consume([0]);
            }
            """);

        await VerifyCS.VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public async Task CodeFixKeepsACompoundElementExpression()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                IReadOnlyList<int> M(int x) => {|λ1011:Sequence.Return(x + 1)|};
            }
            """);

        var fixedSource = WithSequenceStub(
            """
            class C
            {
                IReadOnlyList<int> M(int x) => [x + 1];
            }
            """);

        await VerifyCS.VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public async Task DoesNotSuggestForImplicitlyTypedLocal()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                void M()
                {
                    var result = Sequence.Return(0);
                }
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DoesNotSuggestWhenTargetTypeIsNotACollection()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                void M()
                {
                    object result = Sequence.Return(0);
                }
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DoesNotSuggestForParamsOverloadWithMultipleElements()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                IReadOnlyList<int> M() => Sequence.Return(0, 1, 2);
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DoesNotSuggestWhenLanguageVersionIsBelowCSharp12()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                IReadOnlyList<int> M()
                {
                    IReadOnlyList<int> result = Sequence.Return(0);
                    return result;
                }
            }
            """);

        var test = new VerifyCS.Test { TestCode = source };
        test.SolutionTransforms.Add((solution, projectId) =>
        {
            var parseOptions = (CSharpParseOptions)solution.GetProject(projectId)!.ParseOptions!;
            return solution.WithProjectParseOptions(projectId, parseOptions.WithLanguageVersion(LanguageVersion.CSharp11));
        });

        await test.RunAsync(CancellationToken.None);
    }

    private static string WithSequenceStub(string code)
        => $$"""
            using System.Collections.Generic;
            using Funcky;

            namespace Funcky
            {
                public static class Sequence
                {
                    public static IReadOnlyList<TResult> Return<TResult>(TResult element) => new[] { element };

                    public static IReadOnlyList<TResult> Return<TResult>(params TResult[] elements) => elements;
                }
            }

            {{code}}
            """;
}
