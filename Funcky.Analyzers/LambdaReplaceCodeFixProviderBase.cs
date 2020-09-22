using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;

namespace Funcky.Analyzers
{
    public abstract class LambdaReplaceCodeFixProviderBase : CodeFixProvider
    {
        protected abstract string Title { get; }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (await GetRoot(context) is { } root &&
                FindLambda(context, root) is { } expression)
            {
                RegisterCodeFix(context, expression);
            }
        }

        public override FixAllProvider? GetFixAllProvider() => null;

        protected abstract SyntaxNode CreateReplacement(CodeFixContext context);

        private static async Task<SyntaxNode?> GetRoot(CodeFixContext context)
            => await context.Document.GetSyntaxRootAsync(context.CancellationToken);

        private static AnonymousFunctionExpressionSyntax? FindLambda(CodeFixContext context, SyntaxNode root)
            => root.FindNode(context.Span)
                .DescendantNodesAndSelf()
                .OfType<AnonymousFunctionExpressionSyntax>()
                .FirstOrDefault();

        private void RegisterCodeFix(CodeFixContext context, SyntaxNode expression)
            => context.RegisterCodeFix(
                   CodeAction.Create(
                       title: Title,
                       equivalenceKey: Title,
                       createChangedDocument: CreateChangedDocument(context, expression)),
                   context.Diagnostics);

        private Func<CancellationToken, Task<Document>> CreateChangedDocument(CodeFixContext context, SyntaxNode expression)
            => async cancellationToken =>
            {
                var editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken);
                editor.ReplaceNode(expression, CreateReplacement(context));
                return editor.GetChangedDocument();
            };
    }
}
