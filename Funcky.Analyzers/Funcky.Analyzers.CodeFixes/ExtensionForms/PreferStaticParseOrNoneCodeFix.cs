using System.Collections.Immutable;
using System.Composition;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Funcky.Analyzers.ExtensionForms;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Simplification;

namespace Funcky.Analyzers;

[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(PreferStaticParseOrNoneCodeFix))]
public sealed class PreferStaticParseOrNoneCodeFix : ExtensionMemberCodeFix
{
    private const string GlobalAliasPrefix = "global::";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(PreferStaticParseOrNoneAnalyzer.PreferStaticParseOrNone.Id);

    // The replacement calls `global::TargetType.StaticMember`.
    protected override string GetTitle(ExtensionMemberRewrite rewrite)
        => $"Replace with {((InvocationExpressionSyntax)rewrite.Replacement).Expression.ToString().Replace(GlobalAliasPrefix, string.Empty)}";

    protected override ExtensionMemberRewrite? TryCreateRewrite(SyntaxNode root, SemanticModel semanticModel, Diagnostic diagnostic, CancellationToken cancellationToken)
        => root.FindInvocationExpression(diagnostic.Location.SourceSpan) is { } invocation
           && invocation.Span == diagnostic.Location.SourceSpan
           && diagnostic.Properties.TryGetValue(PreferStaticParseOrNoneAnalyzer.TargetTypeProperty, out var targetType)
           && diagnostic.Properties.TryGetValue(PreferStaticParseOrNoneAnalyzer.StaticMemberNameProperty, out var staticMemberName)
           && targetType is not null
           && staticMemberName is not null
           && semanticModel.GetOperation(invocation, cancellationToken) is IInvocationOperation operation
           && PreferStaticParseOrNoneAnalyzer.StaticInvocation(operation, targetType, staticMemberName) is { } replacement
            ? new ExtensionMemberRewrite(invocation, replacement.WithExpression(replacement.Expression.WithAdditionalAnnotations(Simplifier.Annotation)).WithTriviaFrom(invocation))
            : null;
}
