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
        if (context.Compilation.GetTypeByMetadataName(AttributeFullName) is { } attributeType)
        {
            context.RegisterOperationAction(AnalyzeArgument(attributeType), OperationKind.Argument);
        }
    }

    private static Action<OperationAnalysisContext> AnalyzeArgument(INamedTypeSymbol attributeType)
        => context
            =>
            {
                var operation = (IArgumentOperation)context.Operation;
                if (operation.Parameter is { } parameter
                    && operation.Value is IDelegateCreationOperation { Target: IAnonymousFunctionOperation lambda }
                    && parameter.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeType))
                    && MatchBlockOperationWithSingleReturn(lambda.Body) is { } returnedValue
                    && IsCheapOperation(returnedValue))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, lambda.Syntax.GetLocation()));
                }
            };

    private static IOperation? MatchBlockOperationWithSingleReturn(IOperation operation)
        => operation is IBlockOperation block
            && block.Operations.Length == 1
            && block.Operations[0] is IReturnOperation @return
                ? @return.ReturnedValue
                : null;

    private static bool IsCheapOperation(IOperation operation)
        => operation switch
        {
            IParameterReferenceOperation or ILocalReferenceOperation or ILiteralOperation => true,
            IConversionOperation conversion => IsCheapOperation(conversion.Operand),
            IObjectCreationOperation objectCreation => objectCreation.Children.All(IsCheapOperation),
            IFieldReferenceOperation fieldReference => fieldReference.Field.IsStatic,
            IPropertyReferenceOperation propertyReference => propertyReference.Property.IsStatic,
            _ => operation.ConstantValue.HasValue,
        };
}
