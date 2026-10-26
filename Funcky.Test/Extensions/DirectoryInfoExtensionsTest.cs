namespace Funcky.Test.Extensions;

public sealed class DirectoryInfoExtensionsTest
{
    [Fact]
    public void ParentOrNoneReturnsTheParentDirectory()
    {
        var root = new DirectoryInfo(Path.GetTempPath()).Root;
        var child = new DirectoryInfo(Path.Combine(root.FullName, "child"));

        var parent = FunctionalAssert.Some(child.ParentOrNone);

        Assert.Equal(root.FullName, parent.FullName);
    }

    [Fact]
    public void ParentOrNoneReturnsNoneForARootDirectory()
    {
        FunctionalAssert.None(new DirectoryInfo(Path.GetTempPath()).Root.ParentOrNone);
    }
}
