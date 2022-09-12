using System.ComponentModel;
using static System.ComponentModel.EditorBrowsableState;

namespace Funcky.Monads;

public static partial class Option
{
    /// <summary>
    /// Creates an <see cref="Option{Unit}"/> from a boolean value.
    /// </summary>
    /// <param name="boolean">A boolean value.</param>
    /// <returns> boolean:true returns Some, boolean:false returns None.</returns>
    [EditorBrowsable(Advanced)]
    public static Option<Unit> FromBoolean(bool boolean)
        => FromBoolean(boolean, Unit.Value);

    /// <summary>
    /// Returns an <see cref="Option{TItem}" /> where the given item is returned when the boolean is true.
    /// </summary>
    /// <typeparam name="TItem">Type of the returned item.</typeparam>
    /// <param name="boolean">A boolean value.</param>
    /// <param name="item">A value which is returned when boolean is true.</param>
    /// <returns> boolean:true returns Some(item), boolean:false returns None.</returns>
    [EditorBrowsable(Advanced)]
    public static Option<TItem> FromBoolean<TItem>(bool boolean, TItem item)
        where TItem : notnull
        => FromBoolean(boolean, () => item);

    /// <summary>
    /// Returns an <see cref="Option{TItem}" /> where the given item is returned from a selector function if the boolean is true.
    /// </summary>
    /// <typeparam name="TItem">Type of the returned item.</typeparam>
    /// <param name="boolean">A boolean value.</param>
    /// <param name="selector">A function which returns a value of type TItem.</param>
    /// <returns> boolean:true returns Some(selector()), boolean:false returns None.</returns>
    [EditorBrowsable(Advanced)]
    public static Option<TItem> FromBoolean<TItem>(bool boolean, Func<TItem> selector)
        where TItem : notnull
        => boolean
            ? selector()
            : Option<TItem>.None;
}
