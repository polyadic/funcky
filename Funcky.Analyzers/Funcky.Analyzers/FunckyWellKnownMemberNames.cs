namespace Funcky.Analyzers;

public static class FunckyWellKnownMemberNames
{
    public const string MonadReturnMethodName = "Return";

    public const string OptionSomeMethodName = "Some";

    public const string OptionFromBooleanMethodName = "FromBoolean";

    /// <summary>The <c>Then</c> extension on <c>bool</c>, the replacement of <c>Option.FromBoolean</c>.</summary>
    public const string BooleanThenMethodName = "Then";

    /// <summary>The <c>value</c> parameter of <c>bool.Then</c>; naming it keeps a delegate out of the selector overloads.</summary>
    public const string BooleanThenValueParameterName = "value";

    /// <summary>The <c>Value</c> property of <c>Unit</c>.</summary>
    public const string UnitValuePropertyName = "Value";

    public const string OptionFromNullableMethodName = "FromNullable";

    public const string OptionNonePropertyName = "None";

    /// <summary>The <c>Match</c> parameters that handle the error state of each alternative monad.</summary>
    public const string OptionMatchNoneParameterName = "none";

    public const string EitherMatchLeftParameterName = "left";

    public const string ResultMatchErrorParameterName = "error";

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

    /// <summary>The <c>Select</c> method on monadic types.</summary>
    public const string SelectMethodName = "Select";

    /// <summary>The <c>SelectMany</c> method on monadic types.</summary>
    public const string SelectManyMethodName = "SelectMany";

    /// <summary>The <c>Where</c> method on monad-like types.</summary>
    public const string WhereMethodName = "Where";

    /// <summary>The <c>ToNullable</c> method on alternative monad types.</summary>
    public const string ToNullableMethodName = "ToNullable";

    public static class XunitAssert
    {
        public const string EqualMethodName = "Equal";
    }
}
