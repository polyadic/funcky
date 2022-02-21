using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.SourceGenerator
{
    internal class OrNoneFromTryPatternPartial
    {
        public static CompilationUnitSyntax GetSyntaxTree(string namespaceName, string className, IEnumerable<UsingDirectiveSyntax> usings, IEnumerable<MemberDeclarationSyntax> methods)
            => CompilationUnit()
                .AddUsings(usings.ToArray())
                .AddMembers(BuildNamespace(namespaceName, className, methods));

        private static NamespaceDeclarationSyntax BuildNamespace(string namespaceName, string className, IEnumerable<MemberDeclarationSyntax> methods)
            => NamespaceDeclaration(ParseName(namespaceName))
                .AddMembers(BuildClass(className, methods));

        private static ClassDeclarationSyntax BuildClass(string className, IEnumerable<MemberDeclarationSyntax> methods)
            => ClassDeclaration(className)
                .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.PartialKeyword))
                .AddMembers(methods.ToArray());
    }
}
