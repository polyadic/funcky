using System.Collections;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.Operations;

namespace Funcky.Analyzers;

internal static class ArgumentOperationExtensions
{
    public static IReadOnlyList<IArgumentOperation> InDeclarationOrder(this IReadOnlyList<IArgumentOperation> arguments)
        => new ArgumentsInDeclarationOrder(arguments);

    private sealed class ArgumentsInDeclarationOrder : IReadOnlyList<IArgumentOperation>
    {
        private readonly IReadOnlyList<IArgumentOperation> _arguments;
        private readonly Lazy<IReadOnlyList<IArgumentOperation>> _reorderedArguments;

        public ArgumentsInDeclarationOrder(IReadOnlyList<IArgumentOperation> arguments)
        {
            _arguments = arguments;
            _reorderedArguments = new Lazy<IReadOnlyList<IArgumentOperation>>(() => _arguments.OrderBy(argument => argument.Parameter?.Ordinal ?? 0).ToImmutableArray());
        }

        public int Count => _arguments.Count;

        public IArgumentOperation this[int index] => _reorderedArguments.Value[index];

        public IEnumerator<IArgumentOperation> GetEnumerator() => _reorderedArguments.Value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
