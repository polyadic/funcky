using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.Functions.AnonymousFunctionMatching;

namespace Funcky.Analyzers.Fallback;

/// <summary>Reports a lambda passed to the lazy overload of a fallback-taking method whose body is a plain value, e.g.
/// <c>option.GetOrElse(() => 0)</c>: the value costs nothing to evaluate, so the eager overload says the same with less.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TrivialLazyFallbackAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = $"{DiagnosticName.Prefix}{DiagnosticName.Usage}16";

    private static readonly DiagnosticDescriptor Descriptor = new(
        id: DiagnosticId,
        title: "Pass the fallback value directly",
        messageFormat: "The function passed to '{0}' only returns a value; pass the value directly",
        category: DiagnosticName.Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "The lazy overload defers work until the fallback is needed. A lambda that only returns a literal, a constant, a variable or a member has no work to defer, so the eager overload taking the value is the clearer choice.",
        helpLinkUri: DiagnosticName.HelpLink(DiagnosticId));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        var fallbackTypes = FallbackTypes.FromCompilation(context.Compilation);

        if (!fallbackTypes.IsEmpty)
        {
            context.RegisterOperationAction(context => AnalyzeInvocation(context, fallbackTypes), OperationKind.Invocation);
        }
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context, FallbackTypes fallbackTypes)
    {
        var invocation = (IInvocationOperation)context.Operation;
        var semanticModel = invocation.SemanticModel ?? throw new InvalidOperationException("Semantic model is never null for operations passed to an analyzer (according to docs)");

        foreach (var argument in invocation.Arguments.Where(argument => !argument.IsImplicit))
        {
            if (LazyOverloads.FindEagerOverload(invocation, argument, fallbackTypes) is { } eagerOverload
                && TrivialReturnedValue(argument.Value, semanticModel, fallbackTypes) is { } returnedValue
                && PassingTheValueSelects(eagerOverload, invocation, argument, returnedValue, semanticModel))
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, argument.Value.Syntax.GetLocation(), invocation.TargetMethod.Name));
            }
        }
    }

    // An async lambda returns a task around its body, so the body is not the value the eager overload would take.
    private static IOperation? TrivialReturnedValue(IOperation operation, SemanticModel semanticModel, FallbackTypes fallbackTypes)
    {
        if (operation is IDelegateCreationOperation { Target: IAnonymousFunctionOperation { Symbol.IsAsync: false } lambda }
            && MatchAnonymousFunctionWithSingleReturn(lambda) is [{ ReturnedValue: { } returnedValue }]
            && new TrivialValueClassifier(lambda.Symbol, semanticModel, fallbackTypes).IsTrivial(returnedValue))
        {
            return returnedValue;
        }

        return null;
    }

    // Passing the value is only an improvement if overload resolution then picks the eager overload: a value that is
    // itself a delegate (`Func<object> fallback` for `Option<object>`) would be taken by the lazy overload and invoked.
    private static bool PassingTheValueSelects(IMethodSymbol eagerOverload, IInvocationOperation invocation, IArgumentOperation argument, IOperation returnedValue, SemanticModel semanticModel)
    {
        if (invocation.Syntax is not InvocationExpressionSyntax invocationSyntax || returnedValue.Syntax is not ExpressionSyntax valueSyntax)
        {
            return false;
        }

        var replacement = invocationSyntax.ReplaceNode(argument.Value.Syntax, valueSyntax.WithoutTrivia());
        var symbol = semanticModel.GetSpeculativeSymbolInfo(invocationSyntax.SpanStart, replacement, SpeculativeBindingOption.BindAsExpression).Symbol;
        return SymbolEquals(symbol, eagerOverload);
    }

    /// <summary>A value that costs nothing to evaluate eagerly and therefore gains nothing from being deferred.
    /// <c>null</c> and <c>default</c> are excluded: passed directly they are ambiguous between the two overloads.</summary>
    private sealed class TrivialValueClassifier(IMethodSymbol lambda, SemanticModel semanticModel, FallbackTypes fallbackTypes)
    {
        public bool IsTrivial(IOperation operation)
            => operation switch
            {
                ILiteralOperation { ConstantValue: { HasValue: true, Value: not null } } => true,
                IParameterReferenceOperation parameter => !BelongsToTheLambda(parameter),
                ILocalReferenceOperation => true,
                IInstanceReferenceOperation => true,
                IFieldReferenceOperation { Instance: var instance } => IsSafeReceiver(instance),
                IPropertyReferenceOperation { Property: var property, Instance: var instance, Arguments.IsEmpty: true } => IsCheapGetter(property) && IsSafeReceiver(instance),
                IConversionOperation { IsImplicit: true, Conversion: var conversion, Operand: var operand } => IsCheapConversion(conversion) && IsTrivial(operand),
                ITypeOfOperation => true,
                INameOfOperation nameOf => !nameOf.Descendants().OfType<IParameterReferenceOperation>().Any(BelongsToTheLambda),
                _ => false,
            };

        private bool BelongsToTheLambda(IParameterReferenceOperation parameter)
            => SymbolEquals(parameter.Parameter.ContainingSymbol, lambda);

        // A member read is only free when the receiver cannot be null: a static member, `this`, a value or a reference the
        // compiler knows to be non-null here. Without that knowledge the deferred read may have hidden a null check.
        private bool IsSafeReceiver(IOperation? instance)
            => instance switch
            {
                null => true,
                IInstanceReferenceOperation => true,
                { Type.IsValueType: true } => IsTrivial(instance),
                _ => IsTrivial(instance) && semanticModel.GetTypeInfo(instance.Syntax).Nullability.FlowState == NullableFlowState.NotNull,
            };

        // A getter can do anything (`Lazy<T>.Value` initializes, `Nullable<T>.Value` throws), so only an auto-property
        // in source and the `None` of the monads themselves count as a plain read.
        private bool IsCheapGetter(IPropertySymbol property)
            => (property.Name == "None" && fallbackTypes.Contains(property.ContainingType))
               || property.DeclaringSyntaxReferences.Any(reference => reference.GetSyntax() is PropertyDeclarationSyntax { ExpressionBody: null, AccessorList.Accessors: var accessors }
                   && accessors.Any(accessor => accessor.Keyword.ValueText == "get" && accessor is { Body: null, ExpressionBody: null }));

        // A user-defined conversion runs arbitrary code; the implicit conversions of the monads themselves only wrap the value.
        private bool IsCheapConversion(CommonConversion conversion)
            => !conversion.IsUserDefined
               || (conversion.MethodSymbol is { ContainingType: var operatorOwner } && fallbackTypes.Contains(operatorOwner));
    }
}
