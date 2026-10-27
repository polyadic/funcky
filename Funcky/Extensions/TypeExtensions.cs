namespace Funcky.Extensions;

public static class TypeExtensions
{
    extension(Type type)
    {
        /// <summary>The type the type directly inherits from, or <c>None</c> for <see cref="object"/>, interfaces and pointer types,
        /// where <see cref="Type.BaseType"/> is <see langword="null"/>.</summary>
        public Option<Type> BaseTypeOrNone
            => Option.FromNullable(type.BaseType);
    }
}
