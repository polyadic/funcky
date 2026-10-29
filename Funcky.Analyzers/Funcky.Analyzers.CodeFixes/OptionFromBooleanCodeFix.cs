using System.Collections.Immutable;
using System.Composition;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Simplification;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Funcky.Analyzers;

/// <summary>
/// Offers a fix for the obsoletion warning CS0618 on <c>Option.FromBoolean</c>, which <c>bool.Then</c> replaces:
/// <c>Option.FromBoolean(condition, value)</c> becomes <c>condition.Then(value)</c>, the overload with a selector keeps
/// its selector, <c>Option.FromBoolean(condition)</c> becomes <c>condition.Then(Unit.Value)</c>, and the flattening idiom
/// <c>Option.FromBoolean(condition, option).SelectMany(Identity)</c> becomes <c>condition.Then(() => option)</c>.
/// </summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OptionFromBooleanCodeFix))]
public sealed class OptionFromBooleanCodeFix : ExtensionMemberCodeFix
{
    /// <summary>The compiler diagnostic reported on the use of a member marked <c>[Obsolete]</c> with a message.</summary>
    private const string ObsoleteMemberDiagnosticId = "CS0618";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ObsoleteMemberDiagnosticId);

    // The document is rewritten up front: the fix is withheld when the Then call would not bind to Funcky's extension.
    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

        if (root is null || semanticModel is null)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            if (TryCreateRewrite(root, semanticModel, diagnostic, context.CancellationToken) is { } rewrite
                && await TryApplyAsync(context.Document, root, semanticModel, ImmutableArray.Create(rewrite), context.CancellationToken).ConfigureAwait(false) is { } changedDocument)
            {
                context.RegisterCodeFix(CodeAction.Create(GetTitle(rewrite), _ => Task.FromResult(changedDocument), nameof(OptionFromBooleanCodeFix)), diagnostic);
            }
        }
    }

    protected override string GetTitle(ExtensionMemberRewrite rewrite) => $"Replace Option.{OptionFromBooleanMethodName} with {BooleanThenMethodName}";

    // The warning spans the whole call; one with a smaller span belongs to an obsolete member inside an argument.
    protected override ExtensionMemberRewrite? TryCreateRewrite(SyntaxNode root, SemanticModel semanticModel, Diagnostic diagnostic, CancellationToken cancellationToken)
        => GetSymbols(semanticModel.Compilation) is { } symbols
           && root.FindInvocationExpression(diagnostic.Location.SourceSpan) is { } invocation
           && invocation.Span == diagnostic.Location.SourceSpan
           && semanticModel.GetOperation(invocation, cancellationToken) is IInvocationOperation { TargetMethod: { Name: OptionFromBooleanMethodName, IsStatic: true } method } operation
           && SymbolEqualityComparer.Default.Equals(method.ContainingType, symbols.OptionType)
           && Rewrite.TryCreate(operation, invocation, semanticModel, symbols, cancellationToken) is { } rewrite
            ? rewrite.ToExtensionMemberRewrite()
            : null;

    protected override Task<Document?> TryApplyAsync(Document document, SyntaxNode root, SemanticModel semanticModel, ImmutableArray<ExtensionMemberRewrite> rewrites, CancellationToken cancellationToken)
        => semanticModel.Compilation.GetBooleanExtensionsType() is { } booleanExtensionsType
            ? BooleanThen.TryReplaceAllAsync(document, root, semanticModel, rewrites, booleanExtensionsType, cancellationToken)
            : Task.FromResult<Document?>(null);

    private static Symbols? GetSymbols(Compilation compilation)
        => compilation.GetOptionType() is { } optionType
           && compilation.GetOptionOfTType() is { } optionOfTType
           && compilation.GetUnitType() is { } unitType
           && compilation.GetBooleanExtensionsType() is { } booleanExtensionsType
           && BooleanThen.HasThen(booleanExtensionsType)
            ? new Symbols(optionType, optionOfTType, unitType, booleanExtensionsType, compilation.GetFunctionalType())
            : null;

    private sealed record Symbols(INamedTypeSymbol OptionType, INamedTypeSymbol OptionOfTType, INamedTypeSymbol UnitType, INamedTypeSymbol BooleanExtensionsType, INamedTypeSymbol? FunctionalType);

    /// <summary>What to replace (the <c>FromBoolean</c> call, or the <c>SelectMany(Identity)</c> around it) and with what.</summary>
    private sealed record Rewrite(InvocationExpressionSyntax Target, ExpressionSyntax Condition, SimpleNameSyntax ThenName, ArgumentSyntax ThenArgument)
    {
        public static Rewrite? TryCreate(IInvocationOperation operation, InvocationExpressionSyntax invocation, SemanticModel semanticModel, Symbols symbols, CancellationToken cancellationToken)
        {
            if (GetMethodName(invocation) is not { } name || !KeepsEvaluationOrder(operation))
            {
                return null;
            }

            // The arguments are in evaluation order, which named arguments can change.
            return operation.Arguments.OrderBy(argument => argument.Parameter?.Ordinal ?? int.MaxValue).ToArray() switch
            {
                [{ Syntax: ArgumentSyntax condition }] => new Rewrite(invocation, condition.Expression, BooleanThen.ThenName(name), Argument(UnitValue(symbols))),
                [{ Syntax: ArgumentSyntax condition }, { Syntax: ArgumentSyntax argumentSyntax } argument] => TryCreate(operation, invocation, name, condition.Expression, argumentSyntax.Expression, argument.Value, semanticModel, symbols, cancellationToken),
                _ => null,
            };
        }

        public ExtensionMemberRewrite ToExtensionMemberRewrite()
            => new(Target, BooleanThen.Invocation(Condition, ThenName, ThenArgument).WithTriviaFrom(Target));

        private static Rewrite? TryCreate(IInvocationOperation operation, InvocationExpressionSyntax invocation, SimpleNameSyntax name, ExpressionSyntax condition, ExpressionSyntax argument, IOperation value, SemanticModel semanticModel, Symbols symbols, CancellationToken cancellationToken)
        {
            // The definition tells the overloads apart; the constructed parameter type is a delegate for the value overload too when TItem is one.
            var isSelectorOverload = operation.TargetMethod.OriginalDefinition.Parameters[1].Type.TypeKind == TypeKind.Delegate;
            var itemType = operation.TargetMethod.TypeArguments is [var typeArgument] ? typeArgument : null;
            var producesNestedOption = itemType is INamedTypeSymbol { OriginalDefinition: var itemTypeDefinition } && SymbolEqualityComparer.Default.Equals(itemTypeDefinition, symbols.OptionOfTType);

            return (FindSelectManyIdentity(invocation, semanticModel, symbols, cancellationToken), isSelectorOverload, producesNestedOption) switch
            {
                // Option.FromBoolean(condition, option).SelectMany(Identity) => condition.Then(() => option)
                ({ } selectMany, false, true) => CanMoveIntoLambda(value)
                    ? new Rewrite(selectMany, condition, FlattenedThenName(name, (INamedTypeSymbol)itemType!), Argument(ParenthesizedLambdaExpression(argument.WithoutTrivia())))
                    : null,

                // Option.FromBoolean(condition, () => option).SelectMany(Identity) => condition.Then(() => option)
                ({ } selectMany, true, true) => new Rewrite(selectMany, condition, FlattenedThenName(name, (INamedTypeSymbol)itemType!), Argument(argument)),

                // Option.FromBoolean(condition, () => option) yields an Option<Option<T>>, which Then would flatten.
                (_, true, true) => null,

                (_, true, false) => new Rewrite(invocation, condition, BooleanThen.ThenName(name), Argument(argument)),

                (_, false, _) => new Rewrite(invocation, condition, BooleanThen.ThenName(name), BooleanThen.ValueArgument(argument, itemType)),
            };
        }

        // Then evaluates the condition before the value. Named arguments given in the other order are only reordered when neither has an
        // effect the other could observe; the check is syntactic and errs on the side of keeping the call.
        private static bool KeepsEvaluationOrder(IInvocationOperation operation)
            => operation.Arguments.Select(argument => argument.Parameter?.Ordinal ?? int.MaxValue).ToArray() is var ordinals
               && (ordinals.SequenceEqual(ordinals.OrderBy(ordinal => ordinal))
                   || operation.Arguments.All(argument => argument.Syntax is ArgumentSyntax { Expression: var expression } && IsSideEffectFree(expression)));

        private static bool IsSideEffectFree(ExpressionSyntax expression)
            => expression switch
            {
                LiteralExpressionSyntax or IdentifierNameSyntax or ThisExpressionSyntax or DefaultExpressionSyntax or TypeOfExpressionSyntax or LambdaExpressionSyntax => true,
                MemberAccessExpressionSyntax { Expression: var receiver } => IsSideEffectFree(receiver),
                ParenthesizedExpressionSyntax { Expression: var inner } => IsSideEffectFree(inner),
                PrefixUnaryExpressionSyntax { RawKind: (int)SyntaxKind.LogicalNotExpression or (int)SyntaxKind.UnaryMinusExpression, Operand: var operand } => IsSideEffectFree(operand),
                BinaryExpressionSyntax { Left: var left, Right: var right } => IsSideEffectFree(left) && IsSideEffectFree(right),
                _ => false,
            };

        // The value moves into a non-async lambda: await, by-reference locals and parameters, ref structs and a struct's this cannot follow it.
        // Anonymous functions bring their own scope, in which the same rules already hold.
        private static bool CanMoveIntoLambda(IOperation operation)
            => operation switch
            {
                IAnonymousFunctionOperation or ILocalFunctionOperation => true,
                IAwaitOperation => false,
                ILocalReferenceOperation { Local: { RefKind: not RefKind.None } or { Type.IsRefLikeType: true } } => false,
                IParameterReferenceOperation { Parameter: { RefKind: not RefKind.None } or { Type.IsRefLikeType: true } } => false,
                IInstanceReferenceOperation { ReferenceKind: InstanceReferenceKind.ContainingTypeInstance, Type.IsValueType: true } => false,
                _ => operation.ChildOperations.All(CanMoveIntoLambda),
            };

        /// <summary>The flattening call returns <c>Option&lt;T&gt;</c> for an <c>Option&lt;Option&lt;T&gt;&gt;</c>, so an explicit <c>Option&lt;T&gt;</c>
        /// type argument becomes <c>T</c>; without one the type argument stays inferred.</summary>
        private static SimpleNameSyntax FlattenedThenName(SimpleNameSyntax name, INamedTypeSymbol nestedOptionType)
            => name is GenericNameSyntax
                ? GenericName(Identifier(BooleanThenMethodName), TypeArgumentList(SingletonSeparatedList(ParseTypeName(nestedOptionType.TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)).WithAdditionalAnnotations(Simplifier.Annotation))))
                : IdentifierName(BooleanThenMethodName);

        private static InvocationExpressionSyntax? FindSelectManyIdentity(InvocationExpressionSyntax invocation, SemanticModel semanticModel, Symbols symbols, CancellationToken cancellationToken)
            => invocation.Parent is MemberAccessExpressionSyntax { Name.Identifier.ValueText: SelectManyMethodName, Parent: InvocationExpressionSyntax { ArgumentList.Arguments: [{ Expression: var selector }] } selectMany }
               && symbols.FunctionalType is { } functionalType
               && semanticModel.GetSymbolInfo(selectMany, cancellationToken).Symbol is IMethodSymbol { Name: SelectManyMethodName } selectManyMethod
               && SymbolEqualityComparer.Default.Equals(selectManyMethod.ContainingType.OriginalDefinition, symbols.OptionOfTType)
               && IsIdentity(semanticModel.GetSymbolInfo(selector, cancellationToken), functionalType)
                ? selectMany
                : null;

        private static bool IsIdentity(SymbolInfo symbolInfo, INamedTypeSymbol functionalType)
            => (symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault()) is IMethodSymbol { Name: IdentityMethodName } identity
               && SymbolEqualityComparer.Default.Equals(identity.ContainingType, functionalType);

        private static SimpleNameSyntax? GetMethodName(InvocationExpressionSyntax invocation)
            => invocation.Expression switch
            {
                SimpleNameSyntax simpleName => simpleName,
                MemberAccessExpressionSyntax memberAccess => memberAccess.Name,
                _ => null,
            };

        private static ExpressionSyntax UnitValue(Symbols symbols)
            => ParseExpression($"global::{symbols.UnitType.ToDisplayString()}.{UnitValuePropertyName}").WithAdditionalAnnotations(Simplifier.Annotation);
    }
}
