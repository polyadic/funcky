namespace Funcky.Async.Test.TestUtilities
{
    internal sealed class DescendingIntComparer : IComparer<int>
    {
        private DescendingIntComparer()
        {
        }

        public static DescendingIntComparer Create()
            => new();

        public int Compare(int x, int y)
            => y - x;
    }
}
