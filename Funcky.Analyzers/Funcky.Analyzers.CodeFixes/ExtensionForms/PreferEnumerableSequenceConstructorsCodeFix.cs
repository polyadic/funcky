using System.Collections.Immutable;
using System.Composition;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Funcky.Analyzers.ExtensionForms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Simplification;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferEnumerableSequenceConstructorsCodeFix))]
public sealed class PreferEnumerableSequenceConstructorsCodeFix : ExtensionMemberCodeFix
{
    private const string EnumerableTypeName = "global::System.Linq.Enumerable";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(PreferEnumerableSequenceConstructorsAnalyzer.PreferEnumerable.Id);

    protected override string GetTitle(ExtensionMemberRewrite rewrite) => "Replace with the Enumerable form";

    protected override ExtensionMemberRewrite? TryCreateRewrite(SyntaxNode root, SemanticModel semanticModel, Diagnostic diagnostic, CancellationToken cancellationToken)
        => root.FindInvocationExpression(diagnostic.Location.SourceSpan) is { } invocation
           && invocation.Span == diagnostic.Location.SourceSpan
           && WithEnumerableReceiver(invocation.Expression) is { } replacement
            ? new ExtensionMemberRewrite(invocation, replacement)
            : null;

    // `Sequence.Return(x)` and `Funcky.Sequence.Return(x)` keep their name (and type arguments); `Return(x)` through a
    // `using static` gets the Enumerable receiver in front.
    private static InvocationExpressionSyntax? WithEnumerableReceiver(ExpressionSyntax invoked)
        => invoked switch
        {
            MemberAccessExpressionSyntax memberAccess => ((InvocationExpressionSyntax)invoked.Parent!).WithExpression(memberAccess.WithExpression(Enumerable().WithTriviaFrom(memberAccess.Expression))),
            SimpleNameSyntax name => ((InvocationExpressionSyntax)invoked.Parent!).WithExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, Enumerable().WithLeadingTrivia(name.GetLeadingTrivia()), name.WithoutLeadingTrivia())),
            _ => null,
        };

    private static ExpressionSyntax Enumerable()
        => ParseExpression(EnumerableTypeName).WithAdditionalAnnotations(Simplifier.Annotation);
}
