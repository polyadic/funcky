using System.Collections.Immutable;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.Functions.NamedFunctionMatching;

namespace Funcky.Analyzers.Functions;

/// <summary>Reports a lambda that spells out one of the named functions in <c>Funcky.Functional</c>:
/// <c>x => x</c> is <c>Identity</c>, <c>_ => true</c> is <c>True</c>, <c>_ => false</c> is <c>False</c> and <c>() => { }</c> is <c>NoOperation</c>.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferNamedFunctionAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = $"{DiagnosticName.Prefix}{DiagnosticName.Usage}17";

    public const string FunctionNameProperty = nameof(FunctionNameProperty);

    private const string FunctionalTypeName = "Funcky.Functional";

    private static readonly DiagnosticDescriptor Descriptor = new(
        id: DiagnosticId,
        title: "Prefer the named function over the lambda",
        messageFormat: "Prefer '{0}' over this lambda",
        category: DiagnosticName.Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Funcky.Functional names the functions that are written as lambdas over and over: Identity, True, False and NoOperation. The name says what the function does, where the lambda has to be read. Before C# 11 a method group is not cached by the compiler, so the rule reports an info there.",
        helpLinkUri: DiagnosticName.HelpLink(DiagnosticId));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Descriptor);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        if (context.Compilation.GetFunctionalType() is { } functionalType)
        {
            var expressionOfTType = context.Compilation.GetExpressionOfTType();
            context.RegisterOperationAction(context => AnalyzeAnonymousFunction(context, functionalType, expressionOfTType), OperationKind.AnonymousFunction);
        }
    }

    private static void AnalyzeAnonymousFunction(OperationAnalysisContext context, INamedTypeSymbol functionalType, INamedTypeSymbol? expressionOfTType)
    {
        var anonymousFunction = (IAnonymousFunctionOperation)context.Operation;

        if (anonymousFunction is { Parent: IDelegateCreationOperation { Type: { } delegateType }, Syntax: AnonymousFunctionExpressionSyntax syntax, SemanticModel: { } semanticModel }
            && MatchReplaceableFunction(anonymousFunction, functionalType) is { } namedFunction
            && !syntax.IsInExpressionTree(semanticModel, expressionOfTType, context.CancellationToken)
            && SpeculativeLambdaReplacement.BindsIdentically(semanticModel, syntax, QualifiedName(namedFunction), delegateType, SiblingReplacements(syntax, semanticModel, functionalType, context.CancellationToken), context.CancellationToken))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                Descriptor,
                syntax.GetLocation(),
                EffectiveSeverity(syntax.SyntaxTree),
                additionalLocations: null,
                properties: ImmutableDictionary<string, string?>.Empty.Add(FunctionNameProperty, namedFunction.Name),
                namedFunction.Name));
        }
    }

    private static NamedFunction? MatchReplaceableFunction(IAnonymousFunctionOperation anonymousFunction, INamedTypeSymbol functionalType)
        => MatchNamedFunction(anonymousFunction) is { } namedFunction && HasOverload(functionalType, namedFunction)
            ? namedFunction
            : null;

    /// <summary>The other lambdas of the same call that this rule would report too; a batch fix replaces all of them together.</summary>
    private static ImmutableDictionary<AnonymousFunctionExpressionSyntax, ExpressionSyntax> SiblingReplacements(
        AnonymousFunctionExpressionSyntax anonymousFunction,
        SemanticModel semanticModel,
        INamedTypeSymbol functionalType,
        CancellationToken cancellationToken)
        => anonymousFunction.FirstAncestorOrSelf<ArgumentSyntax>()?.Parent is BaseArgumentListSyntax argumentList
            ? argumentList.Arguments
                .Select(argument => WalkDownParentheses(argument.Expression))
                .OfType<AnonymousFunctionExpressionSyntax>()
                .Where(sibling => sibling != anonymousFunction)
                .Select(sibling => (Sibling: sibling, Function: semanticModel.GetOperation(sibling, cancellationToken) is IAnonymousFunctionOperation operation ? MatchReplaceableFunction(operation, functionalType) : null))
                .Where(pair => pair.Function is not null)
                .ToImmutableDictionary(pair => pair.Sibling, pair => QualifiedName(pair.Function!))
            : ImmutableDictionary<AnonymousFunctionExpressionSyntax, ExpressionSyntax>.Empty;

    private static ExpressionSyntax WalkDownParentheses(ExpressionSyntax expression)
        => expression is ParenthesizedExpressionSyntax parenthesized
            ? WalkDownParentheses(parenthesized.Expression)
            : expression;

    private static bool HasOverload(INamedTypeSymbol functionalType, NamedFunction namedFunction)
        => functionalType
            .GetMembers(namedFunction.Name)
            .OfType<IMethodSymbol>()
            .Any(method => method.IsStatic && method.Parameters.Length == namedFunction.ParameterCount);

    private static ExpressionSyntax QualifiedName(NamedFunction namedFunction)
        => SyntaxFactory.ParseExpression($"global::{FunctionalTypeName}.{namedFunction.Name}");

    /// <summary>Since C# 11 the compiler caches the delegate created from a static method group, just as it does for a non-capturing lambda.
    /// Before that the method group allocates on every evaluation, so the suggestion is a trade-off and only an info.</summary>
    private static DiagnosticSeverity EffectiveSeverity(SyntaxTree syntaxTree)
        => syntaxTree.Options is CSharpParseOptions { LanguageVersion: >= LanguageVersion.CSharp11 }
            ? Descriptor.DefaultSeverity
            : DiagnosticSeverity.Info;
}
