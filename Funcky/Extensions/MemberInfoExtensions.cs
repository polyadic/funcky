using System.Reflection;

namespace Funcky.Extensions;

public static class MemberInfoExtensions
{
    extension(MemberInfo member)
    {
        /// <summary>The type that declares the member, or <c>None</c> for a type that is not nested and for a global member of a
        /// module, where <see cref="MemberInfo.DeclaringType"/> is <see langword="null"/>.</summary>
        public Option<Type> DeclaringTypeOrNone
            => Option.FromNullable(member.DeclaringType);
    }
}
