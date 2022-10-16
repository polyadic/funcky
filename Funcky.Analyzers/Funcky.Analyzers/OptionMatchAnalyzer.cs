using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.ConstantFunctionMatching;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;
using static Funcky.Analyzers.IdentityFunctionMatching;
using static Funcky.Analyzers.OptionReturnMatching;

namespace Funcky.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class OptionMatchAnalyzer : DiagnosticAnalyzer
{
    public const string PreservedArgumentIndexProperty = nameof(PreservedArgumentIndexProperty);

    public static readonly DiagnosticDescriptor PreferGetOrElse = new DiagnosticDescriptor(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.Usage}05",
        title: $"Prefer {GetOrElseMethodName} over {MatchMethodName}",
        messageFormat: $"Prefer {GetOrElseMethodName} over {MatchMethodName}",
        category: nameof(Funcky),
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    public static readonly DiagnosticDescriptor PreferOrElse = new DiagnosticDescriptor(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.Usage}06",
        title: $"Prefer {OrElseMethodName} over {MatchMethodName}",
        messageFormat: $"Prefer {OrElseMethodName} over {MatchMethodName}",
        category: nameof(Funcky),
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    public static readonly DiagnosticDescriptor PreferSelectMany = new DiagnosticDescriptor(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.Usage}07",
        title: $"Prefer {SelectManyMethodName} over {MatchMethodName}",
        messageFormat: $"Prefer {SelectManyMethodName} over {MatchMethodName}",
        category: nameof(Funcky),
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    public static readonly DiagnosticDescriptor PreferToNullable = new DiagnosticDescriptor(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.Usage}08",
        title: $"Prefer {OptionToNullableMethodName} over {MatchMethodName}",
        messageFormat: $"Prefer {OptionToNullableMethodName} over {MatchMethodName}",
        category: nameof(Funcky),
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: string.Empty);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(PreferGetOrElse, PreferOrElse, PreferSelectMany, PreferToNullable);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            if (context.Compilation.GetOptionOfTType() is { } optionOfTType)
            {
                context.RegisterOperationAction(context => AnalyzeInvocation(context, optionOfTType), OperationKind.Invocation);
            }
        });
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context, INamedTypeSymbol optionOfTType)
    {
        var operation = (IInvocationOperation)context.Operation;

        if (IsMatchInvocation(operation, optionOfTType, out var receiverType)
            && AnalyzeMatchInvocation(
                operation,
                receiverType,
                noneArgument: operation.GetArgumentForParameterAtIndex(0),
                someArgument: operation.GetArgumentForParameterAtIndex(1)) is { } diagnostic)
        {
            context.ReportDiagnostic(diagnostic);
        }
    }

    private static bool IsMatchInvocation(
        IInvocationOperation invocation,
        INamedTypeSymbol optionOfTType,
        [NotNullWhen(true)] out INamedTypeSymbol? receiverType)
    {
        receiverType = null;
        return invocation.TargetMethod.ReceiverType is INamedTypeSymbol receiverType_
           && SymbolEqualityComparer.Default.Equals(receiverType_.ConstructedFrom, optionOfTType)
           && invocation.TargetMethod.Name == MatchMethodName
           && invocation.Arguments.Length == 2
           && (receiverType = receiverType_) is var _;
    }

    private static Diagnostic? AnalyzeMatchInvocation(
        IInvocationOperation matchInvocation,
        INamedTypeSymbol receiverType,
        IArgumentOperation noneArgument,
        IArgumentOperation someArgument)
    {
        if (IsToNullableEquivalent(matchInvocation, receiverType, noneArgument, someArgument))
        {
            return Diagnostic.Create(PreferToNullable, matchInvocation.Syntax.GetLocation());
        }

        if (IsGetOrElseEquivalent(receiverType, noneArgument, someArgument))
        {
            var noneArgumentIndex = matchInvocation.Arguments.IndexOf(noneArgument);
            return Diagnostic.Create(
                PreferGetOrElse,
                matchInvocation.Syntax.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty.Add(PreservedArgumentIndexProperty, noneArgumentIndex.ToString()));
        }

        if (IsOrElseEquivalent(matchInvocation, receiverType, someArgument))
        {
            var noneArgumentIndex = matchInvocation.Arguments.IndexOf(noneArgument);
            return Diagnostic.Create(
                PreferOrElse,
                matchInvocation.Syntax.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty.Add(PreservedArgumentIndexProperty, noneArgumentIndex.ToString()));
        }

        if (IsSelectManyEquivalent(matchInvocation, receiverType, noneArgument))
        {
            var someArgumentIndex = matchInvocation.Arguments.IndexOf(someArgument);
            return Diagnostic.Create(
                PreferSelectMany,
                matchInvocation.Syntax.GetLocation(),
                properties: ImmutableDictionary<string, string?>.Empty.Add(PreservedArgumentIndexProperty, someArgumentIndex.ToString()));
        }

        return null;
    }

    /// <summary>Tests for a <c>Match</c> invocation of the shape <c>Match(none: A, some: Identity)</c>.</summary>
    private static bool IsGetOrElseEquivalent(INamedTypeSymbol receiverType, IArgumentOperation noneArgument, IArgumentOperation someArgument)
        => SymbolEqualityComparer.IncludeNullability.Equals(receiverType.TypeArguments.Single(), GetTypeOrDelegateReturnType(noneArgument.Value))
           && SymbolEqualityComparer.IncludeNullability.Equals(receiverType.TypeArguments.Single(), GetTypeOrDelegateReturnType(someArgument.Value))
           && IsIdentityFunction(someArgument.Value);

    /// <summary>Tests for a <c>Match</c> invocation of the shape <c>Match(none: A, some: Option.Return)</c>.</summary>
    private static bool IsOrElseEquivalent(IInvocationOperation matchInvocation, INamedTypeSymbol receiverType, IArgumentOperation someArgument)
        => SymbolEqualityComparer.IncludeNullability.Equals(receiverType, matchInvocation.Type)
            && IsOptionReturnFunction(someArgument.Value);

    /// <summary>Tests for a <c>Match</c> invocation of the shape <c>Match(none: Option&lt;T&gt;>.None, some: A)</c>.</summary>
    private static bool IsSelectManyEquivalent(IInvocationOperation matchInvocation, INamedTypeSymbol receiverType, IArgumentOperation noneArgument)
        => SymbolEqualityComparer.IncludeNullability.Equals(receiverType, matchInvocation.Type)
           && IsOptionNoneExpression(noneArgument.Value);

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
                && SymbolEqualityComparer.Default.Equals(receiverType.TypeArguments.Single(), matchInvocation.Type)
                && IsNullOrNullFunction(noneArgument.Value)
                && IsIdentityFunction(someArgument.Value);

        bool IsToNullableValueType()
            => itemType.IsValueType
                && SymbolEqualityComparer.Default.Equals(matchInvocation.SemanticModel?.NullableOfT(itemType), matchInvocation.Type)
                && IsNullOrNullFunction(noneArgument.Value)
                && IsIdentityFunctionWithNullConversion(someArgument.Value);

        static bool IsNullOrNullFunction(IOperation operation)
            => operation is { ConstantValue: { HasValue: true, Value: null } } || IsConstantFunction(operation, expectedValue: null);
    }

    private static bool IsOptionNoneExpression(IOperation operation)
        => operation is IPropertyReferenceOperation { Property: { Name: OptionNonePropertyName, IsStatic: true, ContainingType: var type } }
            && SymbolEqualityComparer.Default.Equals(type.ConstructedFrom, operation.SemanticModel?.Compilation.GetOptionOfTType());

    private static ITypeSymbol? GetTypeOrDelegateReturnType(IOperation operation)
        => operation switch
        {
            IDelegateCreationOperation { Target: IAnonymousFunctionOperation { Body.Operations: { Length: 1 } operations } } when operations[0] is IReturnOperation returnOperation => returnOperation.ReturnedValue?.Type,
            IDelegateCreationOperation { Target: IAnonymousFunctionOperation { Symbol.ReturnType: var returnType } } => returnType,
            IDelegateCreationOperation { Target: IMethodReferenceOperation { Method.ReturnType: var returnType } } => returnType,
            _ => operation.Type,
        };
}
