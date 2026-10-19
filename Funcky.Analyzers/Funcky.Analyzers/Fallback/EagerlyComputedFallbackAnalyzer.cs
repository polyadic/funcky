using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers.Fallback;

/// <summary>Reports an argument passed to the eager overload of a fallback-taking method that does work, e.g.
/// <c>option.GetOrElse(LoadDefault())</c> or <c>option.OrElse(new Foo())</c>: the work happens before the call and therefore
/// also when the monad has a value and the fallback is never used.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EagerlyComputedFallbackAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = $"{DiagnosticName.Prefix}{DiagnosticName.Usage}15";

    /// <summary>The number of parameters of the lazy overload's delegate, for the code fix.</summary>
    public const string LambdaParameterCountProperty = nameof(LambdaParameterCountProperty);

    private static readonly DiagnosticDescriptor Descriptor = new(
        id: DiagnosticId,
        title: "Fallback is computed eagerly",
        messageFormat: "The fallback passed to '{0}' is computed even when there is a value; pass a function to compute it only when needed",
        category: nameof(Funcky),
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "An argument is evaluated before the method runs. A fallback that calls a method, allocates an object or awaits does that work even when the fallback is not needed. The lazy overload taking a function defers the work to the case where the fallback is actually used.",
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
        var fallbackTypes = FallbackTypes.FromCompilation(context.Compilation);

        if (!fallbackTypes.IsEmpty)
        {
            var cheapInvocations = CheapInvocations.FromCompilation(context.Compilation);
            context.RegisterOperationAction(context => AnalyzeInvocation(context, fallbackTypes, cheapInvocations), OperationKind.Invocation);
        }
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context, FallbackTypes fallbackTypes, CheapInvocations cheapInvocations)
    {
        var invocation = (IInvocationOperation)context.Operation;

        foreach (var argument in invocation.Arguments.Where(argument => !argument.IsImplicit && DoesWork(argument.Value, cheapInvocations)))
        {
            if (LazyOverloads.FindLazyOverload(invocation, argument, fallbackTypes) is { } lazyOverload && argument.Parameter is { } parameter)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Descriptor,
                    argument.Value.Syntax.GetLocation(),
                    properties: ImmutableDictionary<string, string?>.Empty.Add(
                        LambdaParameterCountProperty,
                        LazyOverloads.DelegateParameterCount(lazyOverload, parameter.Ordinal).ToString()),
                    invocation.TargetMethod.Name));
            }
        }
    }

    // A throw expression is the business of λ1014, so this rule stays silent on those arguments to not report them twice.
    private static bool DoesWork(IOperation operation, CheapInvocations cheapInvocations)
    {
        var descendants = operation.DescendantsAndSelfOutsideFunctions().ToList();
        return !descendants.Any(descendant => descendant is IThrowOperation)
            && descendants.Any(descendant => IsWork(descendant, cheapInvocations));
    }

    private static bool IsWork(IOperation operation, CheapInvocations cheapInvocations)
        => operation switch
        {
            IInvocationOperation invocation => !cheapInvocations.Contains(invocation.TargetMethod),
            IObjectCreationOperation or IAnonymousObjectCreationOperation or IArrayCreationOperation => true,
            IDynamicInvocationOperation or IDynamicObjectCreationOperation => true,
            IAwaitOperation or IInterpolatedStringOperation => true,
            _ => false,
        };

    /// <summary>Invocations that cost nothing worth deferring: the empty-collection singletons and the constructors of the
    /// monads themselves. Their arguments are still inspected.</summary>
    private sealed class CheapInvocations
    {
        private static readonly ImmutableHashSet<string> MonadConstructorNames = ImmutableHashSet.Create(
            OptionSomeMethodName,
            MonadReturnMethodName,
            ResultOkMethodName,
            ResultErrorMethodName,
            EitherLeftMethodName,
            EitherRightMethodName);

        private readonly ImmutableArray<INamedTypeSymbol> _emptyFactories;
        private readonly ImmutableArray<INamedTypeSymbol> _monadConstructorTypes;

        private CheapInvocations(ImmutableArray<INamedTypeSymbol> emptyFactories, ImmutableArray<INamedTypeSymbol> monadConstructorTypes)
        {
            _emptyFactories = emptyFactories;
            _monadConstructorTypes = monadConstructorTypes;
        }

        public static CheapInvocations FromCompilation(Compilation compilation)
            => new(
                NonNull(compilation.GetSpecialType(SpecialType.System_Array), compilation.GetEnumerableType()),
                NonNull(
                    compilation.GetOptionType(),
                    compilation.GetOptionOfTType(),
                    compilation.GetResultType(),
                    compilation.GetResultOfTType(),
                    compilation.GetEitherType(),
                    compilation.GetEitherOfTType()));

        public bool Contains(IMethodSymbol method)
            => (method.Name == "Empty" && _emptyFactories.Contains(method.ContainingType.OriginalDefinition, SymbolEqualityComparer.Default))
               || (MonadConstructorNames.Contains(method.Name) && _monadConstructorTypes.Contains(method.ContainingType.OriginalDefinition, SymbolEqualityComparer.Default));

        private static ImmutableArray<INamedTypeSymbol> NonNull(params INamedTypeSymbol?[] types)
            => ImmutableArray.CreateRange(types.Where(type => type is not null).Select(type => type!));
    }
}
