namespace Funcky.Test
{
    internal class FunctionalAssert
    {
        public static void Unmatched()
            => throw new UnmatchedException();

        public static void Unmatched(string unmatchedCase)
            => throw new UnmatchedException(unmatchedCase);
    }
}
