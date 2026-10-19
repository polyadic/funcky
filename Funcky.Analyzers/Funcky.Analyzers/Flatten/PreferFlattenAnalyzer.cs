using System.Collections.Immutable;
using Funcky.Analyzers.CodeAnalysisExtensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.Functions.AnonymousFunctionMatching;
using static Funcky.Analyzers.Functions.IdentityFunctionMatching;

namespace Funcky.Analyzers.Flatten;

/// <summary>Reports <c>SelectMany(x => x)</c> and <c>SelectMany(Identity)</c> on a nested <c>Option</c>, <c>Result</c>,
/// <c>Either</c> or <c>IEnumerable</c>: that is <c>Flatten()</c>.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferFlattenAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = $"{DiagnosticName.Prefix}{DiagnosticName.Usage}18";

    public const string FlattenNamespaceProperty = nameof(FlattenNamespaceProperty);

    public const string FlattenMethodName = "Flatten";

    private static readonly DiagnosticDescriptor Descriptor = new(
        id: DiagnosticId,
        title: "Prefer Flatten over SelectMany with the identity function",
        messageFormat: "Prefer 'Flatten' over 'SelectMany' with the identity function",
        category: nameof(Funcky),
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "SelectMany with the identity function collapses one level of nesting. Flatten does the same and says so by name, without a selector to read.",
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
        var flattenHolders = FlattenHolders.Collect(context.Compilation);

        if (!flattenHolders.IsEmpty)
        {
            context.RegisterOperationAction(AnalyzeInvocation(flattenHolders, context.Compilation.GetExpressionOfTType()), OperationKind.Invocation);
        }
    }

    private static Action<OperationAnalysisContext> AnalyzeInvocation(FlattenHolders flattenHolders, INamedTypeSymbol? expressionOfTType)
        => context =>
        {
            var invocation = (IInvocationOperation)context.Operation;

            if (invocation.TargetMethod.Name == FunckyWellKnownMemberNames.SelectManyMethodName
                && MatchSelector(invocation) is { } selector
                && (IsIdentityFunction(selector.Value) || IsIdentityFunctionWithReferenceConversion(selector.Value))
                && flattenHolders.FindNamespace(ReceiverType(invocation)) is { } flattenNamespace
                && !invocation.Syntax.IsInExpressionTree(invocation.SemanticModel!, expressionOfTType, context.CancellationToken))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Descriptor,
                    invocation.Syntax.GetLocation(),
                    ImmutableDictionary<string, string?>.Empty.Add(FlattenNamespaceProperty, flattenNamespace)));
            }
        };

    /// <summary>The single selector of a <c>SelectMany</c> called in member form: the instance method of the monads takes one
    /// parameter, the LINQ extension method takes the receiver as its first parameter.</summary>
    private static IArgumentOperation? MatchSelector(IInvocationOperation invocation)
        => invocation.Syntax is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax, ArgumentList.Arguments: [_] }
            ? invocation switch
            {
                { TargetMethod: { IsExtensionMethod: false, Parameters.Length: 1 }, Arguments: [var selector] } => selector,
                { TargetMethod: { IsExtensionMethod: true, Parameters.Length: 2 }, Arguments: [_, var selector] } => selector,
                _ => null,
            }
            : null;

    /// <summary><c>x => x</c> on an <c>IEnumerable&lt;int[]&gt;</c> returns the parameter through an implicit reference conversion
    /// (array to <c>IEnumerable&lt;int&gt;</c>); <c>Flatten</c> accepts the receiver through the same covariance.</summary>
    private static bool IsIdentityFunctionWithReferenceConversion(IOperation operation)
        => operation is IDelegateCreationOperation { Target: IAnonymousFunctionOperation anonymousFunction }
           && MatchAnonymousUnaryFunctionWithSingleReturn(anonymousFunction) is [var returnOperation]
           && returnOperation.ReturnedValue is IConversionOperation { IsImplicit: true, Conversion.IsReference: true, Operand: IParameterReferenceOperation { Parameter.ContainingSymbol: var parameterContainingSymbol } }
           && SymbolEquals(parameterContainingSymbol, anonymousFunction.Symbol);

    private static ITypeSymbol? ReceiverType(IInvocationOperation invocation)
        => invocation switch
        {
            { Instance: { } instance } => instance.Type,
            { Arguments: [{ Value: var receiver }, ..] } => receiver.Type,
            _ => null,
        };

    /// <summary>The types that have a <c>Flatten</c> extension method, keyed by the original definition of the receiver type,
    /// together with the namespace the extension lives in.</summary>
    private sealed class FlattenHolders
    {
        private readonly ImmutableArray<(INamedTypeSymbol ReceiverType, string Namespace)> _holders;

        private FlattenHolders(ImmutableArray<(INamedTypeSymbol ReceiverType, string Namespace)> holders)
            => _holders = holders;

        public bool IsEmpty => _holders.IsEmpty;

        public static FlattenHolders Collect(Compilation compilation)
            => new(ImmutableArray.CreateRange(
                new (INamedTypeSymbol? ReceiverType, INamedTypeSymbol? Holder)[]
                {
                    (compilation.GetOptionOfTType(), compilation.GetOptionExtensionsType()),
                    (compilation.GetResultOfTType(), compilation.GetTypeByMetadataName("Funcky.Monads.ResultExtensions")),
                    (compilation.GetEitherOfTType(), compilation.GetTypeByMetadataName("Funcky.Monads.EitherExtensions")),
                    (compilation.GetSpecialType(SpecialType.System_Collections_Generic_IEnumerable_T), compilation.GetEnumerableExtensionType()),
                }
                .Where(pair => pair is { ReceiverType: not null, Holder: { } holder } && HasFlatten(holder))
                .Select(pair => (pair.ReceiverType!, pair.Holder!.ContainingNamespace.ToDisplayString()))));

        /// <summary>The receiver is the holder's type itself or, for <c>IEnumerable&lt;T&gt;</c>, any type implementing it.</summary>
        public string? FindNamespace(ITypeSymbol? receiverType)
            => receiverType is null
                ? null
                : _holders
                    .Where(holder => receiverType.AllInterfaces.Prepend(receiverType).Any(type => SymbolEquals(holder.ReceiverType, type.OriginalDefinition)))
                    .Select(holder => holder.Namespace)
                    .FirstOrDefault();

        private static bool HasFlatten(INamedTypeSymbol holder)
            => holder.GetMembers(FlattenMethodName).Any(member => member is IMethodSymbol { IsExtensionMethod: true });
    }
}
