using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using static Funcky.Analyzers.CodeFixResources;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers
{
    [Shared]
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(EnumerableRepeatOnceCodeFix))]
    public sealed class EnumerableRepeatOnceCodeFix : CodeFixProvider
    {
        private const string FullyQualifiedSequence = "Funcky.Sequence";
        private const string Return = "Return";

        public override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(EnumerableRepeatOnceAnalyzer.DiagnosticId);

        public override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnosticSpan = context.Diagnostics.First().Location.SourceSpan;

            if (root?.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf().OfType<InvocationExpressionSyntax>().First() is { } declaration)
            {
                context.RegisterCodeFix(CreateFix(context, declaration), GetDiagnostic(context));
            }
        }

        private static Diagnostic GetDiagnostic(CodeFixContext context)
            => context.Diagnostics.First();

        private static CodeAction CreateFix(CodeFixContext context, InvocationExpressionSyntax declaration)
            => CodeAction.Create(
                EnumerableRepeatOnceCodeFixTitle,
                CreateSequenceReturnAsync(context.Document, declaration),
                nameof(EnumerableRepeatOnceCodeFixTitle));

        private static Func<CancellationToken, Task<Document>> CreateSequenceReturnAsync(Document document, InvocationExpressionSyntax declaration)
            => async cancellationToken
                =>
                {
                    var editor = await DocumentEditor.CreateAsync(document, cancellationToken).ConfigureAwait(false);
                    editor.ReplaceNode(declaration, CreateSequenceReturnRoot(ExtractFirstArgument(declaration), editor.SemanticModel));
                    return editor.GetChangedDocument();
                };

        private static ArgumentSyntax ExtractFirstArgument(InvocationExpressionSyntax invocationExpr)
            => invocationExpr.ArgumentList.Arguments[Argument.First];

        private static SyntaxNode CreateSequenceReturnRoot(ArgumentSyntax firstArgument, SemanticModel model)
            => SyntaxSequenceReturn(firstArgument, model)
                .WithArgumentList(ArgumentList(SingletonSeparatedList(firstArgument))
                .WithCloseParenToken(Token(SyntaxKind.CloseParenToken)))
                .NormalizeWhitespace();

        private static InvocationExpressionSyntax SyntaxSequenceReturn(ArgumentSyntax firstArgument, SemanticModel model)
            => InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(SequenceType(model)?.ToMinimalDisplayString(model, firstArgument.SpanStart) ?? string.Empty),
                    IdentifierName(Return)));

        private static INamedTypeSymbol? SequenceType(SemanticModel model)
            => model.Compilation.GetTypeByMetadataName(FullyQualifiedSequence);
    }
}
