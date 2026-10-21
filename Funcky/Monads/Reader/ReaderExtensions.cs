namespace Funcky.Monads;

public static partial class ReaderExtensions
{
    extension<TEnvironment, TItem>(Reader<TEnvironment, Reader<TEnvironment, TItem>> reader)
        where TEnvironment : notnull
        where TItem : notnull
    {
        public Reader<TEnvironment, TItem> Flatten()
            => reader.SelectMany(Identity);
    }
}
