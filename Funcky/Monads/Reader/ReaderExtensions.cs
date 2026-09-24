namespace Funcky.Monads;

public static partial class ReaderExtensions
{
    public static Reader<TEnvironment, TItem> Flatten<TEnvironment, TItem>(this Reader<TEnvironment, Reader<TEnvironment, TItem>> reader)
        where TEnvironment : notnull
        where TItem : notnull
        => reader.SelectMany(Identity);
}
