using System.Threading.Tasks;
using Funcky.Monads;
using Xunit;
using Xunit.Abstractions;

namespace Funcky.Test
{
    public class ReaderSimpleTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public ReaderSimpleTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        class Config { public string Template; }

        [Fact]
        public async Task Main()
        {
            _testOutputHelper.WriteLine((await GreetGuys().Apply(new Config { Template = "Hi, {0}!" })).ToString());

            _testOutputHelper.WriteLine((await GreetGuys().Apply(new Config { Template = "¡Hola, {0}!" })).ToString());
        }

        //These functions do not have any link to any instance of the Config class.
        public static async Reader<(string gJohn, string gJose)> GreetGuys()
            => (await Greet("John"), await Greet("Jose"));

        static async Reader<string> Greet(string name)
            => string.Format(await ExtractTemplate(), name);

        static async Reader<string> ExtractTemplate()
            => await Reader<string>.Read<Config>(c => c.Template);
    }
}
