using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers.Fallback;

/// <summary>Pairs the eager and the lazy overload of a fallback-taking method on <c>Option&lt;T&gt;</c>, <c>Result&lt;T&gt;</c>
/// and <c>Either&lt;L, R&gt;</c>: <c>GetOrElse(T)</c> and <c>GetOrElse(Func&lt;T&gt;)</c>, <c>OrElse</c>, <c>Match(none: …)</c> and any
/// future pair of the same shape. An overload is the sibling of another when only the parameter at one position differs and
/// the lazy parameter is a delegate returning exactly the eager parameter's type.</summary>
internal static class LazyOverloads
{
    /// <summary>For an argument passed to a non-delegate parameter: the overload that takes a delegate producing that value instead.</summary>
    public static IMethodSymbol? FindLazyOverload(IInvocationOperation invocation, IArgumentOperation argument, FallbackTypes fallbackTypes)
        => argument.Parameter is { Type: var eagerType } parameter
           && fallbackTypes.Contains(invocation.TargetMethod.ContainingType)
            ? SiblingOverloads(invocation.TargetMethod)
                .FirstOrDefault(sibling => DelegateReturnType(sibling.Parameters[parameter.Ordinal].Type) is { } returnType
                    && SymbolEquals(returnType, eagerType)
                    && OtherParametersMatch(invocation.TargetMethod, sibling, parameter.Ordinal))
            : null;

    /// <summary>For an argument passed to a delegate parameter: the overload that takes the delegate's return type directly.</summary>
    public static IMethodSymbol? FindEagerOverload(IInvocationOperation invocation, IArgumentOperation argument, FallbackTypes fallbackTypes)
        => argument.Parameter is { Type: var lazyType } parameter
           && fallbackTypes.Contains(invocation.TargetMethod.ContainingType)
           && DelegateReturnType(lazyType) is { } returnType
           && !IsDelegate(returnType)
            ? SiblingOverloads(invocation.TargetMethod)
                .FirstOrDefault(sibling => SymbolEquals(sibling.Parameters[parameter.Ordinal].Type, returnType)
                    && OtherParametersMatch(invocation.TargetMethod, sibling, parameter.Ordinal))
            : null;

    /// <summary>The number of parameters the delegate at the given position takes, e.g. 0 for <c>Func&lt;T&gt;</c> and 1 for <c>Func&lt;Exception, T&gt;</c>.</summary>
    public static int DelegateParameterCount(IMethodSymbol method, int parameterOrdinal)
        => (method.Parameters[parameterOrdinal].Type as INamedTypeSymbol)?.DelegateInvokeMethod?.Parameters.Length ?? 0;

    private static IEnumerable<IMethodSymbol> SiblingOverloads(IMethodSymbol method)
        => method.ContainingType
            .GetMembers(method.Name)
            .OfType<IMethodSymbol>()
            .Where(sibling => !SymbolEquals(sibling.OriginalDefinition, method.OriginalDefinition)
                && sibling.Parameters.Length == method.Parameters.Length
                && sibling.TypeParameters.Length == method.TypeParameters.Length)
            .Select(sibling => sibling.IsGenericMethod ? sibling.Construct(method.TypeArguments.ToArray()) : sibling);

    private static bool OtherParametersMatch(IMethodSymbol method, IMethodSymbol sibling, int except)
        => method.Parameters
            .Zip(sibling.Parameters, (parameter, siblingParameter) => parameter.Ordinal == except || SymbolEquals(parameter.Type, siblingParameter.Type))
            .All(matches => matches);

    private static ITypeSymbol? DelegateReturnType(ITypeSymbol type)
        => (type as INamedTypeSymbol)?.DelegateInvokeMethod?.ReturnType;

    private static bool IsDelegate(ITypeSymbol type)
        => type.TypeKind == TypeKind.Delegate;
}
