using System;
using System.Linq;
using System.Threading.Tasks;

namespace Funcky.Test
{
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

        private Database()
        {
        }

        private static readonly (int Id, string FirstName, string LastName, int Win)[] Data =
        {
            (1, "John", "Smith", 110),
            (2, "Mary", "Louie", 30),
            (3, "Louis", "Slaughter", 47),
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