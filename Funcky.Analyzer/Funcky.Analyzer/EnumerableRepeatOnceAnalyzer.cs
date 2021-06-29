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
            ////context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(FindEnumerableRepeateOnce, SyntaxKind.InvocationExpression);
        }

        private void FindEnumerableRepeateOnce(SyntaxNodeAnalysisContext context)
        {
            var syntaxMatcher = new SyntaxMatcher(context);

            if (syntaxMatcher.MatchStaticCall(nameof(Enumerable), nameof(Enumerable.Repeat)))
            {
                var invocationExpr = (InvocationExpressionSyntax)context.Node;

                if (invocationExpr.ArgumentList is not ArgumentListSyntax argumentList)
                {
                    return;
                }

                if (argumentList.Arguments.Count != 2)
                {
                    return;
                }

                if (argumentList.Arguments[1] is not ArgumentSyntax repeatArgument)
                {
                    return;
                }

                if (repeatArgument.Expression is not LiteralExpressionSyntax literal)
                {
                    return;
                }

                if (literal.Token.Value as int? != 1)
                {
                    return;
                }

                context.ReportDiagnostic(CreateDiagnostic(invocationExpr));
            }
        }

        private static Diagnostic CreateDiagnostic(InvocationExpressionSyntax invocationExpr)
        {
            return Diagnostic.Create(Rule, invocationExpr.GetLocation());
        }
    }
}
