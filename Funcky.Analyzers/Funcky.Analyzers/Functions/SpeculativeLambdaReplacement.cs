using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Funcky.Analyzers.Functions;

/// <summary>Checks whether an anonymous function can be replaced by a method group without changing the meaning of the code.
/// A method group takes part in overload resolution and type inference differently from a lambda (e.g. it can be ambiguous between
/// overloads of the same arity), so the replacement is bound speculatively and only accepted when it resolves the same way.</summary>
internal static class SpeculativeLambdaReplacement
{
    // siblingReplacements: the other anonymous functions in the same invocation that are reported as well, with their replacements.
    // A batch fix applies all of them at once, so the invocation also has to bind identically with every one replaced: in
    // M((int x) => x, (int y) => y) with M<T>(Func<T, T>, Func<T, T>) either lambda alone supplies T, both method groups together do not.
    public static bool BindsIdentically(
        SemanticModel semanticModel,
        AnonymousFunctionExpressionSyntax anonymousFunction,
        ExpressionSyntax replacement,
        ITypeSymbol delegateType,
        ImmutableDictionary<AnonymousFunctionExpressionSyntax, ExpressionSyntax> siblingReplacements,
        CancellationToken cancellationToken)
        => WalkUpParentheses(anonymousFunction).Parent switch
        {
            ArgumentSyntax { Parent.Parent: var invocation } when invocation is InvocationExpressionSyntax or BaseObjectCreationExpressionSyntax
                => InvocationBindsToSameMethod(semanticModel, invocation, ImmutableDictionary<AnonymousFunctionExpressionSyntax, ExpressionSyntax>.Empty.Add(anonymousFunction, replacement), cancellationToken)
                    && (siblingReplacements.IsEmpty || InvocationBindsToSameMethod(semanticModel, invocation, siblingReplacements.Add(anonymousFunction, replacement), cancellationToken)),
            EqualsValueClauseSyntax equalsValue when IsImplicitlyTypedVariable(equalsValue) => false,
            AssignmentExpressionSyntax assignment when IsDiscard(semanticModel, assignment, cancellationToken) => false,
            EqualsValueClauseSyntax or AssignmentExpressionSyntax or ReturnStatementSyntax or ArrowExpressionClauseSyntax or CastExpressionSyntax
                => semanticModel.ClassifyConversion(anonymousFunction.SpanStart, replacement, delegateType) is { Exists: true, IsImplicit: true },
            _ => false,
        };

    private static SyntaxNode WalkUpParentheses(SyntaxNode node)
        => node.Parent is ParenthesizedExpressionSyntax parenthesized
            ? WalkUpParentheses(parenthesized)
            : node;

    // `var f = x => x` gives the local the lambda's natural type; a generic method group has none, so `var f = Identity` does not compile.
    private static bool IsImplicitlyTypedVariable(EqualsValueClauseSyntax equalsValue)
        => equalsValue.Parent is VariableDeclaratorSyntax { Parent: VariableDeclarationSyntax { Type.IsVar: true } };

    // A discard takes the type of the expression as well.
    private static bool IsDiscard(SemanticModel semanticModel, AssignmentExpressionSyntax assignment, CancellationToken cancellationToken)
        => semanticModel.GetSymbolInfo(assignment.Left, cancellationToken).Symbol is IDiscardSymbol;

    private static bool InvocationBindsToSameMethod(
        SemanticModel semanticModel,
        SyntaxNode invocation,
        ImmutableDictionary<AnonymousFunctionExpressionSyntax, ExpressionSyntax> replacements,
        CancellationToken cancellationToken)
        => semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol is { } originalMethod
            && GetSpeculativeSemanticModel(semanticModel, invocation, replacements) is var (speculativeSemanticModel, speculativeInvocation)
            && SymbolEquals(originalMethod, speculativeSemanticModel.GetSymbolInfo(speculativeInvocation, cancellationToken).Symbol);

    /// <summary>Re-binds the innermost statement (or member body) around the invocation with the anonymous functions replaced,
    /// and returns the model together with the invocation inside the re-bound syntax.</summary>
    private static (SemanticModel SemanticModel, SyntaxNode Invocation)? GetSpeculativeSemanticModel(
        SemanticModel semanticModel,
        SyntaxNode invocation,
        ImmutableDictionary<AnonymousFunctionExpressionSyntax, ExpressionSyntax> replacements)
    {
        var annotation = new SyntaxAnnotation();
        var replacedInvocation = invocation
            .ReplaceNodes(replacements.Keys, (original, _) => replacements[original].WithTriviaFrom(original))
            .WithAdditionalAnnotations(annotation);

        foreach (var ancestor in invocation.Ancestors())
        {
            if (ancestor is MemberDeclarationSyntax)
            {
                return null;
            }

            var replacedAncestor = ancestor.ReplaceNode(invocation, replacedInvocation);
            if (TryGetSpeculativeSemanticModel(semanticModel, ancestor, replacedAncestor) is { } speculativeSemanticModel)
            {
                return (speculativeSemanticModel, replacedAncestor.GetAnnotatedNodes(annotation).First());
            }
        }

        return null;
    }

    private static SemanticModel? TryGetSpeculativeSemanticModel(SemanticModel semanticModel, SyntaxNode original, SyntaxNode replaced)
        => replaced switch
        {
            StatementSyntax statement when semanticModel.TryGetSpeculativeSemanticModel(original.SpanStart, statement, out var speculative) => speculative,
            ArrowExpressionClauseSyntax arrow when semanticModel.TryGetSpeculativeSemanticModel(original.SpanStart, arrow, out var speculative) => speculative,
            EqualsValueClauseSyntax initializer when IsMemberInitializer(original) && semanticModel.TryGetSpeculativeSemanticModel(original.SpanStart, initializer, out var speculative) => speculative,
            _ => null,
        };

    /// <summary>Only field and property initializers can be re-bound on their own; a local's initializer is re-bound as part of its statement.</summary>
    private static bool IsMemberInitializer(SyntaxNode node)
        => node.Parent is VariableDeclaratorSyntax { Parent.Parent: FieldDeclarationSyntax } or PropertyDeclarationSyntax;
}
