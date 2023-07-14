namespace Funcky.Analyzers;

public static class FunckyWellKnownMemberNames
{
    public const string MonadReturnMethodName = "Return";

    public const string OptionSomeMethodName = "Some";

    public const string OptionFromBooleanMethodName = "FromBoolean";

    public const string OptionNonePropertyName = "None";

    public const string EitherRightMethodName = "Right";

    public const string EitherLeftMethodName = "Left";

    public const string ResultOkMethodName = "Ok";

    public const string ResultErrorMethodName = "Error";

    /// <summary>The name of the identity function.</summary>
    public const string IdentityMethodName = "Identity";

    /// <summary>The <c>Match</c> method on Either-like types.</summary>
    public const string MatchMethodName = "Match";

    /// <summary>The <c>GetOrElse</c> method on Either-like types.</summary>
    public const string GetOrElseMethodName = "GetOrElse";

    /// <summary>The <c>GetOrElse</c> method on Either-like types.</summary>
    public const string OrElseMethodName = "OrElse";

    /// <summary>The <c>SelectMany</c> method on monadic types.</summary>
    public const string SelectManyMethodName = "SelectMany";

    /// <summary>The <c>Where</c> method on monad-like types.</summary>
    public const string WhereMethodName = "Where";

    /// <summary>The <c>ToNullable</c> method on alternative monad types.</summary>
    public const string ToNullableMethodName = "ToNullable";
}
