using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Funcky.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EnumerableRepeatOnceAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(EnumerableRepeatOnceAnalyzer);
        private const string Category = "Funcky";

        private static readonly LocalizableString Title = "Enumerable.Repeat";
        private static readonly LocalizableString MessageFormat = "Enumerable.Repeat of single item.";
        private static readonly LocalizableString Description = "Use Sequence.Return instead";

        private static readonly DiagnosticDescriptor Rule = new("EnumerableRepeatOnceAnalyzer", Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(FindEnumerableRepeateOnce, SyntaxKind.InvocationExpression);
        }

        private void FindEnumerableRepeateOnce(SyntaxNodeAnalysisContext context)
        {
            var invocationExpr = (InvocationExpressionSyntax)context.Node;

            if (invocationExpr.Expression is not MemberAccessExpressionSyntax memberAccessExpr)
            {
                return;
            }

            if (memberAccessExpr.Name.ToString() != nameof(Enumerable.Repeat))
            {
                return;
            }

            if (context.SemanticModel.GetSymbolInfo(memberAccessExpr).Symbol is not IMethodSymbol memberSymbol)
            {
                return;
            }

            if (memberSymbol.Name == nameof(Enumerable))
            {
                return;
            }

            var argumentList = invocationExpr.ArgumentList;
            if (argumentList is not null && argumentList.Arguments.Count == 2)
            {
                if (argumentList.Arguments[1] is not null)
                {
                    var diagnostic = Diagnostic.Create(Rule, invocationExpr.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}
