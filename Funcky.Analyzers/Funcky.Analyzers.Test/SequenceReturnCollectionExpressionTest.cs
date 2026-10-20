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
                    IReadOnlyList<int> result = {|λ1013:Sequence.Return(0)|};
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
                IEnumerable<int> M() => {|λ1013:Sequence.Return(0)|};
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

                void M() => Consume({|λ1013:Sequence.Return(0)|});
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
                IReadOnlyList<int> M(int x) => {|λ1013:Sequence.Return(x + 1)|};
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
    public async Task CodeFixParenthesizesARangeWithoutLeftOperandSoItIsNotASpread()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                IReadOnlyList<System.Range> M() => {|λ1013:Sequence.Return(..5)|};
            }
            """);

        var fixedSource = WithSequenceStub(
            """
            class C
            {
                IReadOnlyList<System.Range> M() => [(..5)];
            }
            """);

        await VerifyCS.VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public async Task CodeFixKeepsCommentsInsideTheInvocation()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                IReadOnlyList<int> M() => {|λ1013:Sequence.Return(/* explanation */ 0 /* trailing */)|};
            }
            """);

        var fixedSource = WithSequenceStub(
            """
            class C
            {
                IReadOnlyList<int> M() => [/* explanation */ 0 /* trailing */];
            }
            """);

        await VerifyCS.VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public async Task CodeFixKeepsTheLineBreakAfterALineComment()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                IReadOnlyList<int> M() => {|λ1013:Sequence.Return(0 // explanation
                )|};
            }
            """);

        var fixedSource = WithSequenceStub(
            """
            class C
            {
                IReadOnlyList<int> M() => [0 // explanation
                ];
            }
            """);

        await VerifyCS.VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public async Task SuggestsCollectionExpressionWhenAssignedToAVariableNamedUnderscore()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                void M()
                {
                    IReadOnlyList<int> _;
                    _ = {|λ1013:Sequence.Return(0)|};
                }
            }
            """);

        var fixedSource = WithSequenceStub(
            """
            class C
            {
                void M()
                {
                    IReadOnlyList<int> _;
                    _ = [0];
                }
            }
            """);

        await VerifyCS.VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public async Task SuggestsCollectionExpressionInReturnOfATargetTypedLambda()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                void M()
                {
                    System.Func<IEnumerable<int>> f = () => { return {|λ1013:Sequence.Return(0)|}; };
                }
            }
            """);

        var fixedSource = WithSequenceStub(
            """
            class C
            {
                void M()
                {
                    System.Func<IEnumerable<int>> f = () => { return [0]; };
                }
            }
            """);

        await VerifyCS.VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public async Task SuggestsCollectionExpressionWhenTheElementTypeIsInferredFromTheCollection()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                void Consume<T>(IEnumerable<T> values) { }

                void M() => Consume({|λ1013:Sequence.Return(0)|});
            }
            """);

        var fixedSource = WithSequenceStub(
            """
            class C
            {
                void Consume<T>(IEnumerable<T> values) { }

                void M() => Consume([0]);
            }
            """);

        await VerifyCS.VerifyCodeFixAsync(source, fixedSource);
    }

    [Fact]
    public async Task DoesNotSuggestWhenAssignedToADiscard()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                void M()
                {
                    _ = Sequence.Return(0);
                }
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DoesNotSuggestInsideAnExpressionTree()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                void Consume(IReadOnlyList<int> values) { }

                void M()
                {
                    System.Linq.Expressions.Expression<System.Action> e = () => Consume(Sequence.Return(0));
                }
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DoesNotSuggestWhenTheTargetElementTypeDiffersFromTheReturnedElementType()
    {
        var source = WithSequenceStub(
            """
            class Widget { }

            class C
            {
                void M()
                {
                    IEnumerable<object> widgets = Sequence.Return(new Widget());
                    IEnumerable<object> names = Sequence.Return("tag");
                }
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DoesNotSuggestWhenTheTypeParameterCouldNotBeInferredFromACollectionExpression()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                void Consume<T>(T value) { }

                void M() => Consume(Sequence.Return(0));
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DoesNotSuggestWhenACollectionExpressionWouldPickAnotherOverload()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                void Consume(IEnumerable<int> values) { }

                void Consume(int[] values) { }

                void M() => Consume(Sequence.Return(0));
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DoesNotSuggestInReturnOfALambdaWithInferredType()
    {
        var source = WithSequenceStub(
            """
            class C
            {
                void M()
                {
                    var f = () => { return Sequence.Return(0); };
                }
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
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
