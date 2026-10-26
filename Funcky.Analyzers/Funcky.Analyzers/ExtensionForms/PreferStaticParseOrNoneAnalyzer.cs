using System.Collections.Immutable;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.ExtensionForms.ExtensionMemberSupport;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers.ExtensionForms;

/// <summary>Points from the <c>Parse…OrNone</c> extensions on <c>string</c> and spans to the static extension member on
/// the parsed type: <c>text.ParseInt32OrNone()</c> is <c>int.ParseOrNone(text)</c>.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferStaticParseOrNoneAnalyzer : DiagnosticAnalyzer
{
    public const string TargetTypeProperty = nameof(TargetTypeProperty);

    public const string StaticMemberNameProperty = nameof(StaticMemberNameProperty);

    public static readonly DiagnosticDescriptor PreferStaticParseOrNone = new(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.ExtensionForms}01",
        title: "Prefer the static ParseOrNone member on the parsed type",
        messageFormat: "Replace '{0}' with '{1}.{2}'",
        category: DiagnosticName.Category,
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        helpLinkUri: DiagnosticName.HelpLink($"{DiagnosticName.Prefix}{DiagnosticName.ExtensionForms}01"));

    private const string ParsePrefix = "Parse";

    private const string ExactInfix = "Exact";

    private const string OrNoneSuffix = "OrNone";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(PreferStaticParseOrNone);

    /// <summary>The name of the static member that replaces the string extension: <c>ParseOrNone</c> or <c>ParseExactOrNone</c>.</summary>
    public static string StaticMemberName(string extensionMethodName)
        => extensionMethodName.Contains(ExactInfix)
            ? $"{ParsePrefix}{ExactInfix}{OrNoneSuffix}"
            : $"{ParsePrefix}{OrNoneSuffix}";

    /// <summary>Builds <c>TargetType.ParseOrNone(receiver, arguments…)</c> from the invocation of the string extension.</summary>
    public static InvocationExpressionSyntax? StaticInvocation(IInvocationOperation operation, string targetTypeName, string staticMemberName)
    {
        if (operation.Syntax is not InvocationExpressionSyntax invocation || Arguments(operation) is not { } arguments)
        {
            return null;
        }

        return InvocationExpression(
            ParseExpression($"{targetTypeName}.{staticMemberName}"),
            ArgumentList(SeparatedList(arguments)).WithTriviaFrom(invocation.ArgumentList));
    }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        if (context.Compilation.GetParseExtensionsType() is { } parseExtensionsType
            && context.Compilation.GetOptionOfTType() is { } optionOfTType)
        {
            var expressionOfTType = context.Compilation.GetExpressionOfTType();
            context.RegisterOperationAction(AnalyzeInvocation(parseExtensionsType, optionOfTType, expressionOfTType), OperationKind.Invocation);
        }
    }

    private static Action<OperationAnalysisContext> AnalyzeInvocation(INamedTypeSymbol parseExtensionsType, INamedTypeSymbol optionOfTType, INamedTypeSymbol? expressionOfTType)
        => context =>
        {
            var operation = (IInvocationOperation)context.Operation;
            var semanticModel = operation.SemanticModel ?? throw new InvalidOperationException("Semantic model is never null for operations passed to an analyzer (according to docs)");

            if (IsParseOrNoneExtension(operation.TargetMethod, parseExtensionsType)
                && SupportsExtensionMembers(operation.Syntax.SyntaxTree)
                && TargetType(operation.TargetMethod, optionOfTType) is { } targetType
                && !operation.Syntax.IsInExpressionTree(semanticModel, expressionOfTType, context.CancellationToken)
                && StaticMemberName(operation.TargetMethod.Name) is var staticMemberName
                && StaticInvocation(operation, targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat), staticMemberName) is { } replacement
                && BindsToStaticParseMember(replacement, operation, semanticModel, parseExtensionsType))
            {
                var properties = ImmutableDictionary<string, string?>.Empty
                    .Add(TargetTypeProperty, targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
                    .Add(StaticMemberNameProperty, staticMemberName);
                context.ReportDiagnostic(Diagnostic.Create(
                    PreferStaticParseOrNone,
                    operation.Syntax.GetLocation(),
                    properties,
                    messageArgs: [operation.TargetMethod.Name, targetType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat), staticMemberName]));
            }
        };

    private static bool IsParseOrNoneExtension(IMethodSymbol method, INamedTypeSymbol parseExtensionsType)
        => SymbolEquals((method.ReducedFrom ?? method).ContainingType, parseExtensionsType)
           && method.Name.StartsWith(ParsePrefix, StringComparison.Ordinal)
           && method.Name.EndsWith(OrNoneSuffix, StringComparison.Ordinal);

    // The parsed type is the type argument of the returned Option<T>; a type parameter of the enclosing method cannot be a receiver.
    private static ITypeSymbol? TargetType(IMethodSymbol method, INamedTypeSymbol optionOfTType)
        => method.ReturnType is INamedTypeSymbol { TypeArguments: [{ TypeKind: not TypeKind.TypeParameter and not TypeKind.Error } targetType] } returnType
           && SymbolEquals(returnType.OriginalDefinition, optionOfTType)
            ? targetType.WithNullableAnnotation(NullableAnnotation.NotAnnotated)
            : null;

    // The replacement must bind to one of the generated static parse classes (Int32ParseExtensions, …), not back to the string extension.
    private static bool BindsToStaticParseMember(InvocationExpressionSyntax replacement, IInvocationOperation operation, SemanticModel semanticModel, INamedTypeSymbol parseExtensionsType)
        => semanticModel.GetSpeculativeSymbolInfo(operation.Syntax.SpanStart, replacement, SpeculativeBindingOption.BindAsExpression).Symbol is { } symbol
           && DeclaringClass(symbol) is { } declaringClass
           && !SymbolEquals(declaringClass, parseExtensionsType)
           && IsDeclaredInFunckyExtensions(symbol);

    // The receiver becomes the first argument; the IOperation already lists it as the first argument of the lowered static
    // call. The arguments keep their source order, so named arguments stay where the author put them.
    private static IEnumerable<ArgumentSyntax>? Arguments(IInvocationOperation operation)
    {
        var arguments = new List<ArgumentSyntax>();

        if (operation.Instance is { Syntax: ExpressionSyntax receiver })
        {
            arguments.Add(Argument(receiver.WithoutTrivia()));
        }

        foreach (var argument in operation.Arguments)
        {
            switch (argument)
            {
                case { ArgumentKind: ArgumentKind.DefaultValue }:
                    continue;
                case { Syntax: ArgumentSyntax syntax }:
                    arguments.Add(arguments.Count == 0 ? syntax.WithExpression(syntax.Expression.WithoutTrivia()).WithNameColon(null) : syntax);
                    break;
                case { Syntax: ExpressionSyntax receiverExpression } when arguments.Count == 0:
                    arguments.Add(Argument(receiverExpression.WithoutTrivia()));
                    break;
                default:
                    return null;
            }
        }

        return arguments;
    }
}
