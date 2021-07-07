using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using static Funcky.Analyzer.LocalizedResourceLoader;
using static Funcky.Analyzer.Resources;

namespace Funcky.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class EnumerableRepeatOnceAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = nameof(EnumerableRepeatOnceAnalyzer);
        private const string Category = nameof(Funcky);
        private const int SecondArgument = 1;

        private static readonly LocalizableString Title = LoadFromResource(nameof(EnumerableRepeatOnceAnalyzerTitle));
        private static readonly LocalizableString MessageFormat = LoadFromResource(nameof(EnumerableRepeatOnceAnalyzerMessageFormat));
        private static readonly LocalizableString Description = LoadFromResource(nameof(EnumerableRepeatOnceAnalyzerDescription));

        private static readonly DiagnosticDescriptor Rule = new(nameof(EnumerableRepeatOnceAnalyzer), Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxNodeAction(FindEnumerableRepeateOnce, SyntaxKind.InvocationExpression);
        }

        private void FindEnumerableRepeateOnce(SyntaxNodeAnalysisContext context)
        {
            var syntax = new SyntaxMatcher(context);

            if (syntax.MatchStaticCall(nameof(Enumerable), nameof(Enumerable.Repeat))
                && syntax.MatchArgument(SecondArgument, 1))
            {
                context.ReportDiagnostic(CreateDiagnostic(context));
            }
        }

        private static Diagnostic CreateDiagnostic(SyntaxNodeAnalysisContext context)
        {
            var invocationExpr = (InvocationExpressionSyntax)context.Node;

            return Diagnostic.Create(Rule, invocationExpr.GetLocation());
        }
    }
}
