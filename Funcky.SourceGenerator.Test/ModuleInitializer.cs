using System.Runtime.CompilerServices;

namespace Funcky.SourceGenerator.Test;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Enable();
    }
}
