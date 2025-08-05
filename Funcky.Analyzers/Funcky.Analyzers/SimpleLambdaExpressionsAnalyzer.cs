using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SimpleLambdaExpressionsAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = $"{DiagnosticName.Prefix}{DiagnosticName.Usage}05";

    private const string AttributeFullName = "Funcky.CodeAnalysis.InlineSimpleLambdaExpressionsAttribute";

    private static readonly DiagnosticDescriptor Descriptor = new DiagnosticDescriptor(
        id: DiagnosticId,
        title: "Simple lambda expression can be inlined",
        messageFormat: "Simple lambda expression can be inlined",
        description: "TODO.",
        category: nameof(Funcky),
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(OnCompilationStarted);
    }

    private static void OnCompilationStarted(CompilationStartAnalysisContext context)
    {
        context.RegisterOperationAction(AnalyzeArgument, OperationKind.Argument);
    }

    private static void AnalyzeArgument(OperationAnalysisContext context)
    {
        var operation = (IArgumentOperation)context.Operation;
        if (operation.Parameter is { } parameter
            && operation.Value is IDelegateCreationOperation { Target: IAnonymousFunctionOperation lambda }
            && parameter.ContainingSymbol is IMethodSymbol { ContainingType: var containingType } method
            && MatchBlockOperationWithSingleReturn(lambda.Body) is { } returnedValue
            && containingType.GetMembers().OfType<IMethodSymbol>().Any(m => m.Name == method.Name
                && SymbolEqualityComparer.IncludeNullability.Equals(m.ReturnType, method.ReturnType)
                && m.Parameters.Length == 1
                && SymbolEqualityComparer.IncludeNullability.Equals(m.Parameters[0].Type, returnedValue.Type)))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, lambda.Syntax.GetLocation()));

            // var kind = DetectSimpleOperation(returnedValue, lambda);
            //
            // switch (kind)
            // {
            //     case SimpleOperationKind.Certain:
            //         context.ReportDiagnostic(Diagnostic.Create(Descriptor, lambda.Syntax.GetLocation()));
            //         break;
            //     case SimpleOperationKind.Maybe:
            //         context.ReportDiagnostic(Diagnostic.Create(Descriptor, lambda.Syntax.GetLocation(), DiagnosticSeverity.Info, null, null));
            //         break;
            // }
        }
    }

    private static IOperation? MatchBlockOperationWithSingleReturn(IOperation operation)
        => operation is IBlockOperation blockOperation
            && blockOperation.Operations.Length == 1
            && blockOperation.Operations[0] is IReturnOperation @return
            ? @return.ReturnedValue
            : null;

    private static SimpleOperationKind DetectSimpleOperation(IOperation operation, IAnonymousFunctionOperation lambda)
        => operation switch
        {
            _ when operation.ConstantValue.HasValue => SimpleOperationKind.Certain,
            IParameterReferenceOperation parameterReference when !SymbolEqualityComparer.Default.Equals(parameterReference.Parameter.ContainingSymbol, lambda.Symbol) => SimpleOperationKind.Certain,
            ILocalReferenceOperation or ILiteralOperation => SimpleOperationKind.Certain,
            IConversionOperation conversion => DetectSimpleOperation(conversion.Operand, lambda),
            IObjectCreationOperation objectCreation when objectCreation.Children.Any() => Min(objectCreation.Children.Min(c => DetectSimpleOperation(c, lambda)), SimpleOperationKind.Maybe),
            IObjectCreationOperation => SimpleOperationKind.Maybe,
            IAnonymousObjectCreationOperation creation when creation.Initializers.Any() => creation.Initializers.Cast<ISimpleAssignmentOperation>().Select(x => x.Value).Min(c => DetectSimpleOperation(c, lambda)),
            IAnonymousObjectCreationOperation => SimpleOperationKind.Certain,
            IFieldReferenceOperation fieldReference when fieldReference.Field.IsStatic => SimpleOperationKind.Certain,
            IPropertyReferenceOperation propertyReference when propertyReference.Property.IsStatic => SimpleOperationKind.Maybe,
            _ => SimpleOperationKind.None,
        };

    private static SimpleOperationKind Min(SimpleOperationKind lhs, SimpleOperationKind rhs) => lhs < rhs ? lhs : rhs;

#pragma warning disable SA1201
    private enum SimpleOperationKind
#pragma warning restore SA1201
    {
        None,
        Maybe,
        Certain,
    }
}
