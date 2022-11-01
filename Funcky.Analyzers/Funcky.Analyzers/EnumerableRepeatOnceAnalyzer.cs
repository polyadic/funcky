using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.LocalizedResourceLoader;
using static Funcky.Analyzers.OperationMatching;
using static Funcky.Analyzers.Resources;

namespace Funcky.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EnumerableRepeatOnceAnalyzer : DiagnosticAnalyzer
{
    public const string ValueParameterIndexProperty = nameof(ValueParameterIndexProperty);

    public const string DiagnosticId = $"{DiagnosticName.Prefix}{DiagnosticName.Usage}01";
    private const string Category = nameof(Funcky);

    private static readonly LocalizableString Title = LoadFromResource(nameof(EnumerableRepeatOnceAnalyzerTitle));
    private static readonly LocalizableString MessageFormat = LoadFromResource(nameof(EnumerableRepeatOnceAnalyzerMessageFormat));
    private static readonly LocalizableString Description = LoadFromResource(nameof(EnumerableRepeatOnceAnalyzerDescription));

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            if (context.Compilation.GetEnumerableType() is { } enumerableType
                && context.Compilation.GetSequenceType() is not null)
            {
                context.RegisterOperationAction(FindEnumerableRepeatOnce(enumerableType), OperationKind.Invocation);
            }
        });
    }

    private static Action<OperationAnalysisContext> FindEnumerableRepeatOnce(INamedTypeSymbol enumerableType)
        => context =>
        {
            var operation = (IInvocationOperation)context.Operation;
            if (MatchRepeatOnce(operation, enumerableType, out var valueArgument))
            {
                context.ReportDiagnostic(CreateDiagnostic(operation, valueArgument));
            }
        };

    private static bool MatchRepeatOnce(
        IInvocationOperation operation,
        INamedTypeSymbol enumerableType,
        [NotNullWhen(true)] out IArgumentOperation? valueArgument)
    {
        valueArgument = null;
        return MatchMethod(operation, enumerableType, nameof(Enumerable.Repeat))
            && MatchArguments(operation, out valueArgument, AnyArgument,  out _, ConstantArgument(1));
    }

    private static Diagnostic CreateDiagnostic(IInvocationOperation operation, IArgumentOperation valueArgument)
        => Diagnostic.Create(
            Rule,
            operation.Syntax.GetLocation(),
            ImmutableDictionary<string, string?>.Empty.Add(ValueParameterIndexProperty, operation.Arguments.IndexOf(valueArgument).ToString()),
            valueArgument.Value.Syntax.ToString());
}
