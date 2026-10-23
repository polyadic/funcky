namespace Funcky.Xunit.Serializers;

internal static class StringExtensions
{
    extension(string s)
    {
        public Option<string> StripPrefix(string prefix)
            => s.StartsWith(prefix)
                ? s[prefix.Length..]
                : Option<string>.None;
    }
}
