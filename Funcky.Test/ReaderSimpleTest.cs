using System.Threading.Tasks;
using Funcky.Monads;
using Xunit;

namespace Funcky.Test
{
    public class ReaderSimpleTest
    {
        [Fact]
        public async Task GivenTheGreetExampleIGetTheRightText()
        {
            Assert.Equal("(Hi, John!, Hi, Jose!)", (await GreetGuys().Apply(new Config { Template = "Hi, {0}!" })).ToString());
            Assert.Equal("(¡Hola, John!, ¡Hola, Jose!)", (await GreetGuys().Apply(new Config { Template = "¡Hola, {0}!" })).ToString());
        }

        // These functions do not have any link to any instance of the Config class.
        public static async Reader<(string GreetJohn, string GreetJose)> GreetGuys()
            => (await Greet("John"), await Greet("Jose"));

        private static async Reader<string> Greet(string name)
            => string.Format(await ExtractTemplate(), name);

        private static async Reader<string> ExtractTemplate()
            => await Reader<string>.Read<Config>(c => c.Template);

        private struct Config
        {
            public string Template;
        }
    }
}
