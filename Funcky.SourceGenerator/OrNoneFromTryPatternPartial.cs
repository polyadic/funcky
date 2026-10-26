using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.SourceGenerator;

internal static class OrNoneFromTryPatternPartial
{
    public static CompilationUnitSyntax GetSyntaxTree(string namespaceName, string className, IEnumerable<MethodDeclarationSyntax> methods)
        => CompilationUnit()
            .AddMembers(BuildNamespace(namespaceName, className, methods));

    // The Roslyn version of the generator predates extension blocks, so the static extension members are emitted as text.
    public static string GetStaticExtensionClasses(string namespaceName, IEnumerable<StaticExtensionMember> members)
        => members.Any()
            ? $"namespace {namespaceName}\n{{\n{string.Join("\n\n", members.GroupBy(member => (member.ClassName, member.ReceiverType)).Select(BuildStaticExtensionClass))}\n}}\n"
            : string.Empty;

    private static string BuildStaticExtensionClass(IGrouping<(string ClassName, string ReceiverType), StaticExtensionMember> members)
        => $$"""
                 public static partial class {{members.Key.ClassName}}
                 {
                     extension({{members.Key.ReceiverType}})
                     {
             {{string.Join("\n", members.Select(member => Indent(member.Method.NormalizeWhitespace().ToFullString(), 12)))}}
                     }
                 }
             """;

    private static string Indent(string text, int width)
        => string.Join("\n", text.Split('\n').Select(line => line.Length is 0 ? line : new string(' ', width) + line.TrimEnd('\r')));

    private static NamespaceDeclarationSyntax BuildNamespace(string namespaceName, string className, IEnumerable<MemberDeclarationSyntax> methods)
        => NamespaceDeclaration(ParseName(namespaceName))
            .AddMembers(BuildClass(className, methods));

    private static ClassDeclarationSyntax BuildClass(string className, IEnumerable<MemberDeclarationSyntax> methods)
        => ClassDeclaration(className)
            .AddModifiers(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.StaticKeyword), Token(SyntaxKind.PartialKeyword))
            .AddMembers(methods.ToArray());
}
