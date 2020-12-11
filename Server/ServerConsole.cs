using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dfe.Server
{
    public static class ServerConsole
    {

        public static async Task processInput()
        {
            while (true)
            {
                string input = Console.ReadLine();
                Console.WriteLine("Input: {0}", input);
            }
        }
    }
}
