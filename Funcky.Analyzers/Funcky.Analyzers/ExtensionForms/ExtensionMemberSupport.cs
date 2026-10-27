using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Funcky.Analyzers.ExtensionForms;

/// <summary>What the rules for the C# 14 extension forms share: the language gate and the lookup of members that
/// live in an extension block, which the compiler nests in an unnamed extension type inside the static class.</summary>
public static class ExtensionMemberSupport
{
    public const string ExtensionsNamespace = "Funcky.Extensions";

    // 1400 is the numeric value of LanguageVersion.CSharp14; the analyzer is compiled against Roslyn versions that predate it.
    private const int CSharp14 = 1400;

    /// <summary>Whether the file is compiled with C# 14 or newer; on an older language version the static extension members do not bind.</summary>
    public static bool SupportsExtensionMembers(SyntaxTree syntaxTree)
        => syntaxTree.Options is CSharpParseOptions parseOptions
           && (int)parseOptions.LanguageVersion.MapSpecifiedToEffectiveVersion() >= CSharp14;

    /// <summary>Whether the static class declares a member of that name, directly or in one of its extension blocks.</summary>
    public static bool HasMember(INamedTypeSymbol type, string name)
        => type.GetMembers(name).Any()
           || type.GetTypeMembers().Any(extension => extension.GetMembers(name).Any());

    /// <summary>Whether the static class declares a static member of that name in one of its extension blocks (or directly, as the
    /// lowered form), which is what a static extension member is; a classic extension method does not count.</summary>
    public static bool HasStaticMember(INamedTypeSymbol type, string name)
        => type.GetTypeMembers()
            .SelectMany(extension => extension.GetMembers(name))
            .Concat(type.GetMembers(name))
            .OfType<IMethodSymbol>()
            .Any(method => method.IsStatic && !method.IsExtensionMethod);

    /// <summary>The static class an extension member or classic extension method is declared in. Roslyn 4.4 has no notion of
    /// an extension block, so any type nested in a static class is taken for one.</summary>
    public static INamedTypeSymbol? DeclaringClass(ISymbol? symbol)
        => symbol is IMethodSymbol method && (method.ReducedFrom ?? method).ContainingType is { } containingType
            ? containingType.ContainingType is { TypeKind: TypeKind.Class, IsStatic: true } outer
                ? outer
                : containingType
            : null;

    /// <summary>Whether the symbol is declared in a class of the <c>Funcky.Extensions</c> namespace, directly or through an extension block.</summary>
    public static bool IsDeclaredInFunckyExtensions(ISymbol? symbol)
        => DeclaringClass(symbol)?.ContainingNamespace.ToDisplayString() == ExtensionsNamespace;

    /// <summary>Whether the type can be named by its simple name at that position, which is the case when its namespace is imported.</summary>
    public static bool IsVisible(INamedTypeSymbol type, SemanticModel semanticModel, int position)
        => semanticModel.LookupNamespacesAndTypes(position, name: type.Name).Any(candidate => SymbolEquals(candidate, type));
}
