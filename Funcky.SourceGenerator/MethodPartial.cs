using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Funcky.SourceGenerator;

/// <param name="NamespaceName">The namespace of the attributed class.</param>
/// <param name="ClassName">The attributed class, which receives the classic <c>this</c> extension methods.</param>
/// <param name="Methods">The classic extension methods on the parsed text.</param>
/// <param name="StaticMembers">The same methods as static extension members on the target type, grouped by their enclosing class.</param>
/// <param name="SourceTree">The syntax tree the attributed class is declared in.</param>
internal record MethodPartial(
    string NamespaceName,
    string ClassName,
    ImmutableArray<MethodDeclarationSyntax> Methods,
    ImmutableArray<StaticExtensionMember> StaticMembers,
    SyntaxTree SourceTree);

/// <param name="ClassName">The enclosing static class; one per target type, because the lowered static members of different types would otherwise share a signature.</param>
/// <param name="ReceiverType">The fully qualified target type of the <c>extension(...)</c> block.</param>
/// <param name="Method">The static member.</param>
internal record StaticExtensionMember(string ClassName, string ReceiverType, MethodDeclarationSyntax Method);
