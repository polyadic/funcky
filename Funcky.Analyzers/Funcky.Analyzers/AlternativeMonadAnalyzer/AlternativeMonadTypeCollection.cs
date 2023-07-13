using System.Collections;
using Microsoft.CodeAnalysis;
using static Funcky.Analyzers.FunckyWellKnownMemberNames;

namespace Funcky.Analyzers;

internal sealed class AlternativeMonadTypeCollection : IReadOnlyDictionary<ISymbol, AlternativeMonadType>
{
    private readonly IReadOnlyDictionary<ISymbol, AlternativeMonadType> _types;

    private AlternativeMonadTypeCollection(IReadOnlyDictionary<ISymbol, AlternativeMonadType> types)
    {
        _types = types;
    }

    public int Count => _types.Count;

    public IEnumerable<ISymbol> Keys => _types.Keys;

    public IEnumerable<AlternativeMonadType> Values => _types.Values;

    public AlternativeMonadType this[ISymbol key] => _types[key];

    public static AlternativeMonadTypeCollection FromCompilation(Compilation compilation)
    {
        var alternativeMonadTypes = new Dictionary<ISymbol, AlternativeMonadType>(SymbolEqualityComparer.Default);
        AddAlternativeMonadType(alternativeMonadTypes, Option(compilation));
        AddAlternativeMonadType(alternativeMonadTypes, Either(compilation));
        AddAlternativeMonadType(alternativeMonadTypes, Result(compilation));
        return new(alternativeMonadTypes);
    }

    public IEnumerator<KeyValuePair<ISymbol, AlternativeMonadType>> GetEnumerator() => _types.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool ContainsKey(ISymbol key) => _types.ContainsKey(key);

    public bool TryGetValue(ISymbol key, out AlternativeMonadType value) => _types.TryGetValue(key, out value);

    private static void AddAlternativeMonadType(IDictionary<ISymbol, AlternativeMonadType> dictionary, AlternativeMonadType? alternativeMonadType)
    {
        if (alternativeMonadType is not null)
        {
            dictionary.Add(alternativeMonadType.Type, alternativeMonadType);
        }
    }

    private static AlternativeMonadType? Option(Compilation compilation)
        => compilation.GetOptionOfTType() is { } optionType
            ? new AlternativeMonadType(
                optionType,
                matchHasSuccessStateFirst: false,
                returnAlias: OptionSomeMethodName,
                extensionsType: compilation.GetOptionExtensionsType())
            : null;

    private static AlternativeMonadType? Either(Compilation compilation)
        => compilation.GetEitherOfTType() is { } optionType
            ? new AlternativeMonadType(
                optionType,
                matchHasSuccessStateFirst: false,
                returnAlias: EitherRightMethodName)
            : null;

    private static AlternativeMonadType? Result(Compilation compilation)
        => compilation.GetResultOfTType() is { } optionType
            ? new AlternativeMonadType(
                optionType,
                matchHasSuccessStateFirst: true,
                returnAlias: ResultOkMethodName)
            : null;
}
