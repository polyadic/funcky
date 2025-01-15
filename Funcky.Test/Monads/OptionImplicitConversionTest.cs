using Funcky.Test.TestUtils;

namespace Funcky.Test.Monads;

public sealed class OptionImplicitConversionTest
{
    [Fact]
    public void ImplicitFunctionParameterConversionDoesCompile()
    {
        AcceptStringOption("Hello world");
        AcceptIntOption(1337);
        AcceptCovariantOption(Comparer<int>.Default);
    }

    [Fact]
    public void ImplicitReturnDoesCompile()
    {
        ReturnStringOption();
        ReturnIntOption();
    }

    [Fact]
    public void OverloadResolutionPrefersTypeWithoutImplicitConversion()
    {
        Assert.True(OverloadResolution(42));
        Assert.False(OverloadResolution(Option.Some(42)));
    }

    [Fact]
    public void ImplicitAssignmentCompiles()
    {
        Option<int> number = 12;

        var v = FunctionalAssert.Some(number);
        Assert.Equal(12, v);
    }

    [Fact]
    public void NullAssignmentThrows()
    {
        Assert.Throws<ArgumentNullException>(TryNullAssignment);
    }

    [Fact]
    public void ImplicitConversionOnlyWorkOnInstancesOfClassType()
    {
        // The specs do not allow the implicit operator with generics on interfaces
        var instance = new MyClass();

        // Class type does work
        Foo(instance);

        // Interface type does not: this works as specified by C#
        // https://stackoverflow.com/questions/39618845/implicit-operator-with-generic-not-working-for-interface
        // Foo((IMyInterface)instance);
    }

    private void Foo(Option<IMyInterface> foo)
    {
    }

    private void TryNullAssignment()
    {
        Option<string> number = null!;
    }

    private void AcceptStringOption(Option<string> s)
    {
    }

    private void AcceptIntOption(Option<int> i)
    {
    }

    private void AcceptCovariantOption(Option<IComparer<int>> c = default)
    {
    }

    private Option<string> ReturnStringOption()
        => "Hello world!";

    private Option<int> ReturnIntOption()
        => 42;

    private bool OverloadResolution(int i)
        => true;

    private bool OverloadResolution(Option<int> i)
        => false;
}
