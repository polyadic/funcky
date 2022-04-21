using static Funcky.SourceGenerator.TemplateLoader;

namespace Funcky.SourceGenerator.Test;

public class TemplateLoaderTest
{
    [Fact]
    public void CanLoadTemplateAsStringFromResourceName()
    {
        Assert.StartsWith("using System;", CodeFromTemplate("OrNoneFromTryPatternAttribute"));
    }
}
