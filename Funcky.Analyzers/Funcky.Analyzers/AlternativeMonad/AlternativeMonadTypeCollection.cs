using Microsoft.CodeAnalysis;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers.AlternativeMonad;

internal sealed class AlternativeMonadTypeCollection
{
    private AlternativeMonadTypeCollection(IReadOnlyDictionary<ISymbol, AlternativeMonadType> value)
    {
        Value = value;
    }

    public IReadOnlyDictionary<ISymbol, AlternativeMonadType> Value { get; }

    public static AlternativeMonadTypeCollection FromCompilation(Compilation compilation)
    {
        var alternativeMonadTypes = new Dictionary<ISymbol, AlternativeMonadType>(SymbolEqualityComparer.Default);
        AddAlternativeMonadType(alternativeMonadTypes, Option(compilation));
        AddAlternativeMonadType(alternativeMonadTypes, Either(compilation));
        AddAlternativeMonadType(alternativeMonadTypes, Result(compilation));
        return new(alternativeMonadTypes);
    }

    private static void AddAlternativeMonadType(IDictionary<ISymbol, AlternativeMonadType> dictionary, AlternativeMonadType? alternativeMonadType)
    {
        if (alternativeMonadType is not null)
        {
            dictionary.Add(alternativeMonadType.Type, alternativeMonadType);
        }
    }

    private static AlternativeMonadType? Option(Compilation compilation)
        => compilation.GetOptionOfTType() is { } optionOfTType
            && compilation.GetOptionType() is { } optionType
            ? new AlternativeMonadType(
                optionOfTType,
                constructorsType: optionType,
                errorStateConstructorName: OptionNonePropertyName,
                returnAlias: OptionSomeMethodName,
                extensionsType: compilation.GetOptionExtensionsType())
            : null;

    private static AlternativeMonadType? Either(Compilation compilation)
        => compilation.GetEitherOfTType() is { } eitherOfTType
            && compilation.GetEitherType() is { } eitherType
            ? new AlternativeMonadType(
                eitherOfTType,
                constructorsType: eitherType,
                errorStateConstructorName: EitherLeftMethodName,
                returnAlias: EitherRightMethodName)
            : null;

    private static AlternativeMonadType? Result(Compilation compilation)
        => compilation.GetResultOfTType() is { } resultOfTType
            && compilation.GetResultType() is { } resultType
            ? new AlternativeMonadType(
                resultOfTType,
                constructorsType: resultType,
                errorStateConstructorName: ResultErrorMethodName,
                matchHasSuccessStateFirst: true,
                returnAlias: ResultOkMethodName)
            : null;
}
