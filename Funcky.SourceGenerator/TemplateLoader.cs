using System.Reflection;

namespace Funcky.SourceGenerator;

internal class TemplateLoader
{
    public static string CodeFromTemplate(string source)
        => Assembly.GetExecutingAssembly().GetManifestResourceStream(GetTemplateName(source)) is { } resourceStream
            ? ResourceString(resourceStream)
            : throw new FileNotFoundException("Resource not found", GetTemplateName(source));

    private static string ResourceString(Stream resourceStream)
    {
        using var reader = new StreamReader(resourceStream);

        return reader.ReadToEnd();
    }

    private static string GetTemplateName(string source) =>
        $"Funcky.SourceGenerator.Templates.{source}.template";
}
