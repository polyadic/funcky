using System.Collections.Generic;
using System.Threading.Tasks;
using Funcky.Monads;
using Xunit;

namespace Funcky.Test
{
    public class ReaderTest
    {
        [Fact]
        public async Task MainTest()
        {
            int[] ids = { 1, 2, 3 };

            Configuration[] configurations =
            {
                new Configuration(100, "Congratulations, {0}! You won {1}$!", "{0} {1}"),
                new Configuration(100, "¡Felicidades, {0}! Ganaste {1} $", "{0}"),
            };

            var expected = new[]
            {
                "Congratulations, John Smith! You won 110$!",
                "Congratulations, Mary Louie! You won 30$!",
                "Congratulations, Louis Slaughter! You won 47$!",
                "¡Felicidades, John! Ganaste 110 $",
                "¡Felicidades, Mary! Ganaste 30 $",
                "¡Felicidades, Louis! Ganaste 47 $",
            };

            var actual = new List<string>();

            foreach (var configuration in configurations)
            {
                foreach (var userId in ids)
                {
                    // The logic receives only a single explicit parameter - userId
                    var logic = GetGreeting(userId);

                    // The rest of parameters (database Id, templates) can be passed implicitly
                    var greeting = await logic.Apply(configuration);

                    actual.Add(greeting);
                }
            }

            Assert.Equal(expected, actual);
        }

        private static async Reader<string> GetGreeting(int userId)
        {
            var template = await Reader<string>.Read<Configuration>(cfg => cfg.GreetingTemplate);

            var fullName = await GetFullName(userId);

            var win = await GetWin(userId);

            return string.Format(template, fullName, win);
        }

        private static async Reader<string> GetFullName(int userId)
        {
            var template = await Reader<string>.Read<Configuration>(cfg => cfg.NameFormat);

            var firstName = await GetFirstName(userId);
            var lastName = await GetLastName(userId);

            return string.Format(template, firstName, lastName);
        }

        private static async Reader<string> GetFirstName(int userId)
        {
            var dataBase = await GetDataBase();
            return await dataBase.GetFirstName(userId);
        }

        private static async Reader<string> GetLastName(int userId)
        {
            var dataBase = await GetDataBase();
            return await dataBase.GetLastName(userId);
        }

        private static async Reader<int> GetWin(int userId)
        {
            var dataBase = await GetDataBase();
            return await dataBase.GetWin(userId);
        }

        private static async Reader<Database> GetDataBase()
        {
            var dataBaseId = await Reader<int>.Read<Configuration>(cfg => cfg.DataBaseId);
            return Database.ConnectTo(dataBaseId);
        }

        public readonly struct Configuration
        {
            public readonly int DataBaseId;

            public readonly string GreetingTemplate;

            public readonly string NameFormat;

            public Configuration(int dataBaseId, string greetingTemplate, string nameFormat)
            {
                DataBaseId = dataBaseId;
                GreetingTemplate = greetingTemplate;
                NameFormat = nameFormat;
            }
        }
    }
}