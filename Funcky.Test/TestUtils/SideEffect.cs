namespace Funcky.Test
{
    internal class SideEffect
    {
        public bool IsDone { get; private set; }

        public void Do() => IsDone = true;
    }
}