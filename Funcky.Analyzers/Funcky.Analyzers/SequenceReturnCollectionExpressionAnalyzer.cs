using System.Collections.Immutable;
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
        id: $"{DiagnosticName.Prefix}{DiagnosticName.Usage}11",
        title: "Sequence.Return can be replaced with a collection expression",
        messageFormat: "Replace 'Sequence.Return' with a collection expression",
        category: nameof(Funcky),
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

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
            context.RegisterOperationAction(AnalyzeInvocation(sequenceType, collectionTargetTypes), OperationKind.Invocation);
        }
    }

    private static Action<OperationAnalysisContext> AnalyzeInvocation(INamedTypeSymbol sequenceType, ImmutableArray<INamedTypeSymbol> collectionTargetTypes)
        => context =>
        {
            var operation = (IInvocationOperation)context.Operation;
            if (MatchSingleElementSequenceReturn(operation, sequenceType)
                && SupportsCollectionExpressions(operation.Syntax.SyntaxTree)
                && IsInTargetTypedContext(operation.Syntax)
                && HasCollectionTargetType(operation, collectionTargetTypes))
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

    private static bool HasCollectionTargetType(IInvocationOperation operation, ImmutableArray<INamedTypeSymbol> collectionTargetTypes)
        => operation.SemanticModel?.GetTypeInfo(operation.Syntax).ConvertedType is { } convertedType
            && collectionTargetTypes.Any(targetType => SymbolEquals(convertedType.OriginalDefinition, targetType));

    // A collection expression needs a target type, so we only suggest the replacement in positions
    // that supply one. `var` is excluded since `var x = [...]` does not compile.
    private static bool IsInTargetTypedContext(SyntaxNode expression)
    {
        var node = expression;
        while (node.Parent is ParenthesizedExpressionSyntax parenthesized)
        {
            node = parenthesized;
        }

        return node.Parent switch
        {
            ArgumentSyntax => true,
            ReturnStatementSyntax => true,
            ArrowExpressionClauseSyntax => true,
            CastExpressionSyntax => true,
            AssignmentExpressionSyntax assignment => assignment.Right == node,
            EqualsValueClauseSyntax equalsValue => !IsImplicitlyTypedVariable(equalsValue),
            _ => false,
        };
    }

    private static bool IsImplicitlyTypedVariable(EqualsValueClauseSyntax equalsValue)
        => equalsValue.Parent is VariableDeclaratorSyntax { Parent: VariableDeclarationSyntax { Type.IsVar: true } };

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
