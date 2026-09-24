namespace Funcky.CodeAnalysis;

#pragma warning disable CS9113 // Parameter is unread.
[AttributeUsage(AttributeTargets.Property)]
internal sealed class SyntaxSupportOnlyAttribute(string syntaxFeature) : Attribute
#pragma warning restore CS9113 // Parameter is unread.
{
    public const string ListPattern = "list pattern";
}
