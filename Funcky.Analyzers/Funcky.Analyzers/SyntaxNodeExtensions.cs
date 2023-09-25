using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Funcky.Analyzers;

internal static partial class SyntaxNodeExtensions
{
    internal static ImmutableArray<ISymbol> GetAllSymbols(SymbolInfo info)
        => info.Symbol == null
            ? info.CandidateSymbols
            : ImmutableArray.Create(info.Symbol);

    // Copied from Roslyn's source code as this API is not public:
    // https://github.com/dotnet/roslyn/blob/232f7afa4966411958759c880de3a1765bdb28a0/src/Workspaces/SharedUtilitiesAndExtensions/Compiler/CSharp/Extensions/SyntaxNodeExtensions.cs#L925
    internal static bool IsAnyLambda([NotNullWhen(returnValue: true)] this SyntaxNode? node)
        => node?.Kind() is SyntaxKind.ParenthesizedLambdaExpression or SyntaxKind.SimpleLambdaExpression;
}
