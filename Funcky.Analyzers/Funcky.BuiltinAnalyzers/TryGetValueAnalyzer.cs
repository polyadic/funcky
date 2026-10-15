using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.BuiltinAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TryGetValueAnalyzer : DiagnosticAnalyzer
{
    public const string DisallowedTryGetId = $"{DiagnosticName.Prefix}{DiagnosticName.BuiltIn}01";

    private const string ComponentBaseTypeName = "Microsoft.AspNetCore.Components.ComponentBase";

    private const string RazorPageBaseTypeName = "Microsoft.AspNetCore.Mvc.Razor.RazorPageBase";

    private static readonly DiagnosticDescriptor Descriptor = new(
        id: DisallowedTryGetId,
        title: "Disallowed use of TryGetValue",
        messageFormat: "Disallowed use of TryGetValue",
        category: "Funcky",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "TryGetValue should be used carefully.",
        helpLinkUri: DiagnosticName.HelpLink(DisallowedTryGetId));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterCompilationStartAction(OnCompilationStarted);
    }

    private static void OnCompilationStarted(CompilationStartAnalysisContext context)
    {
        if (context.Compilation.GetOptionOfTType() is { } optionOfTType)
        {
            context.RegisterOperationAction(AnalyzeOperation(optionOfTType), OperationKind.Invocation, OperationKind.MethodReference);
        }
    }

    private static Action<OperationAnalysisContext> AnalyzeOperation(INamedTypeSymbol optionOfTType)
        => context
            =>
            {
                if (ShouldReportDiagnostic(context.Operation, optionOfTType) && !OriginatesInRazorComponent(context))
                {
                    context.ReportDiagnostic(Diagnostic.Create(Descriptor, context.Operation.Syntax.GetLocation()));
                }
            };

    private static bool ShouldReportDiagnostic(IOperation operation, INamedTypeSymbol optionOfTType)
        => operation switch
        {
            IInvocationOperation invocation => IsTryGetValueMethod(invocation.TargetMethod, optionOfTType) && !IsAllowedUsageOfTryGetValue(invocation.Syntax),
            IMethodReferenceOperation methodReference => IsTryGetValueMethod(methodReference.Method, optionOfTType),
            _ => throw new InvalidOperationException($"Internal error: Unexpectedly called with an unexpected operation '{operation.GetType()}'"),
        };

    private static bool IsTryGetValueMethod(IMethodSymbol method, INamedTypeSymbol optionOfTType)
        => method.MetadataName == WellKnownMemberNames.OptionOfT.TryGetValue
           && SymbolEqualityComparer.Default.Equals(method.ContainingType.OriginalDefinition, optionOfTType);

    /// <summary>
    /// <c>TryGetValue</c> is allowed anywhere inside a catch filter clause, inside the condition of a loop
    /// or inside the condition of an <c>if</c> statement that yields, as long as the call belongs to the same
    /// method, lambda or local function as that construct.
    /// </summary>
    private static bool IsAllowedUsageOfTryGetValue(SyntaxNode node)
        => node.AncestorsAndSelfWithinReturnableConstruct().Any(n
            => IsCatchFilterClause(n)
               || IsLoopCondition(n)
               || (IsIfConditionWithYield(n) && node.IsWithinIterator()));

    private static bool IsCatchFilterClause(SyntaxNode node)
        => node is CatchFilterClauseSyntax;

    private static bool IsLoopCondition(SyntaxNode node)
        => node.Parent switch
        {
            WhileStatementSyntax whileStatement => whileStatement.Condition == node,
            DoStatementSyntax doStatement => doStatement.Condition == node,
            ForStatementSyntax forStatement => forStatement.Condition == node,
            _ => false,
        };

    private static bool IsIfConditionWithYield(SyntaxNode node)
        => node.Parent is IfStatementSyntax ifStatement
           && ifStatement.Condition == node
           && ifStatement.ContainsYield();

    /// <summary>Razor components (<c>.razor</c>) and Razor pages and views (<c>.cshtml</c>) have no control flow constructs of their own.</summary>
    private static bool OriginatesInRazorComponent(OperationAnalysisContext context)
        => GetMappedFileExtension(context.Operation) switch
        {
            ".razor" => IsContainedInTypeDerivedFrom(context, ComponentBaseTypeName),
            ".cshtml" => IsContainedInTypeDerivedFrom(context, RazorPageBaseTypeName),
            _ => false,
        };

    private static string? GetMappedFileExtension(IOperation operation)
    {
        var mappedLineSpan = operation.Syntax.SyntaxTree.GetMappedLineSpan(operation.Syntax.Span);
        return mappedLineSpan.HasMappedPath ? Path.GetExtension(mappedLineSpan.Path) : null;
    }

    private static bool IsContainedInTypeDerivedFrom(OperationAnalysisContext context, string baseTypeMetadataName)
        => context.Compilation.GetTypeByMetadataName(baseTypeMetadataName) is { } baseType
           && GetBaseTypes(context.ContainingSymbol.ContainingType).Contains(baseType, SymbolEqualityComparer.Default);

    private static IEnumerable<INamedTypeSymbol> GetBaseTypes(INamedTypeSymbol type)
    {
        for (INamedTypeSymbol? baseType = type.BaseType; baseType is not null; baseType = baseType.BaseType)
        {
            yield return baseType;
        }
    }
}
