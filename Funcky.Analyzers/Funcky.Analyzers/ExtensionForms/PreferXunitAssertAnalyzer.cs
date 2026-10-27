using System.Collections.Immutable;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.ExtensionForms.ExtensionMemberSupport;

namespace Funcky.Analyzers.ExtensionForms;

/// <summary>Points from <c>FunctionalAssert</c> to the same assertions as extension members of xunit's <c>Assert</c>:
/// <c>FunctionalAssert.Some(option)</c> is <c>Assert.Some(option)</c>.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferXunitAssertAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor PreferAssert = new(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.ExtensionForms}03",
        title: "Prefer the assertion on Assert over FunctionalAssert",
        messageFormat: "Replace 'FunctionalAssert.{0}' with 'Assert.{0}'",
        category: DiagnosticName.Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: DiagnosticName.HelpLink($"{DiagnosticName.Prefix}{DiagnosticName.ExtensionForms}03"));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(PreferAssert);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        if (context.Compilation.GetFunctionalAssertType() is { } functionalAssertType
            && context.Compilation.GetAssertExtensionsType() is { } assertExtensionsType
            && context.Compilation.GetXunitAssertType() is not null)
        {
            var expressionOfTType = context.Compilation.GetExpressionOfTType();
            context.RegisterOperationAction(AnalyzeInvocation(functionalAssertType, assertExtensionsType, expressionOfTType), OperationKind.Invocation);
        }
    }

    // Static extension members cannot be invoked in an expression tree, so FunctionalAssert stays the only form there.
    private static Action<OperationAnalysisContext> AnalyzeInvocation(INamedTypeSymbol functionalAssertType, INamedTypeSymbol assertExtensionsType, INamedTypeSymbol? expressionOfTType)
        => context =>
        {
            var operation = (IInvocationOperation)context.Operation;
            var semanticModel = operation.SemanticModel ?? throw new InvalidOperationException("Semantic model is never null for operations passed to an analyzer (according to docs)");

            if (SymbolEquals(operation.TargetMethod.ContainingType, functionalAssertType)
                && SupportsExtensionMembers(operation.Syntax.SyntaxTree)
                && HasMember(assertExtensionsType, operation.TargetMethod.Name)
                && !operation.Syntax.IsInExpressionTree(semanticModel, expressionOfTType, context.CancellationToken))
            {
                context.ReportDiagnostic(Diagnostic.Create(PreferAssert, operation.Syntax.GetLocation(), operation.TargetMethod.Name));
            }
        };
}
