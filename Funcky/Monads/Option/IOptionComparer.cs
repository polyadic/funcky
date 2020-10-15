using System.Collections;
using System.Collections.Generic;

namespace Funcky.Monads
{
    internal interface IOptionComparer<TItem> : IComparer<Option<TItem>>, IComparer
        where TItem : notnull
    {
    }
}
