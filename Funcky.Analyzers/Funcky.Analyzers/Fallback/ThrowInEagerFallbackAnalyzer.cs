using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers.Fallback;

/// <summary>Reports a <c>throw</c> expression inside an argument that is passed to the eager overload of a fallback-taking
/// method, e.g. <c>option.GetOrElse(fallback ?? throw new Exception())</c>: the argument is evaluated before the call, so the
/// throw happens regardless of the state of the monad.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ThrowInEagerFallbackAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = $"{DiagnosticName.Prefix}{DiagnosticName.Usage}14";

    /// <summary>The number of parameters of the lazy overload's delegate, for the code fix.</summary>
    public const string LambdaParameterCountProperty = nameof(LambdaParameterCountProperty);

    private static readonly DiagnosticDescriptor Descriptor = new(
        id: DiagnosticId,
        title: "Throw expression in an eagerly evaluated fallback",
        messageFormat: "The fallback passed to '{0}' is evaluated before the call, so the throw expression throws even when there is a value; pass a function instead",
        category: nameof(Funcky),
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "An argument is evaluated before the method runs. A throw expression inside a fallback argument therefore throws regardless of whether the fallback is needed. The lazy overload taking a function defers the evaluation to the case where the fallback is actually used.",
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
            context.RegisterOperationAction(context => AnalyzeInvocation(context, fallbackTypes), OperationKind.Invocation);
        }
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context, FallbackTypes fallbackTypes)
    {
        var invocation = (IInvocationOperation)context.Operation;

        foreach (var argument in invocation.Arguments.Where(argument => !argument.IsImplicit && ContainsThrow(argument.Value)))
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

    private static bool ContainsThrow(IOperation operation)
        => operation.DescendantsAndSelfOutsideFunctions().Any(descendant => descendant is IThrowOperation);
}
