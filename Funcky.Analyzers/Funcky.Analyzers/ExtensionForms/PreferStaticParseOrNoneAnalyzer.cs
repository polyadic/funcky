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

    /// <summary>Builds <c>TargetType.ParseOrNone(receiver, arguments…)</c> from the invocation of the string extension.
    /// An invocation inside a conditional access (<c>text?.ParseInt32OrNone()</c>) has no static form that keeps the null propagation.</summary>
    public static InvocationExpressionSyntax? StaticInvocation(IInvocationOperation operation, string targetTypeName, string staticMemberName)
        => operation.Syntax is InvocationExpressionSyntax invocation
           && !IsConditionallyAccessed(invocation.Expression)
           && ArgumentList(operation, invocation.ArgumentList) is { } argumentList
            ? InvocationExpression(ParseExpression($"{targetTypeName}.{staticMemberName}"), argumentList)
            : null;

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
                && BindsToStaticParseMember(replacement, operation, semanticModel, parseExtensionsType, targetType, staticMemberName))
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

    // The replacement must bind to one of the generated static parse classes (Int32ParseExtensions, …), not back to the string
    // extension. When it binds to nothing because `Funcky.Extensions` is not imported (the extension was called fully qualified
    // or through a `using static`), the member is looked up through its static class instead; the code fix adds the import.
    private static bool BindsToStaticParseMember(InvocationExpressionSyntax replacement, IInvocationOperation operation, SemanticModel semanticModel, INamedTypeSymbol parseExtensionsType, ITypeSymbol targetType, string staticMemberName)
    {
        var symbolInfo = semanticModel.GetSpeculativeSymbolInfo(operation.Syntax.SpanStart, replacement, SpeculativeBindingOption.BindAsExpression);
        return symbolInfo.Symbol is { } symbol
            ? IsStaticParseMember(symbol, parseExtensionsType)
            : symbolInfo.CandidateSymbols.IsEmpty
                && !IsVisible(parseExtensionsType, semanticModel, operation.Syntax.SpanStart)
                && BindsThroughStaticClass(replacement, operation, semanticModel, parseExtensionsType, targetType, staticMemberName);
    }

    private static bool IsStaticParseMember(ISymbol symbol, INamedTypeSymbol parseExtensionsType)
        => DeclaringClass(symbol) is { } declaringClass
           && !SymbolEquals(declaringClass, parseExtensionsType)
           && IsDeclaredInFunckyExtensions(symbol);

    // A static extension member is also callable as a static method of its class (`Int32ParseExtensions.ParseOrNone(text)`), which
    // needs no import when fully qualified. The member belongs to the parsed type when it returns the same Option<T> as the
    // extension; a generic class (EnumParseExtensions) takes the parsed type as its type argument.
    private static bool BindsThroughStaticClass(InvocationExpressionSyntax replacement, IInvocationOperation operation, SemanticModel semanticModel, INamedTypeSymbol parseExtensionsType, ITypeSymbol targetType, string staticMemberName)
        => parseExtensionsType.ContainingNamespace.GetTypeMembers()
            .Where(type => type is { TypeKind: TypeKind.Class, IsStatic: true } && !SymbolEquals(type, parseExtensionsType) && HasStaticMember(type, staticMemberName))
            .SelectMany(type => StaticClassInvocations(type, replacement, targetType, staticMemberName))
            .Select(invocation => semanticModel.GetSpeculativeSymbolInfo(operation.Syntax.SpanStart, invocation, SpeculativeBindingOption.BindAsExpression).Symbol)
            .Any(symbol => symbol is IMethodSymbol { IsStatic: true, IsExtensionMethod: false } method
                && IsDeclaredInFunckyExtensions(method)
                && SymbolEquals(method.ReturnType.WithNullableAnnotation(NullableAnnotation.NotAnnotated), operation.TargetMethod.ReturnType.WithNullableAnnotation(NullableAnnotation.NotAnnotated)));

    private static IEnumerable<InvocationExpressionSyntax> StaticClassInvocations(INamedTypeSymbol type, InvocationExpressionSyntax replacement, ITypeSymbol targetType, string staticMemberName)
    {
        var className = type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var typeArguments = $"<{targetType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>";

        return StaticMemberArities(type, staticMemberName)
            .Select(arity => arity switch { 0 => string.Empty, 1 => typeArguments, _ => null })
            .Where(arguments => arguments is not null)
            .Select(arguments => replacement.WithExpression(ParseExpression($"{className}.{staticMemberName}{arguments}")));
    }

    // From source the member is nested in its extension block, which contributes its own type parameters; from metadata the
    // lowered static method is a direct member and already carries them.
    private static IEnumerable<int> StaticMemberArities(INamedTypeSymbol type, string staticMemberName)
        => type.GetTypeMembers()
            .SelectMany(extension => extension.GetMembers(staticMemberName).OfType<IMethodSymbol>().Select(method => extension.Arity + method.Arity))
            .Concat(type.GetMembers(staticMemberName).OfType<IMethodSymbol>().Where(method => method.IsStatic && !method.IsExtensionMethod).Select(method => method.Arity))
            .Distinct();

    // `text?.ParseInt32OrNone()` and `text?.Trim().ParseInt32OrNone()` invoke through a member binding somewhere down the receiver chain.
    private static bool IsConditionallyAccessed(ExpressionSyntax invoked)
        => invoked switch
        {
            MemberBindingExpressionSyntax or ElementBindingExpressionSyntax => true,
            MemberAccessExpressionSyntax memberAccess => IsConditionallyAccessed(memberAccess.Expression),
            InvocationExpressionSyntax invocation => IsConditionallyAccessed(invocation.Expression),
            ElementAccessExpressionSyntax elementAccess => IsConditionallyAccessed(elementAccess.Expression),
            _ => false,
        };

    // The receiver becomes the first argument; the IOperation already lists it as the first argument of the lowered static
    // call. The original argument list is kept token by token (its parentheses, commas and comments); the receiver is inserted
    // in front, or only loses its name when the author already passed it as the first argument.
    private static ArgumentListSyntax? ArgumentList(IInvocationOperation operation, ArgumentListSyntax argumentList)
    {
        if (operation.Instance is { Syntax: ExpressionSyntax instance })
        {
            return argumentList.WithArguments(WithReceiver(argumentList.Arguments, instance));
        }

        return operation.Arguments.FirstOrDefault(argument => argument.Parameter?.Ordinal == 0) switch
        {
            { Syntax: ArgumentSyntax { NameColon: { } nameColon } receiverArgument } when argumentList.Arguments.IndexOf(receiverArgument) == 0
                => argumentList.ReplaceNode(receiverArgument, receiverArgument.WithNameColon(null).WithExpression(receiverArgument.Expression.WithLeadingTrivia(nameColon.GetLeadingTrivia()))),
            { Syntax: ArgumentSyntax } => argumentList,
            { Syntax: ExpressionSyntax receiver } => argumentList.WithArguments(WithReceiver(argumentList.Arguments, receiver)),
            _ => null,
        };
    }

    // The receiver takes the place (and the line) of the first argument, which then follows it after the comma.
    private static SeparatedSyntaxList<ArgumentSyntax> WithReceiver(SeparatedSyntaxList<ArgumentSyntax> arguments, ExpressionSyntax receiver)
        => arguments is [var first, ..]
            ? arguments
                .Replace(first, first.WithoutLeadingTrivia())
                .Insert(0, Argument(receiver.WithoutTrivia()).WithLeadingTrivia(first.GetLeadingTrivia()))
            : arguments.Add(Argument(receiver.WithoutTrivia()));
}
