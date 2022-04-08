using Microsoft.CodeAnalysis;

namespace Funcky.SourceGenerator;

internal static class OrNoneFromTryPatternValidator
{
    private static readonly DiagnosticDescriptor TypeMismatchDescriptor = new(
        id: "OrNoneFromTryPatternGenerator0001",
        title: "Type mismatch with target method",
        messageFormat: "Type mismatch. Expected: {0}, Actual: {1}.",
        category: "SourceGeneration",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static IEnumerable<Diagnostic> ValidateNullabilityOfParameters(
        SemanticModel semanticModel,
        IMethodSymbol parseOrNoneMethod,
        ISymbol? targetTypeSymbol,
        string targetMethodName)
        => FindTargetMethod(parseOrNoneMethod, targetTypeSymbol, targetMethodName) is { } targetMethod
            ? ValidateNullabilityOfParameters(semanticModel, parseOrNoneMethod, targetMethod)
            : Enumerable.Empty<Diagnostic>();

    private static IEnumerable<Diagnostic> ValidateNullabilityOfParameters(SemanticModel semanticModel, IMethodSymbol parseOrNoneMethod, IMethodSymbol targetMethod)
        => parseOrNoneMethod.Parameters.Zip(GetForwardedParameters(targetMethod), (parameter, expectedParameter) => (parameter, expectedParameter))
            .Where(p => !NullabilityMatchesIgnoringMissingAnnotation(p.parameter.Type, p.expectedParameter.Type))
            .Select(p => CreateDiagnostic(semanticModel, p.parameter, p.expectedParameter));

    private static Diagnostic CreateDiagnostic(SemanticModel semanticModel, IParameterSymbol parameter, IParameterSymbol expectedParameter)
    {
        var parameterLocation = parameter.Locations.First();
        return Diagnostic.Create(
            TypeMismatchDescriptor,
            parameterLocation,
            expectedParameter.ToMinimalDisplayString(semanticModel, parameterLocation.SourceSpan.Start),
            parameter.ToMinimalDisplayString(semanticModel, parameterLocation.SourceSpan.Start));
    }

    private static IMethodSymbol? FindTargetMethod(IMethodSymbol parseOrNoneMethod, ISymbol? targetTypeSymbol, string targetMethodName)
        => targetTypeSymbol is ITypeSymbol targetType
            ? targetType
                .GetMembers()
                .OfType<IMethodSymbol>()
                .Where(m => m.Name == targetMethodName)
                .SingleOrDefault(ParameterTypesMatchIgnoringNullability(parseOrNoneMethod))
            : null;

    private static Func<IMethodSymbol, bool> ParameterTypesMatchIgnoringNullability(IMethodSymbol parseOrNoneMethod)
        => methodSymbol
            => GetForwardedParameters(methodSymbol).Select(p => p.Type)
                .SequenceEqual(parseOrNoneMethod.Parameters.Select(p => p.Type), SymbolEqualityComparer.Default);

    private static bool NullabilityMatchesIgnoringMissingAnnotation(ITypeSymbol x, ITypeSymbol y)
        => (x.NullableAnnotation == y.NullableAnnotation
            || x.NullableAnnotation == NullableAnnotation.None
            || y.NullableAnnotation == NullableAnnotation.None)
           && SymbolEqualityComparer.Default.Equals(x, y);

    private static IEnumerable<IParameterSymbol> GetForwardedParameters(IMethodSymbol targetMethod)
        => targetMethod.Parameters.Where(p => p.RefKind is not RefKind.Out);
}
