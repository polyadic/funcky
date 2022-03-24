using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.LocalizedResourceLoader;
using static Funcky.Analyzers.Resources;

namespace Funcky.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class EnumerableRepeatNeverAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = $"{DiagnosticName.Prefix}{DiagnosticName.Usage}02";
        private const string Category = nameof(Funcky);

        private static readonly LocalizableString Title = LoadFromResource(nameof(EnumerableRepeatNeverAnalyzerTitle));
        private static readonly LocalizableString MessageFormat = LoadFromResource(nameof(EnumerableRepeatNeverAnalyzerMessageFormat));
        private static readonly LocalizableString Description = LoadFromResource(nameof(EnumerableRepeatNeverAnalyzerDescription));

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
                    context.RegisterOperationAction(FindEnumerableRepeatNever(enumerableType), OperationKind.Invocation);
                }
            });
        }

        private static Action<OperationAnalysisContext> FindEnumerableRepeatNever(INamedTypeSymbol enumerableType)
            => context =>
            {
                var operation = (IInvocationOperation)context.Operation;

                if (IsRepeatNever(enumerableType, operation))
                {
                    context.ReportDiagnostic(CreateDiagnostic(operation));
                }
            };

        private static bool IsRepeatNever(INamedTypeSymbol enumerableType, IInvocationOperation operation)
            => SymbolEqualityComparer.Default.Equals(operation.TargetMethod.ContainingType, enumerableType)
                && operation.TargetMethod.Name == nameof(Enumerable.Repeat)
                && operation.Arguments.Length is 2
                && operation.Arguments[Argument.Second] is { Value.ConstantValue: { HasValue: true, Value: 0 } };

        private static Diagnostic CreateDiagnostic(IInvocationOperation operation)
        {
            var argumentValue = operation.Arguments[Argument.First].Value;
            return Diagnostic.Create(
                Rule,
                operation.Syntax.GetLocation(),
                argumentValue.Syntax.ToString(),
                argumentValue.Type?.ToDisplayString());
        }
    }
}
