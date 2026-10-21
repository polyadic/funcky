using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers.OrNone;

/// <summary>Reports <c>FirstOrDefault</c>, <c>LastOrDefault</c>, <c>SingleOrDefault</c> and <c>ElementAtOrDefault</c>
/// whose result is immediately tested for <c>null</c>. The <c>…OrNone</c> variants in <c>Funcky.Extensions</c> make the
/// missing element a <c>None</c> instead of a <c>null</c> that has to be checked by hand.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PreferOrNoneAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = $"{DiagnosticName.Prefix}{DiagnosticName.Usage}12";

    public const string OrNoneMethodNameProperty = nameof(OrNoneMethodNameProperty);

    private const string OrDefaultSuffix = "OrDefault";

    private const string OrNoneSuffix = "OrNone";

    private static readonly ImmutableArray<string> OrDefaultMethodNames = ImmutableArray.Create("FirstOrDefault", "LastOrDefault", "SingleOrDefault", "ElementAtOrDefault");

    private static readonly DiagnosticDescriptor Descriptor = new(
        id: DiagnosticId,
        title: "Prefer the …OrNone method over …OrDefault with a null check",
        messageFormat: "Prefer '{0}' over '{1}' followed by a null check",
        category: DiagnosticName.Category,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "FirstOrDefault, LastOrDefault, SingleOrDefault and ElementAtOrDefault signal a missing element with null, which the caller then has to test for. The …OrNone variants return an Option, so the missing element is a None and the handling is a GetOrElse, Select or Match instead of a null check.",
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
        if (context.Compilation.GetEnumerableType() is { } enumerableType
            && context.Compilation.GetEnumerableExtensionType() is { } enumerableExtensionsType)
        {
            context.RegisterOperationAction(AnalyzeInvocation(enumerableType, enumerableExtensionsType), OperationKind.Invocation);
        }
    }

    private static Action<OperationAnalysisContext> AnalyzeInvocation(INamedTypeSymbol enumerableType, INamedTypeSymbol enumerableExtensionsType)
        => context =>
        {
            var invocation = (IInvocationOperation)context.Operation;

            if (MatchOrDefaultMethod(invocation, enumerableType) is { } orDefaultMethodName
                && OrNoneMethodName(orDefaultMethodName) is var orNoneMethodName
                && HasOrNoneVariant(enumerableExtensionsType, orNoneMethodName)
                && IsNullChecked(invocation))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Descriptor,
                    invocation.Syntax.GetLocation(),
                    ImmutableDictionary<string, string?>.Empty.Add(OrNoneMethodNameProperty, orNoneMethodName),
                    orNoneMethodName,
                    orDefaultMethodName));
            }
        };

    /// <summary>The LINQ methods called in extension form, without the overloads that take an explicit default value, over a
    /// reference type that is not nullable-annotated: there a <c>null</c> result can only mean "no element". The element type
    /// is the type argument, not the return type: the nullable-annotated LINQ returns <c>TSource?</c> for every sequence.
    /// The operation tree presents an extension call unreduced, with the receiver as the first argument.</summary>
    private static string? MatchOrDefaultMethod(IInvocationOperation invocation, INamedTypeSymbol enumerableType)
    {
        if (invocation.TargetMethod is { IsExtensionMethod: true, TypeArguments: [{ IsReferenceType: true, NullableAnnotation: not NullableAnnotation.Annotated }] } method
            && SymbolEquals(method.ContainingType, enumerableType)
            && OrDefaultMethodNames.Contains(method.Name)
            && method.Parameters.All(parameter => parameter.Name != "defaultValue")
            && IsCalledInExtensionForm(invocation))
        {
            return method.Name;
        }

        return null;
    }

    private static bool IsCalledInExtensionForm(IInvocationOperation invocation)
        => invocation.Syntax is InvocationExpressionSyntax { Expression: MemberAccessExpressionSyntax, ArgumentList.Arguments.Count: var argumentCount }
           && argumentCount == invocation.TargetMethod.Parameters.Length - 1;

    private static string OrNoneMethodName(string orDefaultMethodName)
        => orDefaultMethodName.Substring(0, orDefaultMethodName.Length - OrDefaultSuffix.Length) + OrNoneSuffix;

    private static bool HasOrNoneVariant(INamedTypeSymbol enumerableExtensionsType, string orNoneMethodName)
        => enumerableExtensionsType.GetMembers(orNoneMethodName).Any(member => member is IMethodSymbol { IsStatic: true, IsExtensionMethod: true });

    /// <summary>Whether the result of the invocation is consumed by a null test: <c>??</c>, <c>?.</c>, <c>== null</c>, <c>!= null</c>
    /// or an <c>is</c> pattern that distinguishes <c>null</c> from a value.</summary>
    private static bool IsNullChecked(IInvocationOperation invocation)
    {
        var (child, parent) = SkipTransparentParents(invocation);
        return parent switch
        {
            ICoalesceOperation coalesce => coalesce.Value == child,
            IConditionalAccessOperation conditionalAccess => conditionalAccess.Operation == child,
            IIsPatternOperation isPattern => isPattern.Value == child && IsNullTest(isPattern.Pattern),
            IBinaryOperation { OperatorKind: BinaryOperatorKind.Equals or BinaryOperatorKind.NotEquals } binary
                => (binary.LeftOperand == child && IsNullLiteral(binary.RightOperand)) || (binary.RightOperand == child && IsNullLiteral(binary.LeftOperand)),
            _ => false,
        };
    }

    private static (IOperation Child, IOperation? Parent) SkipTransparentParents(IOperation operation)
        => operation.Parent is IParenthesizedOperation or IConversionOperation { IsImplicit: true }
            ? SkipTransparentParents(operation.Parent)
            : (operation, operation.Parent);

    private static bool IsNullTest(IPatternOperation pattern)
        => pattern switch
        {
            IConstantPatternOperation constantPattern => IsNullLiteral(constantPattern.Value),
            INegatedPatternOperation negatedPattern => IsNullTest(negatedPattern.Pattern),
            IRecursivePatternOperation { DeconstructionSubpatterns.IsEmpty: true, PropertySubpatterns.IsEmpty: true } => true,
            IDeclarationPatternOperation { MatchesNull: false } => true,
            ITypePatternOperation => true,
            _ => false,
        };

    private static bool IsNullLiteral(IOperation operation)
        => operation switch
        {
            ILiteralOperation { ConstantValue: { HasValue: true, Value: null } } => true,
            IConversionOperation { IsImplicit: true } conversion => IsNullLiteral(conversion.Operand),
            _ => false,
        };
}
