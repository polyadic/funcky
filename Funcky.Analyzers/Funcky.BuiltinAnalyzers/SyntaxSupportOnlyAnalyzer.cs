using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.BuiltinAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SyntaxSupportOnlyAnalyzer : DiagnosticAnalyzer
{
    private const string AttributeFullName = "Funcky.CodeAnalysis.SyntaxSupportOnlyAttribute";

    private static readonly DiagnosticDescriptor SyntaxSupportOnly = new(
        id: "λ0003",
        title: "Member is not intended for direct usage",
        messageFormat: "This {0} exists only to support the {1} syntax, do not use it directly",
        category: nameof(Funcky),
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        customTags: [WellKnownDiagnosticTags.NotConfigurable]);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(SyntaxSupportOnly);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        if (context.Compilation.GetTypeByMetadataName(AttributeFullName) is { } attributeType)
        {
            context.RegisterOperationAction(AnalyzePropertyReference(new AttributeType(attributeType)), OperationKind.PropertyReference);
        }
    }

    private static Action<OperationAnalysisContext> AnalyzePropertyReference(AttributeType attributeType)
        => context =>
        {
            var propertyReference = (IPropertyReferenceOperation)context.Operation;
            if (HasSyntaxSupportOnlyAttribute(propertyReference.Property, attributeType, out var syntaxFeature))
            {
                context.ReportDiagnostic(Diagnostic.Create(SyntaxSupportOnly, context.Operation.Syntax.GetLocation(), messageArgs: ["property", syntaxFeature]));
            }
        };

    private static bool HasSyntaxSupportOnlyAttribute(ISymbol symbol, AttributeType attributeType, [NotNullWhen((true))] out string? syntaxFeature)
    {
        syntaxFeature = null;
        return symbol.GetAttributes().FirstOrDefault(IsAttribute(attributeType.Value)) is { } attributeData
            && attributeData.ConstructorArguments is [{ Value: string syntaxFeatureValue }]
            && (syntaxFeature = syntaxFeatureValue) is var _;
    }

    private static Func<AttributeData, bool> IsAttribute(INamedTypeSymbol attributeClass)
        => attributeData
            => SymbolEqualityComparer.Default.Equals(attributeData.AttributeClass, attributeClass);

    private sealed record AttributeType(INamedTypeSymbol Value);
}
