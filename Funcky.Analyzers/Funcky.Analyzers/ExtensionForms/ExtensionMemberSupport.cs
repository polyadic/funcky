using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers.ExtensionForms;

/// <summary>What the rules for the C# 14 extension forms share: the lookup of members that live in an extension block,
/// which the compiler nests in an unnamed extension type inside the static class. There is no language gate: Funcky 4
/// requires the .NET 10 SDK, whose compiler binds the static extension members.</summary>
public static class ExtensionMemberSupport
{
    public const string ExtensionsNamespace = "Funcky.Extensions";

    /// <summary>The extension blocks of a static class. Roslyn 4.14 has no notion of an extension block, but the compiler nests each
    /// one as an unnamed type, which tells it apart from an ordinary nested class.</summary>
    public static IEnumerable<INamedTypeSymbol> ExtensionBlocks(INamedTypeSymbol type)
        => type.GetTypeMembers().Where(IsExtensionBlock);

    /// <summary>Whether the static class declares a member of that name, directly or in one of its extension blocks.</summary>
    public static bool HasMember(INamedTypeSymbol type, string name)
        => type.GetMembers(name).Any()
           || ExtensionBlocks(type).Any(extension => extension.GetMembers(name).Any());

    /// <summary>Whether the static class declares a static member of that name in one of its extension blocks (or directly, as the
    /// lowered form), which is what a static extension member is; a classic extension method does not count.</summary>
    public static bool HasStaticMember(INamedTypeSymbol type, string name)
        => ExtensionBlocks(type)
            .SelectMany(extension => extension.GetMembers(name))
            .Concat(type.GetMembers(name))
            .OfType<IMethodSymbol>()
            .Any(method => method.IsStatic && !method.IsExtensionMethod);

    /// <summary>The class a method is declared in: for a member of an extension block that is the static class around the block,
    /// for every other method (a classic extension method included) its containing type.</summary>
    public static INamedTypeSymbol? DeclaringClass(ISymbol? symbol)
        => symbol is IMethodSymbol method && (method.ReducedFrom ?? method).ContainingType is { } containingType
            ? IsExtensionBlock(containingType) && containingType.ContainingType is { } outer
                ? outer
                : containingType
            : null;

    /// <summary>Whether the symbol is declared in a class of the <c>Funcky.Extensions</c> namespace, directly or through an extension block.</summary>
    public static bool IsDeclaredInFunckyExtensions(ISymbol? symbol)
        => DeclaringClass(symbol)?.ContainingNamespace.ToDisplayString() == ExtensionsNamespace;

    /// <summary>Whether the type can be named by its simple name at that position, which is the case when its namespace is imported.</summary>
    public static bool IsVisible(INamedTypeSymbol type, SemanticModel semanticModel, int position)
        => semanticModel.LookupNamespacesAndTypes(position, name: type.Name).Any(candidate => SymbolEquals(candidate, type));

    /// <summary>The receiver of a call and its remaining arguments in parameter order. A member of an extension block is called on
    /// an instance; a classic extension method, also when called in static form, and the lowered static form of an extension
    /// member take the receiver as their first argument. A static method without arguments has no receiver.</summary>
    internal static Option<(IOperation Receiver, ImmutableArray<IArgumentOperation> Arguments)> ReceiverAndArguments(IInvocationOperation invocation)
        => invocation switch
        {
            { Instance: { } instance } => [(instance, ArgumentsInParameterOrder(invocation))],
            { TargetMethod.IsStatic: true } when ArgumentsInParameterOrder(invocation) is [var receiver, .. var arguments] => [(receiver.Value, arguments)],
            _ => [],
        };

    // From source the block is an unnamed type, from metadata it carries the unspeakable name the compiler gives it.
    private static bool IsExtensionBlock(INamedTypeSymbol type)
        => type.Name.Length == 0 || type.MetadataName.StartsWith("<>E__", StringComparison.Ordinal);

    private static ImmutableArray<IArgumentOperation> ArgumentsInParameterOrder(IInvocationOperation invocation)
        => invocation.Arguments.Sort((x, y) => Comparer<int?>.Default.Compare(x.Parameter?.Ordinal, y.Parameter?.Ordinal));
}
