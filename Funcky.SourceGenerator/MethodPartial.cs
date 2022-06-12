using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Funcky.SourceGenerator;

internal record MethodPartial(string NamespaceName, string ClassName, ImmutableArray<MethodDeclarationSyntax> Methods, SyntaxTree SourceTree);
