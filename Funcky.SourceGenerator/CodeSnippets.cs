namespace Funcky.SourceGenerator;

internal static class CodeSnippets
{
    public const string OrNoneFromTryPatternAttribute =
        """
        namespace Funcky.Internal
        {
            [global::System.Diagnostics.Conditional("COMPILE_TIME_ONLY")]
            [global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = true)]
            internal class OrNoneFromTryPatternAttribute : global::System.Attribute
            {
                public OrNoneFromTryPatternAttribute(global::System.Type type, string method)
                    => (Type, Method) = (type, method);

                public global::System.Type Type { get; }

                public string Method { get; }
            }
        }
        """;
}
