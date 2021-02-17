using System.Diagnostics.Contracts;
using Funcky.Monads;

namespace Funcky.Extensions
{
    public static partial class StringExtensions
    {
        private const int NotFoundValue = -1;

        [Pure]
        private static Option<int> MapIndexToOption(int index)
            => index == NotFoundValue
                ? Option<int>.None()
                : index;
    }
}
