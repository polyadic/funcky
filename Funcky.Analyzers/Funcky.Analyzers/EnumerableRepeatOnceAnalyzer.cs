using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.LocalizedResourceLoader;
using static Funcky.Analyzers.Resources;

namespace Funcky.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class EnumerableRepeatOnceAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = $"{DiagnosticName.Prefix}{DiagnosticName.Usage}01";
        private const string Category = nameof(Funcky);

        private static readonly LocalizableString Title = LoadFromResource(nameof(EnumerableRepeatOnceAnalyzerTitle));
        private static readonly LocalizableString MessageFormat = LoadFromResource(nameof(EnumerableRepeatOnceAnalyzerMessageFormat));
        private static readonly LocalizableString Description = LoadFromResource(nameof(EnumerableRepeatOnceAnalyzerDescription));

        private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterCompilationStartAction(static context =>
            {
                if (context.Compilation.GetTypeByMetadataName(typeof(Enumerable).FullName) is { } enumerableType)
                {
                    context.RegisterOperationAction(FindEnumerableRepeatOnce(enumerableType), OperationKind.Invocation);
                }
            });
        }

        private static Action<OperationAnalysisContext> FindEnumerableRepeatOnce(INamedTypeSymbol enumerableType)
            => context =>
            {
                var operation = (IInvocationOperation)context.Operation;
                if (IsRepeatOnce(operation, enumerableType))
                {
                    context.ReportDiagnostic(CreateDiagnostic(operation));
                }
            };

        private static bool IsRepeatOnce(IInvocationOperation operation, INamedTypeSymbol enumerableType)
            => SymbolEqualityComparer.Default.Equals(operation.TargetMethod.ContainingType, enumerableType)
                && operation.TargetMethod.Name == nameof(Enumerable.Repeat)
                && operation.Arguments.Length is 2
                && operation.Arguments[Argument.Second] is { Value.ConstantValue: { HasValue: true, Value: 1 } };

        private static Diagnostic CreateDiagnostic(IInvocationOperation operation)
        {
            var argumentValue = operation.Arguments[Argument.First].Value;
            return Diagnostic.Create(
                Rule,
                operation.Syntax.GetLocation(),
                argumentValue.Syntax.ToString());
        }
    }
}
