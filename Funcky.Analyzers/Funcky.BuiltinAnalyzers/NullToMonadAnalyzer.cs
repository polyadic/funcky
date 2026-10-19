#pragma warning disable SA1010 // StyleCop support for collection expressions is missing
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.BuiltinAnalyzers;

/// <summary>Reports a <c>null</c> literal that becomes the value of an <c>Option&lt;T&gt;</c>, <c>Result&lt;T&gt;</c>
/// or <c>Either&lt;L, R&gt;</c>, either through the implicit conversion from the inner type or through one of the
/// factory methods. The monads reject <c>null</c> with an <c>ArgumentNullException</c>, so such code throws every time
/// it runs. With nullable reference types disabled the compiler does not warn about it.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NullToMonadAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = DiagnosticName.NullToMonadId;

    private static readonly DiagnosticDescriptor Descriptor = new(
        id: DiagnosticId,
        title: "A null literal is passed to Option, Result or Either",
        messageFormat: "{0} throws an ArgumentNullException for 'null'",
        category: "Funcky",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Option, Result and Either never hold null. The implicit conversion from the inner type and the factory methods (Some, Return, Ok, Error, Left, Right) throw an ArgumentNullException when given null, so a null literal in that position throws every time it runs. Use Option<T>.None for an empty option.",
        helpLinkUri: DiagnosticName.HelpLink(DiagnosticId));

    private static readonly SymbolDisplayFormat FactoryDisplayFormat = SymbolDisplayFormat.MinimallyQualifiedFormat
        .WithMemberOptions(SymbolDisplayMemberOptions.IncludeContainingType)
        .WithGenericsOptions(SymbolDisplayGenericsOptions.IncludeTypeParameters)
        .WithParameterOptions(SymbolDisplayParameterOptions.None);

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
            context.RegisterOperationAction(AnalyzeInvocation(NullRejectingFactories.Collect(context.Compilation)), OperationKind.Invocation);
        }
    }

    private static Action<OperationAnalysisContext> AnalyzeConversion(ImmutableArray<INamedTypeSymbol> monadTypes)
        => context
            =>
            {
                var conversion = (IConversionOperation)context.Operation;

                if (IsUserDefinedConversionTo(conversion, monadTypes) && conversion.Type is { } targetType)
                {
                    ReportNullLiterals(context, conversion.Operand, $"The conversion to {targetType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)}");
                }
            };

    private static Action<OperationAnalysisContext> AnalyzeInvocation(NullRejectingFactories factories)
        => context
            =>
            {
                var invocation = (IInvocationOperation)context.Operation;

                if (factories.Contains(invocation.TargetMethod))
                {
                    foreach (var argument in invocation.Arguments)
                    {
                        ReportNullLiterals(context, argument.Value, invocation.TargetMethod.ToDisplayString(FactoryDisplayFormat));
                    }
                }
            };

    private static void ReportNullLiterals(OperationAnalysisContext context, IOperation operand, string receiver)
    {
        foreach (var nullLiteral in FindNullLiterals(operand))
        {
            context.ReportDiagnostic(Diagnostic.Create(Descriptor, nullLiteral.Syntax.GetLocation(), receiver));
        }
    }

    private static bool IsUserDefinedConversionTo(IConversionOperation conversion, ImmutableArray<INamedTypeSymbol> monadTypes)
        => conversion.Conversion is { IsUserDefined: true, MethodSymbol.ContainingType: var operatorOwner }
           && monadTypes.Contains(operatorOwner.OriginalDefinition, SymbolEqualityComparer.Default);

    /// <summary>Walks the branches of the expression and yields every <c>null</c> literal that can end up as its value.
    /// Other expressions are opaque: whether a variable or a call produces <c>null</c> is a runtime question.</summary>
    private static IEnumerable<ILiteralOperation> FindNullLiterals(IOperation operation)
        => operation switch
        {
            ILiteralOperation { ConstantValue: { HasValue: true, Value: null } } nullLiteral => [nullLiteral],
            IConditionalOperation { WhenFalse: { } whenFalse } conditional => FindNullLiterals(conditional.WhenTrue).Concat(FindNullLiterals(whenFalse)),
            ISwitchExpressionOperation switchExpression => switchExpression.Arms.SelectMany(arm => FindNullLiterals(arm.Value)),
            ICoalesceOperation coalesce => FindNullLiterals(coalesce.WhenNull),
            IConversionOperation { Conversion.IsUserDefined: false } conversion => FindNullLiterals(conversion.Operand),
            IParenthesizedOperation parenthesized => FindNullLiterals(parenthesized.Operand),
            _ => [],
        };

    /// <summary>The static factory methods that throw an <c>ArgumentNullException</c> for a <c>null</c> argument.</summary>
    private sealed class NullRejectingFactories
    {
        private readonly ImmutableArray<(INamedTypeSymbol Type, string MethodName)> _factories;

        private NullRejectingFactories(ImmutableArray<(INamedTypeSymbol Type, string MethodName)> factories)
            => _factories = factories;

        public static NullRejectingFactories Collect(Compilation compilation)
            => new(ImmutableArray.CreateRange(
                new (INamedTypeSymbol? Type, string MethodName)[]
                {
                    (compilation.GetOptionType(), "Some"),
                    (compilation.GetOptionType(), "Return"),
                    (compilation.GetResultType(), "Ok"),
                    (compilation.GetResultType(), "Return"),
                    (compilation.GetResultOfTType(), "Error"),
                    (compilation.GetEitherOfTType(), "Left"),
                    (compilation.GetEitherOfTType(), "Right"),
                    (compilation.GetEitherOfLeftType(), "Return"),
                }
                .Where(factory => factory.Type is not null)
                .Select(factory => (factory.Type!, factory.MethodName))));

        public bool Contains(IMethodSymbol method)
            => method.IsStatic
               && _factories.Any(factory => factory.MethodName == method.Name && SymbolEqualityComparer.Default.Equals(factory.Type, method.ContainingType.OriginalDefinition));
    }
}
