using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using static Funcky.Analyzers.FunctionalAssert.FunctionalAssertMatching;
using static Funcky.Analyzers.FunctionalAssert.XunitAssertMatching;

namespace Funcky.Analyzers.FunctionalAssert;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class FunctionalAssertAnalyzer : DiagnosticAnalyzer
{
    public const string ExpectedArgumentIndex = nameof(ExpectedArgumentIndex);
    public const string ActualArgumentIndex = nameof(ActualArgumentIndex);

    public static readonly DiagnosticDescriptor PreferFunctionalAssert = new(
        id: $"{DiagnosticName.Prefix}{DiagnosticName.FunctionalAssertUsage}01",
        title: "Assert can be simplified",
        messageFormat: "Assert can be simplified to a single call to {0}.{1}",
        category: nameof(Funcky),
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private const string AttributeFullName = "Funcky.CodeAnalysis.AssertMethodHasOverloadWithExpectedValueAttribute";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(PreferFunctionalAssert);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(OnCompilationStart);
    }

    private static void OnCompilationStart(CompilationStartAnalysisContext context)
    {
        if (context.Compilation.GetTypeByMetadataName(AttributeFullName) is { } attributeType)
        {
            context.RegisterOperationAction(AnalyzeInvocation(new AssertMethodHasOverloadWithExpectedValueAttributeType(attributeType)), OperationKind.Invocation);
        }
    }

    private static Action<OperationAnalysisContext> AnalyzeInvocation(AssertMethodHasOverloadWithExpectedValueAttributeType attributeType)
        => context =>
        {
            var invocation = (IInvocationOperation)context.Operation;
            if (MatchGenericAssertEqualInvocation(invocation) is [var (expectedArgument, actualArgument)]
                && actualArgument.Value is IInvocationOperation innerInvocation
                && IsAssertMethodWithAccompanyingEqualOverload(innerInvocation, attributeType))
            {
                var properties = ImmutableDictionary<string, string?>.Empty
                    .Add(ActualArgumentIndex, invocation.Arguments.IndexOf(actualArgument).ToString())
                    .Add(ExpectedArgumentIndex, invocation.Arguments.IndexOf(expectedArgument).ToString());
                context.ReportDiagnostic(Diagnostic.Create(
                    PreferFunctionalAssert,
                    invocation.Syntax.GetLocation(),
                    properties,
                    messageArgs: [innerInvocation.TargetMethod.ContainingType.Name, innerInvocation.TargetMethod.Name]));
            }
        };
}
