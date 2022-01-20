//HintName: OrNoneFromTryPatternAttribute.g.cs
using System;

namespace Funcky.Internal
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class OrNoneFromTryPatternAttribute : Attribute
    {
        public OrNoneFromTryPatternAttribute(Type type, string method)
            => (Type, Method) = (type, method);

        public Type Type { get; }

        public string Method { get; }
    }
}
