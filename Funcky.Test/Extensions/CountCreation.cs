namespace Funcky.Test.Extensions
{
    internal class CountCreation
    {
        public CountCreation()
        {
            Count += 1;
        }

        public static int Count { get; private set; }
    }
}
