using System.Collections.Generic;

namespace Funcky.Test.TestUtils
{
    internal sealed class DescendingIntComparer : IComparer<int>
    {
        private DescendingIntComparer()
        {
        }

        public static DescendingIntComparer Create()
            => new DescendingIntComparer();

        public int Compare(int x, int y)
            => y - x;
    }
}
