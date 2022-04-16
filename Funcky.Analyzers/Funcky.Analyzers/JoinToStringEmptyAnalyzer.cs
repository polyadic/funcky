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
public sealed class JoinToStringEmptyAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = $"{DiagnosticName.Prefix}{DiagnosticName.Usage}04";
    private const string Category = nameof(Funcky);
    private const string JoinToString = "JoinToString";

    private static readonly LocalizableString Title = LoadFromResource(nameof(JoinToStringEmptyAnalyzerTitle));

    private static readonly LocalizableString MessageFormat = LoadFromResource(nameof(JoinToStringEmptyAnalyzerMessageFormat));

    private static readonly LocalizableString Description = LoadFromResource(nameof(JoinToStringEmptyAnalyzerDescription));

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    private static Func<IArgumentOperation, bool> IsEmptyStringConstant { get; } = ConstantArgument(string.Empty);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(static context =>
        {
            if (context.Compilation.GetEnumerableExtensionType() is { } enumerableType && context.Compilation.GetStringType() is { } stringType)
            {
                context.RegisterOperationAction(FindJoinToStringEmpty(new Symbols(enumerableType, stringType)), OperationKind.Invocation);
            }
        });
    }

    private static Action<OperationAnalysisContext> FindJoinToStringEmpty(Symbols symbols)
        => context =>
        {
            var operation = (IInvocationOperation)context.Operation;

            if (MatchJoinToStringEmpty(symbols, operation, out var valueArgument, out var stringArgument))
            {
                context.ReportDiagnostic(CreateDiagnostic(operation, valueArgument, stringArgument));
            }
        };

    private static bool MatchJoinToStringEmpty(Symbols symbols, IInvocationOperation operation, [NotNullWhen(true)] out IArgumentOperation? valueArgument, [NotNullWhen(true)] out IArgumentOperation? stringArgument)
    {
        valueArgument = null;
        stringArgument = null;
        return MatchMethod(operation, symbols.EnumerableType, JoinToString)
               && MatchArguments(operation, out valueArgument, AnyArgument, out stringArgument, IsEmptyString(symbols.StringType));
    }

    private static Func<IArgumentOperation, bool> IsEmptyString(INamedTypeSymbol stringType)
        => argument
            => IsEmptyStringConstant(argument) || IsStringEmptyField(stringType, argument);

    private static bool IsStringEmptyField(INamedTypeSymbol stringType, IArgumentOperation argument)
        => argument.Value is IFieldReferenceOperation fieldReferenceOperation && MatchField(fieldReferenceOperation, stringType, nameof(string.Empty));

    private static Diagnostic CreateDiagnostic(IOperation operation, IArgumentOperation valueArgument, IArgumentOperation stringArgument)
        => Diagnostic.Create(
            Rule,
            operation.Syntax.GetLocation(),
            valueArgument.Value.Syntax.ToString(),
            stringArgument.Value.Syntax.ToString());

    private sealed record Symbols(INamedTypeSymbol EnumerableType, INamedTypeSymbol StringType);
}
