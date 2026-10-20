using System.Collections.Immutable;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;
using static Funcky.Analyzers.OperationMatching;

namespace Funcky.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SequenceReturnCollectionExpressionAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor UseCollectionExpression = new(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.Usage}13",
        title: "Sequence.Return can be replaced with a collection expression",
        messageFormat: "Replace 'Sequence.Return' with a collection expression",
        category: DiagnosticName.Category,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        helpLinkUri: DiagnosticName.HelpLink($"{DiagnosticName.Prefix}{DiagnosticName.Usage}13"));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(UseCollectionExpression);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        if (context.Compilation.GetSequenceType() is { } sequenceType)
        {
            var collectionTargetTypes = GetCollectionExpressionTargetTypes(context.Compilation);
            var expressionOfTType = context.Compilation.GetExpressionOfTType();
            context.RegisterOperationAction(AnalyzeInvocation(sequenceType, collectionTargetTypes, expressionOfTType), OperationKind.Invocation);
        }
    }

    private static Action<OperationAnalysisContext> AnalyzeInvocation(INamedTypeSymbol sequenceType, ImmutableArray<INamedTypeSymbol> collectionTargetTypes, INamedTypeSymbol? expressionOfTType)
        => context =>
        {
            var operation = (IInvocationOperation)context.Operation;
            var semanticModel = operation.SemanticModel ?? throw new InvalidOperationException("Semantic model is never null for operations passed to an analyzer (according to docs)");

            if (MatchSingleElementSequenceReturn(operation, sequenceType)
                && SupportsCollectionExpressions(operation.Syntax.SyntaxTree)
                && IsInTargetTypedContext(operation.Syntax)
                && !IsAssignedToDiscard(operation)
                && HasCollectionTargetTypeWithSameElementType(operation, collectionTargetTypes)
                && !operation.Syntax.IsInExpressionTree(semanticModel, expressionOfTType, context.CancellationToken)
                && ReplacementBindsToTheSameMember(operation, semanticModel, context.CancellationToken))
            {
                context.ReportDiagnostic(Diagnostic.Create(UseCollectionExpression, operation.Syntax.GetLocation()));
            }
        };

    // We deliberately only match the single-element overload: a collection expression `[x]` always
    // matches it, whereas the params overload may be called in normal form with an existing array
    // (`Sequence.Return(array)`), where `[array]` would change the meaning.
    private static bool MatchSingleElementSequenceReturn(IInvocationOperation operation, INamedTypeSymbol sequenceType)
        => MatchMethod(operation, sequenceType, MonadReturnMethodName)
            && operation.TargetMethod.Parameters is [{ IsParams: false }];

    // 1200 is the numeric value of LanguageVersion.CSharp12; we can't reference the enum member
    // directly because the analyzer is also compiled against Roslyn versions that predate it.
    private static bool SupportsCollectionExpressions(SyntaxTree syntaxTree)
        => syntaxTree.Options is CSharpParseOptions parseOptions
            && (int)parseOptions.LanguageVersion.MapSpecifiedToEffectiveVersion() >= 1200;

    // A discard takes the type of the expression, so `_ = [0]` has no target type.
    private static bool IsAssignedToDiscard(IOperation operation)
        => SkipConversions(operation.Parent) is ISimpleAssignmentOperation { Target: IDiscardOperation };

    private static IOperation? SkipConversions(IOperation? operation)
    {
        while (operation is IConversionOperation conversion)
        {
            operation = conversion.Parent;
        }

        return operation;
    }

    // The element type has to stay the same: `IEnumerable<object> x = Sequence.Return<Widget>(new())` constructs a
    // Widget, whereas `[new()]` would construct an object, and a user-defined conversion to TResult would disappear.
    private static bool HasCollectionTargetTypeWithSameElementType(IInvocationOperation operation, ImmutableArray<INamedTypeSymbol> collectionTargetTypes)
        => operation.SemanticModel?.GetTypeInfo(operation.Syntax).ConvertedType is INamedTypeSymbol { TypeArguments: [var elementType] } convertedType
            && collectionTargetTypes.Any(targetType => SymbolEquals(convertedType.OriginalDefinition, targetType))
            && operation.TargetMethod.TypeArguments is [var resultType]
            && SymbolEquals(elementType, resultType);

    // A collection expression needs a target type, so we only suggest the replacement in positions
    // that supply one. `var` is excluded since `var x = [...]` does not compile.
    private static bool IsInTargetTypedContext(SyntaxNode expression)
    {
        var node = SkipParentheses(expression);

        return node.Parent switch
        {
            ArgumentSyntax => true,
            ReturnStatementSyntax returnStatement => ReturnsFromFunctionWithDeclaredReturnType(returnStatement),
            ArrowExpressionClauseSyntax => true,
            CastExpressionSyntax => true,
            AssignmentExpressionSyntax assignment => assignment.Right == node,
            EqualsValueClauseSyntax equalsValue => !IsImplicitlyTypedVariable(equalsValue),
            _ => false,
        };
    }

    private static SyntaxNode SkipParentheses(SyntaxNode node)
    {
        while (node.Parent is ParenthesizedExpressionSyntax parenthesized)
        {
            node = parenthesized;
        }

        return node;
    }

    // A lambda's return type is inferred from its return statements unless the lambda itself is target-typed
    // (`var f = () => { return [0]; }` does not compile), so a return inside a lambda only counts when the
    // delegate type is written out next to the lambda.
    private static bool ReturnsFromFunctionWithDeclaredReturnType(ReturnStatementSyntax returnStatement)
        => returnStatement.Ancestors().FirstOrDefault(IsFunctionBoundary) switch
        {
            AnonymousFunctionExpressionSyntax lambda => HasDeclaredDelegateType(lambda),
            _ => true,
        };

    private static bool IsFunctionBoundary(SyntaxNode node)
        => node is AnonymousFunctionExpressionSyntax or MemberDeclarationSyntax or LocalFunctionStatementSyntax or AccessorDeclarationSyntax;

    private static bool HasDeclaredDelegateType(AnonymousFunctionExpressionSyntax lambda)
        => SkipParentheses(lambda).Parent switch
        {
            CastExpressionSyntax => true,
            AssignmentExpressionSyntax assignment => assignment.Right == SkipParentheses(lambda),
            EqualsValueClauseSyntax equalsValue => !IsImplicitlyTypedVariable(equalsValue),
            _ => false,
        };

    private static bool IsImplicitlyTypedVariable(EqualsValueClauseSyntax equalsValue)
        => equalsValue.Parent is VariableDeclaratorSyntax { Parent: VariableDeclarationSyntax { Type.IsVar: true } };

    // In argument position the replacement takes part in overload resolution and type inference, which can pick a
    // different overload (`Consume(int[])` over `Consume(IEnumerable<int>)`) or fail to infer a type parameter
    // (`Consume<T>(T value)`). Binding the call with the replacement in place catches both.
    private static bool ReplacementBindsToTheSameMember(IInvocationOperation operation, SemanticModel semanticModel, CancellationToken cancellationToken)
    {
        if (SkipParentheses(operation.Syntax).Parent is not ArgumentSyntax argument)
        {
            return true;
        }

        if (argument.Parent?.Parent is not ExpressionSyntax call || operation.Syntax is not InvocationExpressionSyntax { ArgumentList.Arguments: [{ } element] })
        {
            return false;
        }

        var replacement = call.ReplaceNode(operation.Syntax, SingleElementCollectionExpression.Create(element.Expression));
        var originalSymbol = semanticModel.GetSymbolInfo(call, cancellationToken).Symbol;
        var replacementSymbol = semanticModel.GetSpeculativeSymbolInfo(call.SpanStart, replacement, SpeculativeBindingOption.BindAsExpression).Symbol;

        return originalSymbol is not null && SymbolEquals(originalSymbol, replacementSymbol);
    }

    // `Sequence.Return` returns IReadOnlyList<T>, so the only target types the existing code can
    // already convert to (and that collection expressions support) are these read-only interfaces.
    private static ImmutableArray<INamedTypeSymbol> GetCollectionExpressionTargetTypes(Compilation compilation)
        => new[]
            {
                compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T),
                compilation.GetTypeByMetadataName("System.Collections.Generic.IReadOnlyCollection`1"),
                compilation.GetTypeByMetadataName("System.Collections.Generic.IReadOnlyList`1"),
            }
            .OfType<INamedTypeSymbol>()
            .ToImmutableArray();
}
