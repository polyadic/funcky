using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics.CodeAnalysis;
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
/// Replaces a call of <c>Option.FromBoolean</c> with <c>bool.Then</c>: <c>Option.FromBoolean(condition, value)</c> becomes
/// <c>condition.Then(value)</c>, the overload with a selector keeps its selector, <c>Option.FromBoolean(condition)</c> becomes
/// <c>condition.Then(Unit.Value)</c>, and the flattening idiom <c>Option.FromBoolean(condition, option).SelectMany(Identity)</c>
/// becomes <c>condition.Then(() => option)</c>. The fix is offered on the obsoletion warning CS0618 (Funcky 3.8.0) and on the
/// compile error CS0117 that a call of the removed method reports (Funcky 4.0.0), so a project can upgrade first and rewrite after.
/// With the member gone the compiler resolves no overload, so the fix reads it off the arguments: a lambda, a method group or a
/// <c>Func&lt;T&gt;</c> is the selector overload, everything else the value overload, as <c>FromBoolean</c> itself would have chosen.
/// </summary>
[Shared]
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(OptionFromBooleanCodeFix))]
public sealed class OptionFromBooleanCodeFix : ExtensionMemberCodeFix
{
    /// <summary>The compiler diagnostic reported on the use of a member marked <c>[Obsolete]</c> with a message.</summary>
    private const string ObsoleteMemberDiagnosticId = "CS0618";

    /// <summary>The compiler diagnostic reported on a member a type does not have, which a call of the removed method is.</summary>
    private const string MissingMemberDiagnosticId = "CS0117";

    /// <summary>The parameter names of the removed <c>FromBoolean</c> overloads, which named arguments still refer to.</summary>
    private const string FromBooleanConditionParameterName = "boolean";

    private const string FromBooleanItemParameterName = "item";

    private const string FromBooleanSelectorParameterName = "selector";

    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(ObsoleteMemberDiagnosticId, MissingMemberDiagnosticId);

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

    protected override ExtensionMemberRewrite? TryCreateRewrite(SyntaxNode root, SemanticModel semanticModel, Diagnostic diagnostic, CancellationToken cancellationToken)
        => GetSymbols(semanticModel.Compilation) is { } symbols
           && FindCall(root, semanticModel, diagnostic, symbols, cancellationToken) is { } call
           && Rewrite.TryCreate(call, semanticModel, symbols, cancellationToken) is { } rewrite
            ? rewrite.ToExtensionMemberRewrite()
            : null;

    protected override Task<Document?> TryApplyAsync(Document document, SyntaxNode root, SemanticModel semanticModel, ImmutableArray<ExtensionMemberRewrite> rewrites, CancellationToken cancellationToken)
        => semanticModel.Compilation.GetBooleanExtensionsType() is { } booleanExtensionsType
            ? BooleanThen.TryReplaceAllAsync(document, root, semanticModel, rewrites, booleanExtensionsType, cancellationToken)
            : Task.FromResult<Document?>(null);

    private static Call? FindCall(SyntaxNode root, SemanticModel semanticModel, Diagnostic diagnostic, Symbols symbols, CancellationToken cancellationToken)
        => diagnostic.Id switch
        {
            ObsoleteMemberDiagnosticId => FindObsoleteCall(root, semanticModel, diagnostic, symbols, cancellationToken),
            MissingMemberDiagnosticId => FindRemovedCall(root, semanticModel, diagnostic, symbols, cancellationToken),
            _ => null,
        };

    // The warning spans the whole call; one with a smaller span belongs to an obsolete member inside an argument.
    private static Call? FindObsoleteCall(SyntaxNode root, SemanticModel semanticModel, Diagnostic diagnostic, Symbols symbols, CancellationToken cancellationToken)
        => root.FindInvocationExpression(diagnostic.Location.SourceSpan) is { } invocation
           && invocation.Span == diagnostic.Location.SourceSpan
           && semanticModel.GetOperation(invocation, cancellationToken) is IInvocationOperation { TargetMethod: { Name: OptionFromBooleanMethodName, IsStatic: true } method } operation
           && SymbolEqualityComparer.Default.Equals(method.ContainingType, symbols.OptionType)
            ? Call.FromOperation(operation, invocation)
            : null;

    // The error spans the member name; the receiver in front of it still binds to the Option type once the member is gone.
    private static Call? FindRemovedCall(SyntaxNode root, SemanticModel semanticModel, Diagnostic diagnostic, Symbols symbols, CancellationToken cancellationToken)
        => root.FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true) is SimpleNameSyntax { Identifier.ValueText: OptionFromBooleanMethodName, Parent: MemberAccessExpressionSyntax { Expression: var receiver, Parent: InvocationExpressionSyntax invocation } memberAccess } name
           && memberAccess.Name == name
           && semanticModel.GetSymbolInfo(receiver, cancellationToken).Symbol is INamedTypeSymbol receiverType
           && SymbolEqualityComparer.Default.Equals(receiverType, symbols.OptionType)
            ? Call.FromSyntax(invocation, name, semanticModel, symbols, cancellationToken)
            : null;

    private static Symbols? GetSymbols(Compilation compilation)
        => compilation.GetOptionType() is { } optionType
           && compilation.GetOptionOfTType() is { } optionOfTType
           && compilation.GetUnitType() is { } unitType
           && compilation.GetBooleanExtensionsType() is { } booleanExtensionsType
           && BooleanThen.HasThen(booleanExtensionsType)
            ? new Symbols(optionType, optionOfTType, unitType, booleanExtensionsType, compilation.GetFunctionalType(), compilation.GetFuncOfTType())
            : null;

    private static SimpleNameSyntax? GetMethodName(InvocationExpressionSyntax invocation)
        => invocation.Expression switch
        {
            SimpleNameSyntax simpleName => simpleName,
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name,
            _ => null,
        };

    private sealed record Symbols(INamedTypeSymbol OptionType, INamedTypeSymbol OptionOfTType, INamedTypeSymbol UnitType, INamedTypeSymbol BooleanExtensionsType, INamedTypeSymbol? FunctionalType, INamedTypeSymbol? FuncOfTType);

    /// <summary>An argument of the call in its position in the source, with the ordinal of the parameter it goes to.</summary>
    private sealed record CallArgument(ArgumentSyntax Syntax, int Ordinal, IOperation? Value);

    /// <summary>A call of <c>Option.FromBoolean</c> as far as the rewrite needs to know it: bound by the compiler while the method is
    /// still there, or reconstructed from the syntax and the types of the arguments once it is gone. <paramref name="ItemType"/> is
    /// the type argument of the called overload, <see langword="null"/> for the overload without a value.</summary>
    private sealed record Call(InvocationExpressionSyntax Invocation, SimpleNameSyntax Name, ImmutableArray<CallArgument> Arguments, bool IsSelectorOverload, ITypeSymbol? ItemType, bool IsBound)
    {
        public static Call? FromOperation(IInvocationOperation operation, InvocationExpressionSyntax invocation)
        {
            if (GetMethodName(invocation) is not { } name)
            {
                return null;
            }

            var arguments = operation.Arguments
                .Select(argument => argument.Syntax is ArgumentSyntax syntax ? new CallArgument(syntax, argument.Parameter?.Ordinal ?? int.MaxValue, argument.Value) : null)
                .ToImmutableArray();

            if (arguments.Any(argument => argument is null))
            {
                return null;
            }

            // The definition tells the overloads apart; the constructed parameter type is a delegate for the value overload too when TItem is one.
            var method = operation.TargetMethod;
            var isSelectorOverload = method.OriginalDefinition.Parameters is [_, { Type.TypeKind: TypeKind.Delegate }];
            var itemType = method.TypeArguments is [var typeArgument] ? typeArgument : null;

            return new Call(invocation, name, arguments!, isSelectorOverload, itemType, IsBound: true);
        }

        public static Call? FromSyntax(InvocationExpressionSyntax invocation, SimpleNameSyntax name, SemanticModel semanticModel, Symbols symbols, CancellationToken cancellationToken)
        {
            var arguments = invocation.ArgumentList.Arguments
                .Select((argument, index) => new CallArgument(argument, GetOrdinal(argument, index), semanticModel.GetOperation(argument.Expression, cancellationToken)))
                .ToImmutableArray();

            if (arguments.Any(argument => argument.Ordinal < 0) || arguments.Select(argument => argument.Ordinal).Distinct().Count() != arguments.Length)
            {
                return null;
            }

            ITypeSymbol? explicitItemType = null;

            if (name is GenericNameSyntax genericName)
            {
                if (genericName.TypeArgumentList.Arguments is not [var typeArgument] || KnownType(semanticModel.GetTypeInfo(typeArgument, cancellationToken).Type) is not { } boundTypeArgument)
                {
                    return null;
                }

                explicitItemType = boundTypeArgument;
            }

            return arguments switch
            {
                [{ Ordinal: 0 }] when explicitItemType is null => new Call(invocation, name, arguments, IsSelectorOverload: false, ItemType: null, IsBound: false),
                [_, _] when InferOverload(arguments.First(argument => argument.Ordinal == 1), explicitItemType, semanticModel, symbols, cancellationToken) is var (isSelectorOverload, itemType)
                    => new Call(invocation, name, arguments, isSelectorOverload, itemType, IsBound: false),
                _ => null,
            };
        }

        private static int GetOrdinal(ArgumentSyntax argument, int position)
            => argument.NameColon?.Name.Identifier.ValueText switch
            {
                null => position,
                FromBooleanConditionParameterName => 0,
                FromBooleanItemParameterName or FromBooleanSelectorParameterName => 1,
                _ => -1,
            };

        /// <summary>Which overload the compiler would have picked for the value argument and with which type argument, or
        /// <see langword="null"/> when that cannot be told from the argument alone.</summary>
        private static (bool IsSelectorOverload, ITypeSymbol ItemType)? InferOverload(CallArgument argument, ITypeSymbol? explicitItemType, SemanticModel semanticModel, Symbols symbols, CancellationToken cancellationToken)
        {
            var isNamedItem = argument.Syntax.NameColon?.Name.Identifier.ValueText == FromBooleanItemParameterName;
            var isNamedSelector = argument.Syntax.NameColon?.Name.Identifier.ValueText == FromBooleanSelectorParameterName;
            var expression = argument.Syntax.Expression;

            if (explicitItemType is { TypeKind: TypeKind.Delegate } && !isNamedSelector)
            {
                return (false, explicitItemType);
            }

            if (expression is AnonymousFunctionExpressionSyntax lambda)
            {
                return !isNamedItem && (explicitItemType ?? GetLambdaReturnType(lambda, semanticModel, cancellationToken)) is { } lambdaItemType
                    ? (true, lambdaItemType)
                    : null;
            }

            if (KnownType(semanticModel.GetTypeInfo(expression, cancellationToken).Type) is { } type)
            {
                return !isNamedItem && IsFuncOfT(type, symbols, out var funcResultType)
                    ? (true, explicitItemType ?? funcResultType)
                    : !isNamedSelector
                        ? (false, explicitItemType ?? type)
                        : null;
            }

            // A method group has no type of its own; it converts to Func<T> with the return type of the method.
            var symbolInfo = semanticModel.GetSymbolInfo(expression, cancellationToken);
            return !isNamedItem && (symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault()) is IMethodSymbol { ReturnsVoid: false } methodGroup
                ? (true, explicitItemType ?? methodGroup.ReturnType)
                : null;
        }

        private static bool IsFuncOfT(ITypeSymbol type, Symbols symbols, [NotNullWhen(true)] out ITypeSymbol? resultType)
        {
            var isFuncOfT = type is INamedTypeSymbol { OriginalDefinition: var definition, TypeArguments: [_] }
                            && SymbolEqualityComparer.Default.Equals(definition, symbols.FuncOfTType);
            resultType = isFuncOfT ? ((INamedTypeSymbol)type).TypeArguments[0] : null;
            return isFuncOfT;
        }

        // The compiler binds the body of a lambda even when the call around it does not; the type of what it returns is the item type.
        private static ITypeSymbol? GetLambdaReturnType(AnonymousFunctionExpressionSyntax lambda, SemanticModel semanticModel, CancellationToken cancellationToken)
        {
            ExpressionSyntax?[] returnedExpressions = lambda.ExpressionBody is { } expressionBody
                ? [expressionBody]
                : lambda.Block?.DescendantNodes(node => node is not AnonymousFunctionExpressionSyntax and not LocalFunctionStatementSyntax)
                    .OfType<ReturnStatementSyntax>()
                    .Select(returnStatement => returnStatement.Expression)
                    .ToArray() ?? [];

            var returnTypes = returnedExpressions
                .Select(expression => expression is null ? null : KnownType(semanticModel.GetTypeInfo(expression, cancellationToken).Type))
                .ToArray();

            return returnTypes.Length > 0 && returnTypes.All(type => type is not null)
                ? returnTypes[0]
                : null;
        }

        private static ITypeSymbol? KnownType(ITypeSymbol? type)
            => type is { TypeKind: not TypeKind.Error } ? type : null;
    }

    /// <summary>What to replace (the <c>FromBoolean</c> call, or the <c>SelectMany(Identity)</c> around it) and with what.</summary>
    private sealed record Rewrite(InvocationExpressionSyntax Target, ExpressionSyntax Condition, SimpleNameSyntax ThenName, ArgumentSyntax ThenArgument)
    {
        public static Rewrite? TryCreate(Call call, SemanticModel semanticModel, Symbols symbols, CancellationToken cancellationToken)
        {
            if (!KeepsEvaluationOrder(call.Arguments))
            {
                return null;
            }

            // The arguments are in evaluation order, which named arguments can change.
            return call.Arguments.OrderBy(argument => argument.Ordinal).ToArray() switch
            {
                [{ Syntax: var condition }] => new Rewrite(call.Invocation, condition.Expression, BooleanThen.ThenName(call.Name), Argument(UnitValue(symbols))),
                [{ Syntax: var condition }, var argument] => TryCreate(call, condition.Expression, argument, semanticModel, symbols, cancellationToken),
                _ => null,
            };
        }

        public ExtensionMemberRewrite ToExtensionMemberRewrite()
            => new(Target, BooleanThen.Invocation(Condition, ThenName, ThenArgument).WithTriviaFrom(Target));

        private static Rewrite? TryCreate(Call call, ExpressionSyntax condition, CallArgument argument, SemanticModel semanticModel, Symbols symbols, CancellationToken cancellationToken)
        {
            var value = argument.Syntax.Expression;
            var producesNestedOption = call.ItemType is INamedTypeSymbol { OriginalDefinition: var itemTypeDefinition } && SymbolEqualityComparer.Default.Equals(itemTypeDefinition, symbols.OptionOfTType);

            return (FindSelectManyIdentity(call, semanticModel, symbols, cancellationToken), call.IsSelectorOverload, producesNestedOption) switch
            {
                // Option.FromBoolean(condition, option).SelectMany(Identity) => condition.Then(() => option)
                ({ } selectMany, false, true) => argument.Value is { } valueOperation && CanMoveIntoLambda(valueOperation)
                    ? new Rewrite(selectMany, condition, FlattenedThenName(call.Name, (INamedTypeSymbol)call.ItemType!), Argument(ParenthesizedLambdaExpression(value.WithoutTrivia())))
                    : null,

                // Option.FromBoolean(condition, () => option).SelectMany(Identity) => condition.Then(() => option)
                ({ } selectMany, true, true) => new Rewrite(selectMany, condition, FlattenedThenName(call.Name, (INamedTypeSymbol)call.ItemType!), Argument(value)),

                // Option.FromBoolean(condition, () => option) yields an Option<Option<T>>, which Then would flatten.
                (_, true, true) => null,

                (_, true, false) => new Rewrite(call.Invocation, condition, BooleanThen.ThenName(call.Name), Argument(value)),

                (_, false, _) => new Rewrite(call.Invocation, condition, BooleanThen.ThenName(call.Name), BooleanThen.ValueArgument(value, call.ItemType)),
            };
        }

        // Then evaluates the condition before the value. Named arguments given in the other order are only reordered when neither has an
        // effect the other could observe; the check is syntactic and errs on the side of keeping the call.
        private static bool KeepsEvaluationOrder(ImmutableArray<CallArgument> arguments)
            => arguments.Select(argument => argument.Ordinal).ToArray() is var ordinals
               && (ordinals.SequenceEqual(ordinals.OrderBy(ordinal => ordinal))
                   || arguments.All(argument => IsSideEffectFree(argument.Syntax.Expression)));

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

        // While the call is bound, the SelectMany has to be the one of Option<T>; once the receiver is an error type the name has to do.
        private static InvocationExpressionSyntax? FindSelectManyIdentity(Call call, SemanticModel semanticModel, Symbols symbols, CancellationToken cancellationToken)
            => call.Invocation.Parent is MemberAccessExpressionSyntax { Name.Identifier.ValueText: SelectManyMethodName, Parent: InvocationExpressionSyntax { ArgumentList.Arguments: [{ Expression: var selector }] } selectMany }
               && symbols.FunctionalType is { } functionalType
               && IsOptionSelectMany(semanticModel.GetSymbolInfo(selectMany, cancellationToken), call.IsBound, symbols)
               && IsIdentity(semanticModel.GetSymbolInfo(selector, cancellationToken), functionalType)
                ? selectMany
                : null;

        private static bool IsOptionSelectMany(SymbolInfo symbolInfo, bool isBound, Symbols symbols)
            => symbolInfo.Symbol is IMethodSymbol { Name: SelectManyMethodName } selectManyMethod
                ? SymbolEqualityComparer.Default.Equals(selectManyMethod.ContainingType.OriginalDefinition, symbols.OptionOfTType)
                : !isBound;

        private static bool IsIdentity(SymbolInfo symbolInfo, INamedTypeSymbol functionalType)
            => (symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault()) is IMethodSymbol { Name: IdentityMethodName } identity
               && SymbolEqualityComparer.Default.Equals(identity.ContainingType, functionalType);

        private static ExpressionSyntax UnitValue(Symbols symbols)
            => ParseExpression($"global::{symbols.UnitType.ToDisplayString()}.{UnitValuePropertyName}").WithAdditionalAnnotations(Simplifier.Annotation);
    }
}
