#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.BuiltinAnalyzers;

/// <summary>Reports a <c>default</c> literal in a branch of a conditional, switch or null-coalescing expression
/// whose result is implicitly converted to <c>Option&lt;T&gt;</c>, <c>Result&lt;T&gt;</c> or <c>Either&lt;L, R&gt;</c>.
/// The literal takes the type of the other branches, so it is <c>default(T)</c> wrapped in a <c>Some</c>/<c>Ok</c>/<c>Right</c>
/// and not the empty monad the author expected.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DefaultInConversionAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = DiagnosticName.DefaultInConversionId;

    private static readonly DiagnosticDescriptor Descriptor = new(
        id: DiagnosticId,
        title: "The default literal is wrapped by an implicit conversion",
        messageFormat: "'default' evaluates to default({0}) here, which is then converted to {1}",
        category: DiagnosticName.Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "In a conditional, switch or null-coalescing expression the 'default' literal takes the type of the other branch. The implicit conversion then wraps that default value, it does not produce an empty Option, an Error or a Left.",
        helpLinkUri: DiagnosticName.HelpLink(DiagnosticId));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterCompilationStartAction(OnCompilationStarted);
    }

    private static void OnCompilationStarted(CompilationStartAnalysisContext context)
    {
        var monadTypes = context.Compilation.GetImplicitlyConvertibleMonadTypes();

        if (monadTypes.Length > 0)
        {
            context.RegisterOperationAction(AnalyzeConversion(monadTypes), OperationKind.Conversion);
        }
    }

    private static Action<OperationAnalysisContext> AnalyzeConversion(ImmutableArray<INamedTypeSymbol> monadTypes)
        => context
            =>
            {
                var conversion = (IConversionOperation)context.Operation;

                if (IsUserDefinedConversionTo(conversion, monadTypes) && conversion.Type is { } targetType)
                {
                    foreach (var defaultLiteral in FindDefaultLiterals(conversion.Operand))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            Descriptor,
                            defaultLiteral.Syntax.GetLocation(),
                            defaultLiteral.Type?.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat),
                            targetType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));
                    }
                }
            };

    private static bool IsUserDefinedConversionTo(IConversionOperation conversion, ImmutableArray<INamedTypeSymbol> monadTypes)
        => conversion.Conversion is { IsUserDefined: true, MethodSymbol.ContainingType: var operatorOwner }
           && monadTypes.Contains(operatorOwner.OriginalDefinition, SymbolEqualityComparer.Default);

    /// <summary>Walks the branches of the converted expression and yields every <c>default</c> literal that ends up
    /// as the value of the expression. Other expressions are opaque: they either have an explicit type or the author
    /// wrote something more deliberate than a bare literal. A literal is only reported once a branch has been entered:
    /// outside a branch, as in <c>(int)default</c>, the cast names the type and the literal is as explicit as <c>default(int)</c>.</summary>
    private static IEnumerable<IDefaultValueOperation> FindDefaultLiterals(IOperation operation, bool insideBranch = false)
        => operation switch
        {
            IDefaultValueOperation defaultValue when insideBranch && IsDefaultLiteral(defaultValue) => [defaultValue],
            IConditionalOperation { WhenFalse: { } whenFalse } conditional => FindDefaultLiterals(conditional.WhenTrue, insideBranch: true).Concat(FindDefaultLiterals(whenFalse, insideBranch: true)),
            ISwitchExpressionOperation switchExpression => switchExpression.Arms.SelectMany(arm => FindDefaultLiterals(arm.Value, insideBranch: true)),
            ICoalesceOperation coalesce => FindDefaultLiterals(coalesce.WhenNull, insideBranch: true),
            IConversionOperation { Conversion.IsUserDefined: false } conversion => FindDefaultLiterals(conversion.Operand, insideBranch),
            IParenthesizedOperation parenthesized => FindDefaultLiterals(parenthesized.Operand, insideBranch),
            _ => [],
        };

    // A typed default(T) is explicit about the value it produces; only the bare literal silently takes the type of its sibling branch.
    private static bool IsDefaultLiteral(IDefaultValueOperation defaultValue)
        => defaultValue.Syntax.IsKind(SyntaxKind.DefaultLiteralExpression);
}
