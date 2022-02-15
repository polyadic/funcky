using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
                => document.WithSyntaxRoot(await ReplaceWithSequenceReturn(document, declaration, cancellationToken).ConfigureAwait(false));

        private static async Task<SyntaxNode> ReplaceWithSequenceReturn(Document document, InvocationExpressionSyntax declaration, CancellationToken cancellationToken)
            => await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false) is { } oldRoot && await document.GetSemanticModelAsync(cancellationToken) is { } semanticModel
                ? oldRoot.ReplaceNode(declaration, CreateSequenceReturnRoot(ExtractFirstArgument(declaration), semanticModel))
                : throw new Exception("oldRoot or semanticModel are null");

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
