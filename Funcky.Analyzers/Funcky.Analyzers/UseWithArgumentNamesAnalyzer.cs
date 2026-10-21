using System.Collections.Immutable;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UseWithArgumentNamesAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = $"{DiagnosticName.Prefix}{DiagnosticName.Usage}03";

    /// <summary>The attribute is matched by name, so any assembly may define it: Funcky, the user's own project or the generator.</summary>
    internal const string AttributeFullName = "Funcky.CodeAnalysis.UseWithArgumentNamesAttribute";

    private const string Category = DiagnosticName.Category;

    private static readonly DiagnosticDescriptor Descriptor = new(
        id: DiagnosticId,
        title: Resources.UseWithArgumentNamesAnalyzerAnalyzerTitle,
        messageFormat: Resources.UseWithArgumentNamesAnalyzerMessageFormat,
        description: Resources.UseWithArgumentNamesAnalyzerDescription,
        category: Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: DiagnosticName.HelpLink(DiagnosticId));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(static context =>
        {
            var expressionOfTType = context.Compilation.GetExpressionOfTType();
            context.RegisterOperationAction(AnalyzeInvocation(expressionOfTType), OperationKind.Invocation);
        });
    }

    private static Action<OperationAnalysisContext> AnalyzeInvocation(INamedTypeSymbol? expressionOfTType)
        => context =>
        {
            var invocation = (IInvocationOperation)context.Operation;
            var semanticModel = invocation.SemanticModel ?? throw new InvalidOperationException("Semantic model is never be null for operations passed to an analyzer (according to docs)");

            if (invocation.TargetMethod.HasAttribute(AttributeFullName)
                && !invocation.Syntax.IsInExpressionTree(semanticModel, expressionOfTType, context.CancellationToken))
            {
                foreach (var argument in invocation.Arguments)
                {
                    if (argument.Syntax is ArgumentSyntax { NameColon: null } argumentSyntax
                        && argument.Parameter?.Name is { } parameterName)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, argumentSyntax.GetLocation(), parameterName));
                    }
                }
            }
        };
}
