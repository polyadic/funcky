using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NonDefaultableAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor DoNotUseDefault = new DiagnosticDescriptor(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.Usage}09",
        title: "Do not use default to instantiate this type",
        messageFormat: "Do not use default(...) to instantiate '{0}'",
        category: nameof(Funcky),
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Values instantiated with default are in an invalid state; any member may throw an exception.");

    private const string AttributeFullName = "Funcky.CodeAnalysis.NonDefaultableAttribute";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(DoNotUseDefault);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        if (context.Compilation.GetTypeByMetadataName(AttributeFullName) is { } nonDefaultableAttribute)
        {
            context.RegisterOperationAction(AnalyzeDefaultValueOperation(nonDefaultableAttribute), OperationKind.DefaultValue);
        }
    }

    private static Action<OperationAnalysisContext> AnalyzeDefaultValueOperation(INamedTypeSymbol nonDefaultableAttribute)
        => context =>
        {
            var operation = (IDefaultValueOperation)context.Operation;
            if (operation.Type is { } type && type.GetAttributes().Any(IsAttribute(nonDefaultableAttribute)))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DoNotUseDefault,
                    operation.Syntax.GetLocation(),
                    messageArgs: type.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
            }
        };

    private static Func<AttributeData, bool> IsAttribute(INamedTypeSymbol attributeClass)
        => attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeClass);
}
