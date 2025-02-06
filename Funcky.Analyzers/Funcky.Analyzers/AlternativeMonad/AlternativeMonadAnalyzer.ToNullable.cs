using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;
using static Funcky.Analyzers.Functions.ConstantFunctionMatching;
using static Funcky.Analyzers.Functions.IdentityFunctionMatching;

namespace Funcky.Analyzers.AlternativeMonad;

public partial class AlternativeMonadAnalyzer
{
    public static readonly DiagnosticDescriptor PreferToNullable = new(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.Usage}08",
        title: $"Prefer {ToNullableMethodName} over {MatchMethodName}",
        messageFormat: $"Prefer {ToNullableMethodName} over {MatchMethodName}",
        category: nameof(Funcky),
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    /// <summary>Tests for a <c>Match</c> invocation of the shape <c>Match(none: null, some: Identity)</c>.</summary>
    private static bool IsToNullableEquivalent(
        IInvocationOperation matchInvocation,
        INamedTypeSymbol receiverType,
        IArgumentOperation noneArgument,
        IArgumentOperation someArgument)
    {
        var itemType = receiverType.TypeArguments.Single();
        return IsToNullableReferenceType() || IsToNullableValueType();

        bool IsToNullableReferenceType()
            => itemType.IsReferenceType
                && SymbolEquals(receiverType.TypeArguments.Single(), matchInvocation.Type)
                && IsNullOrNullFunction(noneArgument.Value)
                && IsIdentityFunction(someArgument.Value);

        bool IsToNullableValueType()
            => itemType.IsValueType
                && SymbolEquals(matchInvocation.SemanticModel?.NullableOfT(itemType), matchInvocation.Type)
                && IsNullOrNullFunction(noneArgument.Value)
                && IsIdentityFunctionWithNullConversion(someArgument.Value);

        static bool IsNullOrNullFunction(IOperation operation)
            => operation is { ConstantValue: { HasValue: true, Value: null } } || IsConstantFunction(operation, expectedValue: null);
    }
}
