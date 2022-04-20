using Microsoft.CodeAnalysis;

namespace Funcky.Analyzers;

internal static class LocalizedResourceLoader
{
    internal static LocalizableString LoadFromResource(string resourceName)
        => new LocalizableResourceString(resourceName, Resources.ResourceManager, typeof(Resources));
}
