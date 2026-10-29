using System.Reflection;

namespace Funcky.Test.Extensions;

public sealed class AssemblyExtensionsTest
{
    [Fact]
    public void EntryPointOrNoneReturnsNoneForAClassLibrary()
    {
        Assert.None(typeof(Option).Assembly.EntryPointOrNone);
    }

    [Fact]
    public void EntryPointOrNoneReturnsTheEntryPointOfAnExecutable()
    {
        // The test host is an executable; only its entry point is of interest, not what it does.
        var entryAssembly = Assembly.GetEntryAssembly();

        if (entryAssembly?.EntryPoint is { } entryPoint)
        {
            Assert.Some(entryPoint, entryAssembly.EntryPointOrNone);
        }
    }
}
