using System.Collections.Immutable;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.ExtensionForms.ExtensionMemberSupport;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers.ExtensionForms;

/// <summary>Points from the constructors of <c>Funcky.Sequence</c> to their static extension members on
/// <c>System.Linq.Enumerable</c>: <c>Sequence.Successors(1, n => n + 1)</c> is <c>Enumerable.Successors(1, n => n + 1)</c>.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferEnumerableSequenceConstructorsAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor PreferEnumerable = new(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.ExtensionForms}02",
        title: "Prefer the Enumerable form of the Sequence constructor",
        messageFormat: "Replace 'Sequence.{0}' with 'Enumerable.{0}'",
        category: DiagnosticName.Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: DiagnosticName.HelpLink($"{DiagnosticName.Prefix}{DiagnosticName.ExtensionForms}02"));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(PreferEnumerable);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        if (context.Compilation.GetSequenceType() is { } sequenceType
            && context.Compilation.GetEnumerableExtensionType() is { } enumerableExtensionsType
            && context.Compilation.GetEnumerableType() is { } enumerableType)
        {
            var expressionOfTType = context.Compilation.GetExpressionOfTType();
            context.RegisterOperationAction(AnalyzeInvocation(sequenceType, enumerableExtensionsType, enumerableType, expressionOfTType), OperationKind.Invocation);
        }
    }

    private static Action<OperationAnalysisContext> AnalyzeInvocation(INamedTypeSymbol sequenceType, INamedTypeSymbol enumerableExtensionsType, INamedTypeSymbol enumerableType, INamedTypeSymbol? expressionOfTType)
        => context =>
        {
            var operation = (IInvocationOperation)context.Operation;
            var semanticModel = operation.SemanticModel ?? throw new InvalidOperationException("Semantic model is never null for operations passed to an analyzer (according to docs)");

            if (SymbolEquals(operation.TargetMethod.ContainingType, sequenceType)
                && SupportsExtensionMembers(operation.Syntax.SyntaxTree)
                && HasMember(enumerableExtensionsType, operation.TargetMethod.Name)
                && operation.Syntax is InvocationExpressionSyntax invocation
                && EnumerableFormBinds(invocation, semanticModel, enumerableExtensionsType, enumerableType)
                && !operation.Syntax.IsInExpressionTree(semanticModel, expressionOfTType, context.CancellationToken))
            {
                context.ReportDiagnostic(Diagnostic.Create(PreferEnumerable, operation.Syntax.GetLocation(), operation.TargetMethod.Name));
            }
        };

    // The Enumerable form is the same call with `Enumerable` as the receiver. It is reported when that call binds to the
    // Enumerable block of EnumerableExtensions, or when it binds to nothing at all because `Funcky.Extensions` is not
    // imported yet, which the code fix takes care of. A call that binds elsewhere (an overload of Enumerable itself) is left alone.
    private static bool EnumerableFormBinds(InvocationExpressionSyntax invocation, SemanticModel semanticModel, INamedTypeSymbol enumerableExtensionsType, INamedTypeSymbol enumerableType)
    {
        if (EnumerableForm(invocation, enumerableType) is not { } replacement)
        {
            return false;
        }

        var symbolInfo = semanticModel.GetSpeculativeSymbolInfo(invocation.SpanStart, replacement, SpeculativeBindingOption.BindAsExpression);
        return symbolInfo.Symbol is { } symbol
            ? SymbolEquals(DeclaringClass(symbol), enumerableExtensionsType)
            : symbolInfo.CandidateSymbols.IsEmpty
                && !IsVisible(enumerableExtensionsType, semanticModel, invocation.SpanStart)
                && HasStaticMember(enumerableExtensionsType, invocation.Expression switch { MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.ValueText, SimpleNameSyntax name => name.Identifier.ValueText, _ => string.Empty });
    }

    // When the class is visible by its simple name, `Funcky.Extensions` is imported, so a call that binds to nothing has no static form.
    private static bool IsVisible(INamedTypeSymbol type, SemanticModel semanticModel, int position)
        => semanticModel.LookupNamespacesAndTypes(position, name: type.Name).Any(candidate => SymbolEquals(candidate, type));

    private static InvocationExpressionSyntax? EnumerableForm(InvocationExpressionSyntax invocation, INamedTypeSymbol enumerableType)
    {
        var enumerable = ParseExpression(enumerableType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        return invocation.Expression switch
        {
            MemberAccessExpressionSyntax memberAccess => invocation.WithExpression(memberAccess.WithExpression(enumerable)),
            SimpleNameSyntax name => invocation.WithExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, enumerable, name)),
            _ => null,
        };
    }
}
