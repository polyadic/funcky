using System.Collections.Immutable;
using Funcky.Extensions;
using Microsoft.CodeAnalysis;

namespace Funcky.SourceGenerator;

[Generator(LanguageNames.CSharp)]
public sealed class ApplyGenerator : IIncrementalGenerator
{
    private const int MaxParameterCount = 4;
    private const string ResultTypeParameter = "TResult";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static context => context.AddSource("FuncExtensions.Apply.g.cs", GenerateApplyExtensions()));
        context.RegisterPostInitializationOutput(static context => context.AddSource("Functional.Apply.g.cs", GenerateApplyFunctions()));
    }

    private static string GenerateApplyExtensions()
        => $$"""
             #nullable enable

             namespace Funcky.Extensions;

             public static partial class FuncExtensions
             {
                 {{string.Join("\n\n    ", GenerateApplyMethods(extensionMethod: true))}}
             }
             """;

    private static string GenerateApplyFunctions()
        => $$"""
             #nullable enable

             namespace Funcky;

             public static partial class Functional
             {
                 {{string.Join("\n\n    ", GenerateApplyMethods(extensionMethod: false))}}
             }
             """;

    private static IEnumerable<string> GenerateApplyMethods(bool extensionMethod)
        => (from parameterCount in Enumerable.Range(2, count: MaxParameterCount)
            from unitParameters in Enumerable.Range(0, count: parameterCount).PowerSet()
            let unitParameters_ = unitParameters.ToImmutableArray()
            where unitParameters_.Length != 0 && unitParameters_.Length != parameterCount
            select GenerateApplyMethod(parameterCount, unitParameters_, extensionMethod));

    private static string GenerateApplyMethod(int parameterCount, ImmutableArray<int> unitParameters, bool extensionMethod)
    {
        var returnType = ReturnType(unitParameters);
        var typeParameters = string.Join(", ", TypeParameters(parameterCount));
        var @this = extensionMethod ? "this " : string.Empty;
        var funcType = FuncType(TypeParameters(parameterCount));
        var parameters = string.Join(", ", Parameters(parameterCount, unitParameters));
        var lambdaParameters = string.Join(", ", LambdaParameters(unitParameters));
        var arguments = string.Join(", ", Arguments(parameterCount, unitParameters));
        return $"public static {returnType} Apply<{typeParameters}>({@this}{funcType} func, {parameters}) => ({lambdaParameters}) => func({arguments});";
    }

    private static IEnumerable<string> TypeParameters(int parameterCount)
        => Enumerable.Range(0, count: parameterCount)
            .Select(TypeParameter)
            .Concat([ResultTypeParameter]);

    private static string ReturnType(ImmutableArray<int> unitParameters)
        => FuncType(unitParameters
            .Select(TypeParameter)
            .Concat([ResultTypeParameter]));

    private static string FuncType(IEnumerable<string> typeParameters) => $"Func<{string.Join(", ", typeParameters)}>";

    private static string TypeParameter(int index) => $"T{index + 1}";

    private static IEnumerable<string> Parameters(int count, ImmutableArray<int> unitParameters)
        => Enumerable.Range(0, count).Select(index => Parameter(index, unitParameters));

    private static string Parameter(int index, ImmutableArray<int> unitParameters)
        => $"{ParameterType(index, unitParameters)} {ParameterName(index)}";

    private static string ParameterName(int index) => $"p{index + 1}";

    private static string ParameterType(int index, ImmutableArray<int> unitParameters)
        => unitParameters.Contains(index)
            ? "Unit"
            : TypeParameter(index);

    private static IEnumerable<string> LambdaParameters(ImmutableArray<int> unitParameters)
        => unitParameters.Select(LambdaParameterName);

    private static string LambdaParameterName(int index) => $"p{index + 1}_";

    private static IEnumerable<string> Arguments(int count, ImmutableArray<int> unitParameters)
        => Enumerable.Range(0, count).Select(index => Argument(index, unitParameters));

    private static string Argument(int index, ImmutableArray<int> unitParameters)
        => unitParameters.Contains(index)
            ? LambdaParameterName(index)
            : ParameterName(index);
}
