using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Simplification;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers
{
    [Shared]
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddArgumentNameCodeFix))]
    public sealed class AddArgumentNameCodeFix : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(UseWithArgumentNamesAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false) is { } root
                && await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false) is { } semanticModel)
            {
                foreach (var diagnostic in context.Diagnostics)
                {
                    if (root.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<ArgumentSyntax>() is { } argumentSyntax
                        && semanticModel.GetOperation(argumentSyntax) is IArgumentOperation { Parameter: { } } argumentOperation)
                    {
                        context.RegisterCodeFix(CreateFix(context, argumentSyntax, argumentOperation.Parameter), diagnostic);
                    }
                }
            }
        }

        private static CodeAction CreateFix(CodeFixContext context, ArgumentSyntax argument, IParameterSymbol parameter)
            => CodeAction.Create(
                string.Format(CodeFixResources.AddArgumentNameCodeFixTitle, parameter.Name),
                AddArgumentLabelAsync(context.Document, argument, parameter),
                nameof(AddArgumentNameCodeFix));

        private static Func<CancellationToken, Task<Document>> AddArgumentLabelAsync(Document document, ArgumentSyntax argument, IParameterSymbol parameter)
            => async cancellationToken
                => await document.GetSyntaxRootAsync(cancellationToken) is { } syntaxRoot
                    ? document.WithSyntaxRoot(syntaxRoot.ReplaceNode(argument, AddArgumentName(argument, parameter)))
                    : document;

        private static ArgumentSyntax AddArgumentName(ArgumentSyntax argument, IParameterSymbol parameter)
            => argument
                .WithNameColon(NameColon(IdentifierName(CreateIdentifier(parameter.Name)))
                    .WithLeadingTrivia(argument.Expression.GetLeadingTrivia())
                    .WithTrailingTrivia(Space))
                .WithExpression(argument.Expression.WithoutLeadingTrivia());

        private static SyntaxToken CreateIdentifier(string identifier)
            => NeedsEscaping(identifier)
                ? CreateEscapedIdentifier(identifier).WithAdditionalAnnotations(Simplifier.Annotation)
                : Identifier(identifier);

        private static SyntaxToken CreateEscapedIdentifier(string identifier)
            => Identifier(
                leading: default,
                contextualKind: SyntaxKind.None,
                text: "@" + identifier,
                valueText: identifier,
                trailing: default);

        private static bool NeedsEscaping(string identifier)
            => SyntaxFacts.GetKeywordKind(identifier) is not SyntaxKind.None;
    }
}
