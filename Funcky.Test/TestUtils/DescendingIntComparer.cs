using System.Collections.Generic;

namespace Funcky.Test.TestUtils
{
    internal class DescendingIntComparer : IComparer<int>
    {
        private DescendingIntComparer()
        {
        }

        public static DescendingIntComparer Create()
        {
            return new DescendingIntComparer();
        }

        public int Compare(int x, int y)
        {
            return y - x;
        }
    }
}
