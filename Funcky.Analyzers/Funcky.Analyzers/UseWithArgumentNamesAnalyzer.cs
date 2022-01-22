using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseWithArgumentNamesAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = $"{DiagnosticName.Prefix}{DiagnosticName.Usage}03";
        private const string Category = nameof(Funcky);

        private const string AttributeFullName = "Funcky.CodeAnalysis.UseWithArgumentNamesAttribute";

        private static readonly DiagnosticDescriptor Descriptor = new(
            id: DiagnosticId,
            title: Resources.UseWithArgumentNamesAnalyzerAnalyzerTitle,
            messageFormat: Resources.UseWithArgumentNamesAnalyzerMessageFormat,
            description: Resources.UseWithArgumentNamesAnalyzerDescription,
            category: Category,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        }

        private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
        {
            var node = (InvocationExpressionSyntax)context.Node;

            if (context.SemanticModel.GetOperation(node) is IInvocationOperation invocation &&
                context.Compilation.GetTypeByMetadataName(AttributeFullName) is { } attributeSymbol &&
                invocation.TargetMethod.GetAttributes().Any(attribute => SymbolEqualityComparer.Default.Equals(attribute.AttributeClass, attributeSymbol)))
            {
                foreach (var argument in node.ArgumentList.Arguments)
                {
                    if (argument.NameColon is null &&
                        context.SemanticModel.GetOperation(argument) is IArgumentOperation argumentOperation)
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Descriptor, argument.GetLocation(), argumentOperation.Parameter.Name));
                    }
                }
            }
        }
    }
}
