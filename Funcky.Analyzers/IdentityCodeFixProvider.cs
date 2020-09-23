using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using static Funcky.Analyzers.DiagnosticDescriptors;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers
{
    [ExportCodeFixProvider(LanguageNames.CSharp)]
    public sealed class IdentityCodeFixProvider : LambdaReplaceCodeFixProviderBase
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(UseFunctionalIdentity.Id);

        protected override string Title => "Replace with Functional.Identity";

        protected override SyntaxNode CreateReplacement(CodeFixContext context)
            => MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("Funcky"),
                    IdentifierName("Functional")),
                IdentifierName("Identity"));
    }
}
