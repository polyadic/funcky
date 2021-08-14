using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
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
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                var argumentSyntax = root.FindNode(diagnostic.Location.SourceSpan).FirstAncestorOrSelf<ArgumentSyntax>();
                var operation = (IArgumentOperation)semanticModel.GetOperation(argumentSyntax);
                context.RegisterCodeFix(CreateFix(context, argumentSyntax, operation.Parameter), diagnostic);
            }
        }

        private CodeAction CreateFix(CodeFixContext context, ArgumentSyntax argument, IParameterSymbol parameter)
            => CodeAction.Create(
                string.Format(CodeFixResources.AddArgumentNameCodeFixTitle, parameter.Name),
                AddArgumentLabelAsync(context.Document, argument, parameter),
                nameof(AddArgumentNameCodeFix));

        private static Func<CancellationToken, Task<Document>> AddArgumentLabelAsync(Document document, ArgumentSyntax argument, IParameterSymbol parameter)
            => async cancellationToken =>
            {
                var syntaxRoot = await document.GetSyntaxRootAsync(cancellationToken);
                return document.WithSyntaxRoot(syntaxRoot.ReplaceNode(argument, AddArgumentName(argument, parameter)));
            };

        private static ArgumentSyntax AddArgumentName(ArgumentSyntax argument, IParameterSymbol parameter)
            => argument
                .WithNameColon(NameColon(parameter.Name)
                    .WithLeadingTrivia(argument.Expression.GetLeadingTrivia())
                    .WithTrailingTrivia(Space))
                .WithExpression(argument.Expression.WithoutLeadingTrivia());
    }
}
