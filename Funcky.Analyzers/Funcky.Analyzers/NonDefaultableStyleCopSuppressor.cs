using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.NonDefaultableAnalyzer;

namespace Funcky.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NonDefaultableStyleCopSuppressor : DiagnosticSuppressor
{
    private static readonly SuppressionDescriptor DoNotUseDefaultValueTypeConstructorSuppression = new(
        "Funcky-Suppress-SA1129",
        "SA1129",
        "λ1009 already disallows usage of this constructor.");

    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(DoNotUseDefaultValueTypeConstructorSuppression);

    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        if (context.Compilation.GetTypeByMetadataName(AttributeFullName) is { } nonDefaultableAttribute)
        {
            ReportSuppressions(context, nonDefaultableAttribute);
        }
    }

    private static void ReportSuppressions(SuppressionAnalysisContext context, INamedTypeSymbol nonDefaultableAttribute)
    {
        var styleCopDiagnostics = context.ReportedDiagnostics.Where(diagnostic => diagnostic.Id == DoNotUseDefaultValueTypeConstructorSuppression.SuppressedDiagnosticId);
        foreach (var diagnostic in styleCopDiagnostics)
        {
            if (GetOperation(context, diagnostic) is IObjectCreationOperation operation
                && IsParameterlessObjectCreationOfNonDefaultableStruct(operation, nonDefaultableAttribute))
            {
                context.ReportSuppression(Suppression.Create(DoNotUseDefaultValueTypeConstructorSuppression, diagnostic));
            }
        }
    }

    private static IOperation? GetOperation(SuppressionAnalysisContext context, Diagnostic diagnostic)
    {
        var location = diagnostic.Location;
        var syntaxTree = location.SourceTree;
        return syntaxTree?.GetRoot(context.CancellationToken) is { } root
            && context.GetSemanticModel(syntaxTree) is { } semanticModel
                ? semanticModel.GetOperation(root.FindNode(location.SourceSpan))
                : null;
    }
}
