//HintName: OrNoneFromTryPatternAttribute.g.cs
using System;

namespace Funcky.Internal
{
    [global::System.Diagnostics.Conditional("COMPILE_TIME_ONLY")]
    [global::System.AttributeUsage(global::System.AttributeTargets.Class, AllowMultiple = true)]
    internal class OrNoneFromTryPatternAttribute : Attribute
    {
        public OrNoneFromTryPatternAttribute(Type type, string method)
            => (Type, Method) = (type, method);

        public Type Type { get; }

        public string Method { get; }
    }
}
