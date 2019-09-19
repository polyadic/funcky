using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funcky.Monads;
using Xunit;

namespace Funcky.Test
{
    public class ReaderSimpleTest
    {
        class Config { public string Template; }

        [Fact]
        public static async Task Main()
        {
            Console.WriteLine(await GreetGuys().Apply(new Config { Template = "Hi, {0}!" }));
            //(Hi, John!, Hi, Jose!)

            Console.WriteLine(await GreetGuys().Apply(new Config { Template = "¡Hola, {0}!" }));
            //(¡Hola, John!, ¡Hola, Jose!)
        }

        //These functions do not have any link to any instance of the Config class.
        public static async Reader<(string gJohn, string gJose)> GreetGuys()
            => (await Greet("John"), await Greet("Jose"));

        static async Reader<string> Greet(string name)
            => string.Format(await ExtractTemplate(), name);

        static async Reader<string> ExtractTemplate()
            => await Reader<string>.Read<Config>(c => c.Template);
    }

    public class Database
    {
        public static Database ConnectTo(int id)
        {
            if (id == 100)
            {
                return new Database();
            }
            throw new Exception("Wrong database");
        }

        private Database() { }

        private static readonly (int Id, string FirstName, string LastName, int Win)[] Data =
        {
            (1, "John","Smith", 110),
            (2, "Mary","Louie", 30),
            (3, "Louis","Slaughter", 47),
        };

        public async Task<string> GetFirstName(int id)
        {
            await Task.Delay(50);
            return Data.Single(i => i.Id == id).FirstName;
        }

        public async Task<string> GetLastName(int id)
        {
            await Task.Delay(50);
            return Data.Single(i => i.Id == id).LastName;
        }

        public async Task<int> GetWin(int id)
        {
            await Task.Delay(50);
            return Data.Single(i => i.Id == id).Win;
        }
    }
}
