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
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferXunitAssertCodeFix))]
public sealed class PreferXunitAssertCodeFix : ExtensionMemberCodeFix
{
    private const string AssertTypeName = "global::Xunit.Assert";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(PreferXunitAssertAnalyzer.PreferAssert.Id);

    protected override string GetTitle(ExtensionMemberRewrite rewrite) => "Replace with the assertion on Assert";

    protected override ExtensionMemberRewrite? TryCreateRewrite(SyntaxNode root, SemanticModel semanticModel, Diagnostic diagnostic, CancellationToken cancellationToken)
        => root.FindInvocationExpression(diagnostic.Location.SourceSpan) is { } invocation
           && invocation.Span == diagnostic.Location.SourceSpan
           && WithAssertReceiver(invocation) is { } replacement
            ? new ExtensionMemberRewrite(invocation, replacement)
            : null;

    // `FunctionalAssert.Some(x)` and `Funcky.FunctionalAssert.Some(x)` keep their name (and type arguments); `Some(x)` through
    // a `using static` gets the Assert receiver in front.
    private static InvocationExpressionSyntax? WithAssertReceiver(InvocationExpressionSyntax invocation)
        => invocation.Expression switch
        {
            MemberAccessExpressionSyntax memberAccess => invocation.WithExpression(memberAccess.WithExpression(Assert().WithTriviaFrom(memberAccess.Expression))),
            SimpleNameSyntax name => invocation.WithExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, Assert().WithLeadingTrivia(name.GetLeadingTrivia()), name.WithoutLeadingTrivia())),
            _ => null,
        };

    private static ExpressionSyntax Assert()
        => ParseExpression(AssertTypeName).WithAdditionalAnnotations(Simplifier.Annotation);
}
