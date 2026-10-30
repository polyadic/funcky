using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using VerifyCS = Funcky.Analyzers.Test.CSharpCodeFixVerifier<Funcky.Analyzers.SequenceReturnCollectionExpressionAnalyzer, Funcky.Analyzers.SequenceReturnCollectionExpressionCodeFix>;

namespace Funcky.Analyzers.Test;

public sealed class SequenceReturnCollectionExpressionTest
{
    [Fact]
    public async Task SuggestsCollectionExpressionForExplicitlyTypedLocal()
    {
        var source = WithEnumerableStub(
            """
            class C
            {
                IReadOnlyList<int> M()
                {
                    IReadOnlyList<int> result = {|λ1013:Enumerable.Return(0)|};
                    return result;
                }
            }
            """);

        var fixedSource = WithEnumerableStub(
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
        var source = WithEnumerableStub(
            """
            class C
            {
                IEnumerable<int> M() => {|λ1013:Enumerable.Return(0)|};
            }
            """);

        var fixedSource = WithEnumerableStub(
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
        var source = WithEnumerableStub(
            """
            class C
            {
                void Consume(IReadOnlyList<int> values) { }

                void M() => Consume({|λ1013:Enumerable.Return(0)|});
            }
            """);

        var fixedSource = WithEnumerableStub(
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
        var source = WithEnumerableStub(
            """
            class C
            {
                IReadOnlyList<int> M(int x) => {|λ1013:Enumerable.Return(x + 1)|};
            }
            """);

        var fixedSource = WithEnumerableStub(
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
        var source = WithEnumerableStub(
            """
            class C
            {
                IReadOnlyList<System.Range> M() => {|λ1013:Enumerable.Return(..5)|};
            }
            """);

        var fixedSource = WithEnumerableStub(
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
        var source = WithEnumerableStub(
            """
            class C
            {
                IReadOnlyList<int> M() => {|λ1013:Enumerable.Return(/* explanation */ 0 /* trailing */)|};
            }
            """);

        var fixedSource = WithEnumerableStub(
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
        var source = WithEnumerableStub(
            """
            class C
            {
                IReadOnlyList<int> M() => {|λ1013:Enumerable.Return(0 // explanation
                )|};
            }
            """);

        var fixedSource = WithEnumerableStub(
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
        var source = WithEnumerableStub(
            """
            class C
            {
                void M()
                {
                    IReadOnlyList<int> _;
                    _ = {|λ1013:Enumerable.Return(0)|};
                }
            }
            """);

        var fixedSource = WithEnumerableStub(
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
        var source = WithEnumerableStub(
            """
            class C
            {
                void M()
                {
                    System.Func<IEnumerable<int>> f = () => { return {|λ1013:Enumerable.Return(0)|}; };
                }
            }
            """);

        var fixedSource = WithEnumerableStub(
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
        var source = WithEnumerableStub(
            """
            class C
            {
                void Consume<T>(IEnumerable<T> values) { }

                void M() => Consume({|λ1013:Enumerable.Return(0)|});
            }
            """);

        var fixedSource = WithEnumerableStub(
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
        var source = WithEnumerableStub(
            """
            class C
            {
                void M()
                {
                    _ = Enumerable.Return(0);
                }
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DoesNotSuggestInsideAnExpressionTree()
    {
        var source = WithEnumerableStub(
            """
            class C
            {
                void Consume(IReadOnlyList<int> values) { }

                void M()
                {
                    System.Linq.Expressions.Expression<System.Action> e = () => Consume(Enumerable.Return(0));
                }
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DoesNotSuggestWhenTheTargetElementTypeDiffersFromTheReturnedElementType()
    {
        var source = WithEnumerableStub(
            """
            class Widget { }

            class C
            {
                void M()
                {
                    IEnumerable<object> widgets = Enumerable.Return(new Widget());
                    IEnumerable<object> names = Enumerable.Return("tag");
                }
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DoesNotSuggestWhenTheTypeParameterCouldNotBeInferredFromACollectionExpression()
    {
        var source = WithEnumerableStub(
            """
            class C
            {
                void Consume<T>(T value) { }

                void M() => Consume(Enumerable.Return(0));
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DoesNotSuggestWhenACollectionExpressionWouldPickAnotherOverload()
    {
        var source = WithEnumerableStub(
            """
            class C
            {
                void Consume(IEnumerable<int> values) { }

                void Consume(int[] values) { }

                void M() => Consume(Enumerable.Return(0));
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DoesNotSuggestInReturnOfALambdaWithInferredType()
    {
        var source = WithEnumerableStub(
            """
            class C
            {
                void M()
                {
                    var f = () => { return Enumerable.Return(0); };
                }
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DoesNotSuggestForImplicitlyTypedLocal()
    {
        var source = WithEnumerableStub(
            """
            class C
            {
                void M()
                {
                    var result = Enumerable.Return(0);
                }
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DoesNotSuggestWhenTargetTypeIsNotACollection()
    {
        var source = WithEnumerableStub(
            """
            class C
            {
                void M()
                {
                    object result = Enumerable.Return(0);
                }
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DoesNotSuggestForParamsOverloadWithMultipleElements()
    {
        var source = WithEnumerableStub(
            """
            class C
            {
                IReadOnlyList<int> M() => Enumerable.Return(0, 1, 2);
            }
            """);

        await VerifyCS.VerifyAnalyzerAsync(source);
    }

    [Fact]
    public async Task DoesNotSuggestWhenLanguageVersionIsBelowCSharp12()
    {
        var source = WithClassicStub(
            """
            class C
            {
                IReadOnlyList<int> M()
                {
                    IReadOnlyList<int> result = EnumerableExtensions.Return(0);
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

    // Enumerable.Return is a static extension member on System.Linq.Enumerable, declared in Funcky.Extensions.EnumerableExtensions.
    private static string WithEnumerableStub(string code)
        => $$"""
            using System.Collections.Generic;
            using System.Linq;
            using Funcky.Extensions;

            namespace Funcky.Extensions
            {
                public static class EnumerableExtensions
                {
                    extension(Enumerable)
                    {
                        public static IReadOnlyList<TResult> Return<TResult>(TResult element) => new[] { element };

                        public static IReadOnlyList<TResult> Return<TResult>(params TResult[] elements) => elements;
                    }
                }
            }

            {{code}}
            """;

    // Extension blocks need C# 14, so the test for older language versions uses the lowered form of the same member.
    private static string WithClassicStub(string code)
        => $$"""
            using System.Collections.Generic;
            using Funcky.Extensions;

            namespace Funcky.Extensions
            {
                public static class EnumerableExtensions
                {
                    public static IReadOnlyList<TResult> Return<TResult>(TResult element) => new[] { element };
                }
            }

            {{code}}
            """;
}
