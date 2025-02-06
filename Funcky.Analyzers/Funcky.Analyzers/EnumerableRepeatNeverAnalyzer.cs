using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.LocalizedResourceLoader;
using static Funcky.Analyzers.OperationMatching;
using static Funcky.Analyzers.Resources;

namespace Funcky.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EnumerableRepeatNeverAnalyzer : DiagnosticAnalyzer
{
    public const string ValueParameterIndexProperty = nameof(ValueParameterIndexProperty);

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
            if (context.Compilation.GetEnumerableType() is { } enumerableType)
            {
                context.RegisterOperationAction(FindEnumerableRepeatNever(enumerableType), OperationKind.Invocation);
            }
        });
    }

    private static Action<OperationAnalysisContext> FindEnumerableRepeatNever(INamedTypeSymbol enumerableType)
        => context =>
        {
            var operation = (IInvocationOperation)context.Operation;

            if (MatchRepeatNever(enumerableType, operation, out var valueArgument))
            {
                context.ReportDiagnostic(CreateDiagnostic(operation, valueArgument));
            }
        };

    private static bool MatchRepeatNever(
        INamedTypeSymbol enumerableType,
        IInvocationOperation operation,
        [NotNullWhen(true)] out IArgumentOperation? valueArgument)
    {
        valueArgument = null;
        return MatchMethod(operation, enumerableType, nameof(Enumerable.Repeat))
            && operation.GetArgumentsInParameterOrder() is [var valueArgument_, var countArgument]
            && MatchConstantArgument(countArgument, 0)
            && (valueArgument = valueArgument_) is var _;
    }

    private static Diagnostic CreateDiagnostic(IInvocationOperation operation, IArgumentOperation valueArgument)
        => Diagnostic.Create(
            Rule,
            operation.Syntax.GetLocation(),
            ImmutableDictionary<string, string?>.Empty.Add(ValueParameterIndexProperty, operation.Arguments.IndexOf(valueArgument).ToString()),
            valueArgument.Value.Syntax.ToString(),
            valueArgument.Value.Type?.ToDisplayString());
}
