namespace Xunit.Sdk
{
    internal static class FormatUtility
    {
        public static string FormatResult(Result<object?> result)
            => result.Match(
                ok: value => $"Ok({value})",
                error: FormatException);

        public static string FormatException(Exception exception)
            => $"{exception.GetType().FullName}: {exception.Message}";
    }
}
