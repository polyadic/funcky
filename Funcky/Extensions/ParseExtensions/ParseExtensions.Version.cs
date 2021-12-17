using Funcky.Internal;

namespace Funcky.Extensions;

public static partial class ParseExtensions
{
    /// <seealso cref="Version.TryParse(string?,out System.Version?)"/>
    public static Option<Version> ParseVersionOrNone(this string? input)
        => FailToOption<Version>.FromTryPattern(Version.TryParse, input);

    #if PARSE_READ_ONLY_SPAN_SUPPORTED
    /// <seealso cref="Version.TryParse(System.ReadOnlySpan{char},out System.Version?)"/>
    public static Option<Version> ParseVersionOrNone(this ReadOnlySpan<char> input)
        => Version.TryParse(input, out var version)
            ? version
            : Option<Version>.None();
    #endif
}
