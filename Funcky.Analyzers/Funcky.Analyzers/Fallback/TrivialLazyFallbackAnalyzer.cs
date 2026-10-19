using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
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
        category: nameof(Funcky),
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

        foreach (var argument in invocation.Arguments.Where(argument => !argument.IsImplicit && IsTrivialLambda(argument.Value)))
        {
            if (LazyOverloads.FindEagerOverload(invocation, argument, fallbackTypes) is not null)
            {
                context.ReportDiagnostic(Diagnostic.Create(Descriptor, argument.Value.Syntax.GetLocation(), invocation.TargetMethod.Name));
            }
        }
    }

    private static bool IsTrivialLambda(IOperation operation)
        => operation is IDelegateCreationOperation { Target: IAnonymousFunctionOperation lambda }
           && MatchAnonymousFunctionWithSingleReturn(lambda) is [{ ReturnedValue: { } returnedValue }]
           && IsTrivialValue(returnedValue, lambda.Symbol);

    /// <summary>A value that costs nothing to evaluate eagerly and therefore gains nothing from being deferred.
    /// <c>null</c> and <c>default</c> are excluded: passed directly they are ambiguous between the two overloads.</summary>
    private static bool IsTrivialValue(IOperation operation, IMethodSymbol lambda)
        => operation switch
        {
            ILiteralOperation { ConstantValue: { HasValue: true, Value: not null } } => true,
            IParameterReferenceOperation { Parameter.ContainingSymbol: var owner } => !SymbolEquals(owner, lambda),
            ILocalReferenceOperation => true,
            IInstanceReferenceOperation => true,
            IFieldReferenceOperation { Instance: var instance } => instance is null || IsTrivialValue(instance, lambda),
            IPropertyReferenceOperation { Instance: var instance, Arguments.IsEmpty: true } => instance is null || IsTrivialValue(instance, lambda),
            IConversionOperation { IsImplicit: true, Operand: var operand } => IsTrivialValue(operand, lambda),
            ITypeOfOperation or INameOfOperation => true,
            _ => false,
        };
}
