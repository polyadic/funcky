using System.Runtime.CompilerServices;

namespace Funcky.Internal.Validators;

internal static class ArgumentValidator
{
    /// <summary>Throws an <see cref="ArgumentNullException"/> when <paramref name="argument"/> is <see langword="null"/>, like
    /// <c>ArgumentNullException.ThrowIfNull</c>, which is not available on all target frameworks.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T NotNull<T>(T argument, [CallerArgumentExpression(nameof(argument))] string parameterName = "")
        where T : class
        => argument ?? throw new ArgumentNullException(parameterName);
}
