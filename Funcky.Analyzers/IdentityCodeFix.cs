using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Simplification;
using static Funcky.Analyzers.DiagnosticDescriptors;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    public sealed class IdentityCodeFix : CodeFixProvider
    {
        private const string Title = "Replace with Functional.Identity";

        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(UseFunctionalIdentity.Id);

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            if (await GetRoot(context) is { } root &&
                FindIdentityFunctionLambda(context, root) is { } expression)
            {
                RegisterCodeFix(context, expression);
            }
        }

        public override FixAllProvider? GetFixAllProvider() => null;

        private static async Task<SyntaxNode?> GetRoot(CodeFixContext context)
            => await context.Document.GetSyntaxRootAsync(context.CancellationToken);

        private static AnonymousFunctionExpressionSyntax? FindIdentityFunctionLambda(CodeFixContext context, SyntaxNode root)
            => root.FindNode(context.Span)
                .DescendantNodesAndSelf()
                .OfType<AnonymousFunctionExpressionSyntax>()
                .FirstOrDefault();

        private static void RegisterCodeFix(CodeFixContext context, SyntaxNode expression)
            => context.RegisterCodeFix(
                   CodeAction.Create(
                       title: Title,
                       equivalenceKey: Title,
                       createChangedDocument: CreateChangedDocument(context, expression)),
                   context.Diagnostics);

        private static Func<CancellationToken, Task<Document>> CreateChangedDocument(CodeFixContext context, SyntaxNode expression)
            => async cancellationToken =>
            {
                var editor = await DocumentEditor.CreateAsync(context.Document, cancellationToken);
                editor.ReplaceNode(expression, CreateIdentityFunctionMethodGroup());
                return editor.GetChangedDocument();
            };

        private static MemberAccessExpressionSyntax CreateIdentityFunctionMethodGroup()
            => MemberAccessExpression(
                   SyntaxKind.SimpleMemberAccessExpression,
                   QualifiedName(
                       IdentifierName("Funcky"),
                       IdentifierName("Functional")),
                   IdentifierName("Identity"));
    }
}
