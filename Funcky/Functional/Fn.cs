using System.Runtime.CompilerServices;

namespace Funcky;

public static partial class Functional
{
    /// <summary>Infers a method group to its natural type i.e. <c>Func&lt;T, ...&gt;</c> and <c>Action&lt;T, ...&gt;</c>.</summary>
    /// <remarks>This function is identical to <see cref="Identity{T}"/>.
    /// Prefer the latter for all other cases.</remarks>
    /// <example>
    /// Sometimes, the C# compiler is unable to infer a method group's natural type.
    /// <code><![CDATA[
    /// var pow = Curry(Math.Pow);
    /// //        ^^^^^
    /// // error CS0411: The type arguments for
    /// // method 'Functional.Curry<...>' cannot
    /// // be inferred from the usage.
    /// ]]></code>
    /// This function can sometimes help with that:
    /// <code><![CDATA[
    /// var pow = Curry(Fn(Math.Pow));
    /// // or:
    /// var pow = Fn(Math.Pow).Curry();
    /// ]]></code>
    /// </example>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Fn<T>(T value) => value;
}
