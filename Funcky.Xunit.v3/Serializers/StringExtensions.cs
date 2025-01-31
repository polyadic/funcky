namespace Funcky.Xunit.Serializers;

internal static class StringExtensions
{
    public static Option<string> StripPrefix(this string s, string prefix)
        => s.StartsWith(prefix)
            ? s[prefix.Length..]
            : Option<string>.None;
}
