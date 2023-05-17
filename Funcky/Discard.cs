namespace Funcky;

public static class Discard
{
    /// <summary>A convenient shortcut for getting a <see cref="Unit"/>.
    /// Useful for using switch expressions purely with guards when matching on non-constant values
    /// or as an alternative to <c>if</c> / <c>else if</c> / <c>else</c> chains.</summary>
    /// <example><code>
    /// using static Funcky.Discard;
    /// return __ switch
    /// {
    ///     _ when user.IsFrenchAdmin() => "le sécret",
    ///     _ when user.IsAdmin() => "secret",
    ///     _ => "(redacted)",
    /// };
    /// </code></example>
    /// <remarks>The name is intentionally two underscores as to not conflict with C#'s discard syntax.</remarks>
    public static readonly Unit __ = Unit.Value;
}
